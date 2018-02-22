﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;

using BizHawk.Common;
using BizHawk.Common.NumberExtensions;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Common.IEmulatorExtensions;

namespace BizHawk.Client.Common
{
	/// <summary>
	/// Captures savestates and manages the logic of adding, retrieving, 
	/// invalidating/clearing of states.  Also does memory management and limiting of states
	/// </summary>
	public class TasStateManager : IDisposable
	{
		// TODO: pass this in, and find a solution to a stale reference (this is instantiated BEFORE a new core instance is made, making this one stale if it is simply set in the constructor
		private IStatable Core => Global.Emulator.AsStatable();

		public Action<int> InvalidateCallback { get; set; }

		private void CallInvalidateCallback(int index)
		{
			InvalidateCallback?.Invoke(index);
		}
		
		internal NDBDatabase NdbDatabase { get; set; }
		private Guid _guid = Guid.NewGuid();
		private SortedList<int, StateManagerState> _states = new SortedList<int, StateManagerState>();

		private string StatePath
		{
			get
			{
				var basePath = PathManager.MakeAbsolutePath(Global.Config.PathEntries["Global", "TAStudio states"].Path, null);
				return Path.Combine(basePath, _guid.ToString());
			}
		}

		private long _stateCleanupTime;
		private readonly long _stateCleanupPeriod = 10000;

		private bool _isMountedForWrite;
		private readonly TasMovie _movie;

		private ulong _expectedStateSize;
		private readonly int _minFrequency = 1;
		private readonly int _maxFrequency = 16;
		private int _maxStates => (int)(Settings.Cap / _expectedStateSize) +
			(int)((ulong)Settings.DiskCapacitymb * 1024 * 1024 / _expectedStateSize);
		private int _fileStateGap => 1 << Settings.FileStateGap;

		private int StateFrequency
		{
			get
			{
				return NumberExtensions.Clamp(
					((int)_expectedStateSize / Settings.MemStateGapDivider / 1024),
					_minFrequency, _maxFrequency);
			}
		}

		public TasStateManager(TasMovie movie)
		{
			_movie = movie;
			Settings = new TasStateManagerSettings(Global.Config.DefaultTasProjSettings);

			if (_movie.StartsFromSavestate)
			{
				SetState(0, _movie.BinarySavestate);
			}

			_stateCleanupTime = DateTime.Now.Ticks + _stateCleanupPeriod;
		}

		public void Dispose()
		{
			// States and BranchStates don't need cleaning because they would only contain an ndbdatabase entry which was demolished by the below
			NdbDatabase?.Dispose();
		}

		/// <summary>
		/// Mounts this instance for write access. Prior to that it's read-only
		/// </summary>
		public void MountWriteAccess()
		{
			if (_isMountedForWrite)
			{
				return;
			}

			int limit = 0;
			_isMountedForWrite = true;
			_expectedStateSize = (ulong)Core.SaveStateBinary().Length;

			if (_expectedStateSize > 0)
			{
				limit = _maxStates;
			}

			_states = new SortedList<int, StateManagerState>(limit);

			if (_expectedStateSize > int.MaxValue)
			{
				throw new InvalidOperationException();
			}

			NdbDatabase = new NDBDatabase(StatePath, Settings.DiskCapacitymb * 1024 * 1024, (int)_expectedStateSize);
		}

		public TasStateManagerSettings Settings { get; set; }

		/// <summary>
		/// Retrieves the savestate for the given frame,
		/// If this frame does not have a state currently, will return an empty array
		/// </summary>
		/// <returns>A savestate for the given frame or an empty array if there isn't one</returns>
		public KeyValuePair<int, byte[]> this[int frame]
		{
			get
			{
				if (frame == 0)
				{
					return new KeyValuePair<int, byte[]>(0, InitialState);
				}

				if (_states.ContainsKey(frame))
				{
					return new KeyValuePair<int, byte[]>(frame, _states[frame].State);
				}

				return new KeyValuePair<int, byte[]>(-1, new byte[0]);
			}
		}

		public byte[] InitialState
		{
			get
			{
				if (_movie.StartsFromSavestate)
				{
					return _movie.BinarySavestate;
				}

				return _states[0].State;
			}
		}

		/// <summary>
		/// Requests that the current emulator state be captured 
		/// Unless force is true, the state may or may not be captured depending on the logic employed by "greenzone" management
		/// </summary>
		public void Capture(bool force = false)
		{
			bool shouldCapture;

			int frame = Global.Emulator.Frame;
			if (_movie.StartsFromSavestate && frame == 0) // Never capture frame 0 on savestate anchored movies since we have it anyway
			{
				shouldCapture = false;
			}
			else if (force)
			{
				shouldCapture = force;
			}
			else if (frame == 0) // For now, long term, TasMovie should have a .StartState property, and a tasproj file for the start state in non-savestate anchored movies
			{
				shouldCapture = true;
			}
			else if (_movie.Markers.IsMarker(frame + 1))
			{
				shouldCapture = true; // Markers shoudl always get priority
			}
			else
			{
				shouldCapture = frame % StateFrequency == 0;
			}

			if (shouldCapture)
			{
				SetState(frame, (byte[])Core.SaveStateBinary().Clone(), skipRemoval: false);
			}
		}

		private void MoveStateToDisk(int index)
		{
			Used -= (ulong)_states[index].Length;
			_states[index].MoveToDisk();
		}

		private void MoveStateToMemory(int index)
		{
			_states[index].MoveToRAM();
			Used += (ulong)_states[index].Length;
		}

		internal void SetState(int frame, byte[] state, bool skipRemoval = true)
		{
			if (!skipRemoval) // skipRemoval: false only when capturing new states
			{
				LimitStateCount(); // Remove before adding so this state won't be removed.
			}

			if (_states.ContainsKey(frame))
			{
				_states[frame].State = state;
			}
			else
			{
				Used += (ulong)state.Length;
				_states.Add(frame, new StateManagerState(this, state, frame));
			}
		}

		public bool HasState(int frame)
		{
			if (_movie.StartsFromSavestate && frame == 0)
			{
				return true;
			}

			return _states.ContainsKey(frame);
		}

		/// <summary>
		/// Clears out all savestates after the given frame number
		/// </summary>
		public bool Invalidate(int frame)
		{
			bool anyInvalidated = false;

			if (Any())
			{
				if (frame == 0) // Never invalidate frame 0
				{
					frame = 1;
				}

				List<KeyValuePair<int, StateManagerState>> statesToRemove = _states.Where(s => s.Key >= frame).ToList();
				anyInvalidated = statesToRemove.Any();

				foreach (var state in statesToRemove)
				{
					RemoveState(state.Key);
				}

				CallInvalidateCallback(frame);
			}

			return anyInvalidated;
		}

		private bool StateIsMarker(int frame)
		{
			if (frame == -1)
			{
				return false;
			}

			return _movie.Markers.IsMarker(frame + 1);
		}

		private void RemoveState(int frame)
		{			
			int index = _states.IndexOfKey(frame);

			if (frame < 1 || index < 1)
			{
				return;
			}

			StateManagerState state = _states.ElementAt(index).Value;

			if (state.IsOnDisk)
			{
				state.Dispose();
			}
			else
			{
				Used -= (ulong)state.Length;
			}

			_states.RemoveAt(index);
		}

		/// <summary>
		/// Deletes/moves states to follow the state storage size limits.
		/// Used after changing the settings too.
		/// </summary>
		public void LimitStateCount()
		{
			if (Used + _expectedStateSize > Settings.Cap
				|| DiskUsed > (ulong)Settings.DiskCapacitymb * 1024 * 1024
				|| DateTime.Now.Ticks > _stateCleanupTime)
			{
				// rely on frames, because relying on indexes while changing the collection is ugly
				List<int> framesToClear = new List<int>();

				// we have 5 greenzone regions, the last one we do not touch
				int regionSize = _maxStates / 5;

				// drop states from previous regions, increasing the gap for each one, make sure to ignore state 0
				for (int gap = 2, last = _states.ToList().Count; gap <= 16; gap <<= 1)
				{
					// last egion index
					last -= regionSize;
					if (last < 1)
					{
						return;
					}

					// first region index
					int first = last - regionSize;
					if (first < 1)
					{
						first = 1;
					}

					// iterate through the region and record states' frames
					for (int i = last; i >= first; i--)
					{
						if ((i & (gap - 1)) > 0)
						{
							framesToClear.Add(_states.ElementAt(i).Key);
						}
					}
				}

				if (framesToClear.Any())
				{
					foreach (var frame in framesToClear)
					{
						RemoveState(frame);
					}
				}

				_stateCleanupTime = DateTime.Now.Ticks + _stateCleanupPeriod;
			}
		}

		private List<int> ExcludeStates()
		{
			List<int> ret = new List<int>();
			ulong saveUsed = Used + DiskUsed;

			// respect state gap no matter how small the resulting size will be
			// still leave marker states
			for (int i = 1; i < _states.Count; i++)
			{
				if (_movie.Markers.IsMarker(_states.ElementAt(i).Key + 1)
					|| _states.ElementAt(i).Key % _fileStateGap == 0)
				{
					continue;
				}

				ret.Add(i);

				if (_states.ElementAt(i).Value.IsOnDisk)
				{
					saveUsed -= _expectedStateSize;
				}
				else
				{
					saveUsed -= (ulong)_states.ElementAt(i).Value.Length;
				}
			}

			// if the size is still too big, exclude states form the beginning
			// still leave marker states
			int index = 0;
			while (saveUsed > (ulong)Settings.DiskSaveCapacitymb * 1024 * 1024)
			{
				do
				{
					index++;
					if (index >= _states.Count)
					{
						break;
					}
				}
				while (_movie.Markers.IsMarker(_states.ElementAt(index).Key + 1));

				if (index >= _states.Count)
				{
					break;
				}

				ret.Add(index);

				if (_states.ElementAt(index).Value.IsOnDisk)
				{
					saveUsed -= _expectedStateSize;
				}
				else
				{
					saveUsed -= (ulong)_states.ElementAt(index).Value.Length;
				}
			}

			// if there are enough markers to still be over the limit, remove marker frames
			index = 0;
			while (saveUsed > (ulong)Settings.DiskSaveCapacitymb * 1024 * 1024)
			{
				index++;
				if (!ret.Contains(index))
				{
					ret.Add(index);
				}

				if (_states.ElementAt(index).Value.IsOnDisk)
				{
					saveUsed -= _expectedStateSize;
				}
				else
				{
					saveUsed -= (ulong)_states.ElementAt(index).Value.Length;
				}
			}

			return ret;
		}

		public void ClearStateHistory()
		{
			if (_states.Any())
			{
				StateManagerState power = _states.Values.First(s => s.Frame == 0);
				_states.Clear();
				SetState(0, power.State);
				Used = (ulong)power.State.Length;
				NdbDatabase?.Clear();
			}
		}

		public void Save(BinaryWriter bw)
		{
			List<int> noSave = ExcludeStates();

			bw.Write(_states.Count - noSave.Count);
			for (int i = 0; i < _states.Count; i++)
			{
				if (noSave.Contains(i))
				{
					continue;
				}
				
				KeyValuePair<int, StateManagerState> kvp = _states.ElementAt(i);
				bw.Write(kvp.Key);
				bw.Write(kvp.Value.Length);
				bw.Write(kvp.Value.State);
			}
		}

		// Map:
		// 4 bytes - total savestate count
		// [Foreach state]
		// 4 bytes - frame
		// 4 bytes - length of savestate
		// 0 - n savestate
		public void Load(BinaryReader br)
		{
			_states.Clear();
			try
			{
				int nstates = br.ReadInt32();
				for (int i = 0; i < nstates; i++)
				{
					int frame = br.ReadInt32();
					int len = br.ReadInt32();
					byte[] data = br.ReadBytes(len);

					// whether we should allow state removal check here is an interesting question
					// nothing was edited yet, so it might make sense to show the project untouched first
					SetState(frame, data);
				}
			}
			catch (EndOfStreamException)
			{
			}
		}

		public KeyValuePair<int, byte[]> GetStateClosestToFrame(int frame)
		{
			var s = _states.LastOrDefault(state => state.Key < frame);

			return this[s.Key];
		}

		private ulong _used;
		private ulong Used
		{
			get
			{
				return _used;
			}

			set
			{
				// TODO: Shouldn't we throw an exception? Debug.Fail only runs in debug mode?
				if (value > 0xf000000000000000)
				{
					System.Diagnostics.Debug.Fail("ulong Used underfow!");
				}
				else
				{
					_used = value;
				}
			}
		}

		private ulong DiskUsed
		{
			get
			{
				if (NdbDatabase == null)
				{
					return 0;
				}

				return (ulong)NdbDatabase.Consumed;
			}
		}

		public int StateCount => _states.Count;

		public bool Any()
		{
			if (_movie.StartsFromSavestate)
			{
				return _states.Count > 0;
			}

			return _states.Count > 1;
		}

		public int LastKey
		{
			get
			{
				if (_states.Count == 0)
				{
					return 0;
				}

				return _states.Last().Key;
			}
		}

		public int LastEmulatedFrame
		{
			get
			{
				if (StateCount > 0)
				{
					return LastKey;
				}

				return 0;
			}
		}

		private int FindState(StateManagerState s)
		{
			if (!_states.ContainsValue(s))
			{
				return -1;
			}

			return s.Frame;
		}
	}
}