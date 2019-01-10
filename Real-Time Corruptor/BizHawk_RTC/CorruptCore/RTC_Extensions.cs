﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Xml.Serialization;
using Ceras;
using Newtonsoft.Json;

namespace RTC
{
	public static class RTC_Extensions
	{

		public static void DirectoryRequired(string path)
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}
		public static void DirectoryRequired(string[] paths)
		{
			foreach (string path in paths)
				DirectoryRequired(path);
		}

		#region ARRAY EXTENSIONS

		public static T[] SubArray<T>(this T[] data, long index, long length)
		{
			T[] result = new T[length];

			if (data == null)
				return null;

			Array.Copy(data, index, result, 0, length);
			return result;
		}

		public static T[] FlipWords<T>(this T[] data, int wordSize)
		{
			//2 : 16-bit
			//4 : 32-bit
			//8 : 64-bit

			T[] result = new T[data.Length];

			for (int i = 0; i < data.Length; i++)
			{
				int wordPos = i % wordSize;
				int wordAddress = i - wordPos;
				int newPos = wordAddress + (wordSize - (wordPos + 1));

				result[newPos] = data[i];
			}

			return result;
		}

		#endregion ARRAY EXTENSIONS

		#region STRING EXTENSIONS

		public static string ToBase64(this string str)
		{
			var bytes = Encoding.UTF8.GetBytes(str);
			return Convert.ToBase64String(bytes);
		}

		public static string FromBase64(this string base64)
		{
			var data = Convert.FromBase64String(base64);
			return Encoding.UTF8.GetString(data);
		}
		/// <summary>
		/// Gets you a byte array representing the characters in a string.
		/// THIS DOES NOT CONVERT A STRING TO A BYTE ARRAY CONTAINING THE SAME CHARACTERS
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static byte[] GetBytes(this string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		/// <summary>
		/// Gets you a byte array from the CONTENTS of a string
		/// </summary>
		/// <param name="hex"></param>
		/// <returns></returns>
		public static byte[] StringToByteArray(string hex)
		{
			//If it's odd, prepend a 0
			if (hex.Length % 2 == 1)
			{
				string temp = "0" + hex;
				return Enumerable.Range(0, temp.Length)
				.Where(x => x % 2 == 0)
				.Select(x => Convert.ToByte(temp.Substring(x, 2), 16))
				.ToArray();
			}

			return Enumerable.Range(0, hex.Length)
				.Where(x => x % 2 == 0)
				.Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
				.ToArray();
		}

		/// <summary>
		/// Gets you a byte array from the CONTENTS of a string 0 padded on the left to a specific length
		/// </summary>
		/// <param name="hex"></param>
		/// <returns></returns>
		public static byte[] StringToByteArrayPadLeft(string hex, int precision)
		{
			var bytes = new byte[precision];
			if (hex == null)
				return null;
			string temp = hex.PadLeft(precision*2,'0'); //*2 since a byte is two characters

			int j = 0;
			for (var i = 0; i < precision*2; i+=2)
			{
				try
				{
					bytes[j] = (byte) Convert.ToUInt32(temp.Substring(i, 2), 16);
				}
				catch (FormatException e)
				{
					return null;
				}
				
				j++;
			}
			return bytes;
		}

		public static string MakeSafeFilename(this string filename, char replaceChar)
		{
			foreach (char c in Path.GetInvalidFileNameChars())
			{
				filename = filename.Replace(c, replaceChar);
			}
			return filename;
		}

		#endregion STRING EXTENSIONS

		#region BYTE ARRAY EXTENSIONS

		//Thanks to JamieSee https://stackoverflow.com/questions/8440938/c-sharp-left-shift-an-entire-byte-array
		/// <summary>
		/// Rotates the bits in an array of bytes to the left.
		/// </summary>
		/// <param name="bytes">The byte array to rotate.</param>
		public static void RotateLeft(byte[] bytes)
		{
			bool carryFlag = ShiftLeft(bytes);

			if (carryFlag == true)
			{
				bytes[bytes.Length - 1] = (byte)(bytes[bytes.Length - 1] | 0x01);
			}
		}

		/// <summary>
		/// Rotates the bits in an array of bytes to the right.
		/// </summary>
		/// <param name="bytes">The byte array to rotate.</param>
		public static void RotateRight(byte[] bytes)
		{
			bool carryFlag = ShiftRight(bytes);

			if (carryFlag == true)
			{
				bytes[0] = (byte)(bytes[0] | 0x80);
			}
		}

		/// <summary>
		/// Shifts the bits in an array of bytes to the left.
		/// </summary>
		/// <param name="bytes">The byte array to shift.</param>
		public static bool ShiftLeft(byte[] bytes)
		{
			bool leftMostCarryFlag = false;

			// Iterate through the elements of the array from left to right.
			for (int index = 0; index < bytes.Length; index++)
			{
				// If the leftmost bit of the current byte is 1 then we have a carry.
				bool carryFlag = (bytes[index] & 0x80) > 0;

				if (index > 0)
				{
					if (carryFlag == true)
					{
						// Apply the carry to the rightmost bit of the current bytes neighbor to the left.
						bytes[index - 1] = (byte)(bytes[index - 1] | 0x01);
					}
				}
				else
				{
					leftMostCarryFlag = carryFlag;
				}

				bytes[index] = (byte)(bytes[index] << 1);
			}

			return leftMostCarryFlag;
		}

		/// <summary>
		/// Shifts the bits in an array of bytes to the right.
		/// </summary>
		/// <param name="bytes">The byte array to shift.</param>
		public static bool ShiftRight(byte[] bytes)
		{
			bool rightMostCarryFlag = false;
			int rightEnd = bytes.Length - 1;

			// Iterate through the elements of the array right to left.
			for (int index = rightEnd; index >= 0; index--)
			{
				// If the rightmost bit of the current byte is 1 then we have a carry.
				bool carryFlag = (bytes[index] & 0x01) > 0;

				if (index < rightEnd)
				{
					if (carryFlag == true)
					{
						// Apply the carry to the leftmost bit of the current bytes neighbor to the right.
						bytes[index + 1] = (byte)(bytes[index + 1] | 0x80);
					}
				}
				else
				{
					rightMostCarryFlag = carryFlag;
				}

				bytes[index] = (byte)(bytes[index] >> 1);
			}

			return rightMostCarryFlag;
		}

		public static ulong GetNumericMaxValue(byte[] Value)
		{
			switch (Value.Length)
			{
				case 1:
					return byte.MaxValue;
				case 2:
					return UInt16.MaxValue;
				case 4:
					return UInt32.MaxValue;
				case 8:
					return UInt64.MaxValue;
			}

			return 0;
		}

		public static decimal GetDecimalValue(byte[] value, bool needsBytesFlipped)
		{
			byte[] _value = (byte[])value.Clone();

			if (needsBytesFlipped)
				Array.Reverse(_value);

			switch (value.Length)
			{
				case 1:
					return (int)_value[0];
				case 2:
					return BitConverter.ToUInt16(_value, 0);
				case 4:
					return BitConverter.ToUInt32(_value, 0);
				case 8:
					return BitConverter.ToUInt64(_value, 0);
			}

			return 0;
		}

		public static byte[] AddValueToByteArrayUnchecked(byte[] originalValue, BigInteger addValue, bool isInputBigEndian)
		{
			byte[] value = (byte[])originalValue.Clone();

			if (isInputBigEndian)
				Array.Reverse(value);

			bool isAdd = addValue >= 0;
			BigInteger bigintAddValueAbs = BigInteger.Abs(addValue);

			switch (value.Length)
			{
				case 1:
					byte byteValue = value[0];
					byte addByteValue = (bigintAddValueAbs > byte.MaxValue ? byte.MaxValue : (byte)bigintAddValueAbs);

					if (isAdd)
						unchecked { byteValue += addByteValue; }
					else
						unchecked { byteValue -= addByteValue; }

					return new byte[] { byteValue };

				case 2:
					{
						UInt16 int16Value = BitConverter.ToUInt16(value, 0);
						UInt16 addInt16Value = (bigintAddValueAbs > UInt16.MaxValue ? UInt16.MaxValue : (ushort)bigintAddValueAbs);

						if (isAdd)
							unchecked { int16Value += addInt16Value; }
						else
							unchecked { int16Value -= addInt16Value; }

						byte[] newInt16Array = BitConverter.GetBytes(int16Value);

						if (isInputBigEndian)
							Array.Reverse(newInt16Array);

						return newInt16Array;
					}
				case 4:
					{
						UInt32 int32Value = BitConverter.ToUInt32(value, 0);
						UInt32 addInt32Value = (bigintAddValueAbs > UInt32.MaxValue ? UInt32.MaxValue : (uint)bigintAddValueAbs);

						if (isAdd)
							unchecked { int32Value += addInt32Value; }
						else
							unchecked { int32Value -= addInt32Value; }

						byte[] newInt32Array = BitConverter.GetBytes(int32Value);

						if (isInputBigEndian)
							Array.Reverse(newInt32Array);

						return newInt32Array;
					}
				case 8:
					{
						UInt64 int64Value = BitConverter.ToUInt64(value, 0);
						UInt64 addInt64Value = (bigintAddValueAbs > UInt64.MaxValue ? UInt64.MaxValue : (ulong)bigintAddValueAbs);

						if (isAdd)
							unchecked { int64Value += addInt64Value; }
						else
							unchecked { int64Value -= addInt64Value; }

						byte[] newInt64Array = BitConverter.GetBytes(int64Value);

						if (isInputBigEndian)
							Array.Reverse(newInt64Array);

						return newInt64Array;
					}
				default:
				{
					//Gets us a positive value
					byte[] temp = new byte[value.Length + 1];
					value.CopyTo(temp, 0);
					BigInteger bigIntValue = new BigInteger(temp);

					if (isAdd)
						bigIntValue += addValue; 
					else
						bigIntValue -= addValue;

					//Calculate the max value you can store in this many bits 
					BigInteger maxValue = BigInteger.Pow(2, value.Length) - 1;

					if (bigIntValue > maxValue)
						bigIntValue = bigIntValue % maxValue - 1; //Works fine for positive
					else if (bigIntValue < 0)
						bigIntValue = Mod(maxValue, bigIntValue); //% means remainder in c#

					byte[] added = bigIntValue.ToByteArray();
					byte[] outArray = new byte[value.Length];
					added.CopyTo(outArray, outArray.Length - added.Length);


					if (isInputBigEndian)
						Array.Reverse(outArray);

					return outArray;
				}
			}
			return null;
		}

		private static decimal Mod(decimal x, long m)
		{
			return (x % m + m) % m;
		}

		private static BigInteger Mod(BigInteger x, BigInteger m)
		{
			return (x % m + m) % m;
		}

		public static byte[] GetByteArrayValue(int precision, decimal newValue, bool needsBytesFlipped = false)
		{
			switch (precision)
			{
				case 1:
					return new byte[] { (byte)newValue };
				case 2:
					{
						byte[] value = BitConverter.GetBytes(Convert.ToUInt16(newValue));
						if (needsBytesFlipped)
							Array.Reverse(value);
						return value;
					}
				case 4:
					{
						byte[] value = BitConverter.GetBytes(Convert.ToUInt32(newValue));
						if (needsBytesFlipped)
							Array.Reverse(value);
						return value;
					}
			}

			return null;
		}

		public static byte[] GetByteArrayValue(int precision, long newValue, bool needsBytesFlipped = false)
		{
			switch (precision)
			{
				case 1:
					return new byte[] { (byte)newValue };
				case 2:
					{
						byte[] value = BitConverter.GetBytes(Convert.ToUInt16(newValue));
						if (needsBytesFlipped)
							Array.Reverse(value);
						return value;
					}
				case 4:
					{
						byte[] value = BitConverter.GetBytes(Convert.ToUInt32(newValue));
						if (needsBytesFlipped)
							Array.Reverse(value);
						return value;
					}
			}

			return null;
		}

		public static byte[] FlipBytes(this byte[] array)
		{
			byte[] arrayClone = (byte[])array.Clone();

			for (int i = 0; i < arrayClone.Length; i++)
				array[i] = arrayClone[(arrayClone.Length - 1) - i];
			return array;
		}

		public static byte[] PadLeft(this byte[] input, int length)
		{
			var newArray = new byte[length];

			var startAt = newArray.Length - input.Length;
			Buffer.BlockCopy(input, 0, newArray, startAt, input.Length);
			return newArray;
		}


		/// <summary>
		/// Converts bytes to an uppercase string of hex numbers in upper case without any spacing or anything
		/// </summary>
		public static string BytesToHexString(byte[] bytes)
		{
			var sb = new StringBuilder();
			foreach (var b in bytes)
			{
				sb.AppendFormat("{0:X2}", b);
			}

			return sb.ToString();
		}

		#endregion BYTE ARRAY EXTENSIONS

		#region BITARRAY EXTENSIONS

		public static byte[] ToByteArray(this BitArray bits)
		{
			int numBytes = bits.Count / 8;
			if (bits.Count % 8 != 0) numBytes++;

			byte[] bytes = new byte[numBytes];
			int byteIndex = 0, bitIndex = 0;

			for (int i = 0; i < bits.Count; i++)
			{
				if (bits[i])
					bytes[byteIndex] |= (byte)(1 << (7 - bitIndex));

				bitIndex++;
				if (bitIndex == 8)
				{
					bitIndex = 0;
					byteIndex++;
				}
			}

			return bytes;
		}

		#endregion BITARRAY EXTENSIONS

		#region COLOR EXTENSIONS

		/// <summary>
		/// Creates color with corrected brightness.
		/// </summary>
		/// <param name="color">Color to correct.</param>
		/// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1.
		/// Negative values produce darker colors.</param>
		/// <returns>
		/// Corrected <see cref="Color"/> structure.
		/// </returns>
		public static Color ChangeColorBrightness(this Color color, float correctionFactor)
		{
			float red = (float)color.R;
			float green = (float)color.G;
			float blue = (float)color.B;

			if (correctionFactor < 0)
			{
				correctionFactor = 1 + correctionFactor;
				red *= correctionFactor;
				green *= correctionFactor;
				blue *= correctionFactor;
			}
			else
			{
				red = (255 - red) * correctionFactor + red;
				green = (255 - green) * correctionFactor + green;
				blue = (255 - blue) * correctionFactor + blue;
			}

			return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
		}

		#endregion COLOR EXTENSIONS

		#region CONTROL EXTENSIONS

		public static List<Control> getControlsWithTag(this Control.ControlCollection controls)
		{
			List<Control> allControls = new List<Control>();

			foreach (Control c in controls)
			{
				if (c.Tag != null)
					allControls.Add(c);

				if (c.HasChildren)
					allControls.AddRange(c.Controls.getControlsWithTag()); //Recursively check all children controls as well; ie groupboxes or tabpages
			}

			return allControls;
		}


		#endregion CONTROL EXTENSIONS

		#region PATH EXTENSIONS
		//why not just use 
		//System.IO.Path.GetFilename(String)
		//System.IO.Path.GetFileNameWithoutExtension(String)
		//System.IO.Path.GetDirectoryName(String) + "\"
		public static string getShortFilenameFromPath(string longFilenamePath)
		{
			// >>> Will contain the character \ at the end

			//returns the filename from the full path
			if (longFilenamePath.Contains(Path.DirectorySeparatorChar))
				return longFilenamePath.Substring(longFilenamePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
			return longFilenamePath;
		}

		public static string removeFileExtension(string filename)
		{
			// filename.wav -> filename

			if (filename.Contains("."))
				return filename.Substring(0, filename.LastIndexOf("."));
			return filename;
		}

		public static string getLongDirectoryFromPath(string longFilenamePath)
		{
			// >>> Will contain the character \ at the end

			//returns the filename from the full path
			if (longFilenamePath.Contains(Path.DirectorySeparatorChar))
				return longFilenamePath.Substring(0, longFilenamePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
			return longFilenamePath;
		}

		#endregion PATH EXTENSIONS

		#region STREAM EXTENSIONS
		//Thanks! https://stackoverflow.com/a/13021983
		public static long CopyBytes(long bytesRequired, Stream inStream, Stream outStream)
		{
			long readSoFar = 0L;
			var buffer = new byte[64 * 1024];
			do
			{
				var toRead = Math.Min(bytesRequired - readSoFar, buffer.Length);
				var readNow = inStream.Read(buffer, 0, (int)toRead);
				if (readNow == 0)
					break; // End of stream
				outStream.Write(buffer, 0, readNow);
				readSoFar += readNow;
			} while (readSoFar < bytesRequired);
			return readSoFar;
		}
		#endregion

		#region LIST EXTENSIONS
		//https://stackoverflow.com/a/29119974
		public static System.Data.DataTable ToDataTable<T>(this List<T> items)
		{
			var tb = new System.Data.DataTable(typeof(T).Name);

			PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var prop in props)
			{
				if (prop.PropertyType.IsEnum)
				{
					tb.Columns.Add(prop.Name, typeof(string));
				}
				else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					tb.Columns.Add(prop.Name, prop.PropertyType.GetGenericArguments()[0]);
				}
				else
					tb.Columns.Add(prop.Name, prop.PropertyType);
			}

			foreach (var item in items)
			{
				var values = new object[props.Length];
				for (var i = 0; i < props.Length; i++)
				{
					if (props[i].PropertyType.IsEnum)
					{
						object val = props[i].GetValue(item, null);
						values[i] = Enum.GetName(props[i].PropertyType, val);
					}
					else
					{
						values[i] = props[i].GetValue(item, null);
					}
				}

				tb.Rows.Add(values);
			}

			return tb;
		}
		#endregion
		#region BINDINGLIST EXTENSIONS

		public static BindingList<T> AddRange<T>(this BindingList<T> input, IEnumerable<T> collection)
		{
			foreach (T item in collection)
				input.Add(item);
			return input;
		}

		public static BindingList<T> ToBindingList<T>(this IEnumerable<T> collection)
		{
			return new BindingList<T>(collection.ToList());
		}
		#endregion
		
		/// <summary>
		/// Force the value to be strictly between min and max (both exclued)
		/// </summary>
		/// <typeparam name="T">Anything that implements <see cref="IComparable{T}"/></typeparam>
		/// <param name="val">Value that will be clamped</param>
		/// <param name="min">Minimum allowed</param>
		/// <param name="max">Maximum allowed</param>
		/// <returns>The value if strictly between min and max; otherwise min (or max depending of what is passed)</returns>
		public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
		{
			if (val.CompareTo(min) < 0)
			{
				return min;
			}

			if (val.CompareTo(max) > 0)
			{
				return max;
			}

			return val;
		}
		#region Image Extensions

		public static byte[] ImageToByteArray(System.Drawing.Image imageIn, System.Drawing.Imaging.ImageFormat imageFormat)
		{
			MemoryStream ms = new MemoryStream();
			imageIn.Save(ms, imageFormat);
			return ms.ToArray();
		}

		public static Image ByteArrayToImage(byte[] byteArrayIn)
		{
			MemoryStream ms = new MemoryStream(byteArrayIn);
			Image returnImage = Image.FromStream(ms);
			return returnImage;
		}

		#endregion
		/**
		 * A HashSet byte which uses a custom comparator for byte array value comparison
		 * This exists because Ceras can't handle read-only properties.
		 */
		public class HashSetByteArrayComparator : HashSet<byte[]>
		{
			public HashSetByteArrayComparator(IEnumerable<byte[]> collection) : base(collection, new ByteArrayComparer())
			{

			}

			public HashSetByteArrayComparator() : base(new ByteArrayComparer())
			{

			}
		}


		[Serializable]
		[Ceras.MemberConfig(TargetMember.All)]
		public class ByteArrayComparer : IEqualityComparer<byte[]>
		{
			public bool Equals(byte[] a, byte[] b)
			{
				if (a.Length != b.Length) return false;
				for (int i = 0; i < a.Length; i++)
					if (a[i] != b[i]) return false;
				return true;
			}
			public int GetHashCode(byte[] a)
			{
				uint b = 0;
				for (int i = 0; i < a.Length; i++)
					b = ((b << 23) | (b >> 9)) ^ a[i];
				return unchecked((int)b);
			}

			public ByteArrayComparer()
			{

			}
		}
	}

	public interface ISF<T>
	{
		//Interface for Singleton Form
		T Me();
		T NewMe();
	}

	public interface IColorable
	{

	}

	//Static singleton manager
	//Call or create a singleton using class type
	public static class S
	{
		static Dictionary<Type, object> instances = new Dictionary<Type, object>();

		public static bool ISNULL<T>()
		{
			Type typ = typeof(T);
			return instances.ContainsKey(typ);
		}

		public static T GET<T>()
		{
			Type typ = typeof(T);

			if (!instances.ContainsKey(typ))
				instances[typ] = Activator.CreateInstance(typ);

			return (T)instances[typ];
		}

		public static object GET(Type typ)
		{
			//Type typ = typeof(T);

			if (!instances.ContainsKey(typ))
				instances[typ] = Activator.CreateInstance(typ);

			return instances[typ];
		}

		public static void SET<T>(T newTyp)
		{
			Type typ = typeof(T);

			if (newTyp is Nullable && newTyp == null)
				instances.Remove(typ);
			else
				instances[typ] = newTyp;
		}
	}



	// Used code from this https://github.com/wasabii/Cogito/blob/master/Cogito.Core/RandomExtensions.cs
	// MIT Licensed. thank you very much.
	internal static class RandomExtensions
	{
		public static long RandomLong(this Random rnd)
		{
			byte[] buffer = new byte[8];
			rnd.NextBytes(buffer);
			return BitConverter.ToInt64(buffer, 0);
		}

		public static long RandomLong(this Random rnd, long min, long max)
		{
			EnsureMinLEQMax(ref min, ref max);
			long numbersInRange = unchecked(max - min + 1);
			if (numbersInRange < 0)
				throw new ArgumentException("Size of range between min and max must be less than or equal to Int64.MaxValue");

			long randomOffset = RandomLong(rnd);
			if (IsModuloBiased(randomOffset, numbersInRange))
				return RandomLong(rnd, min, max); // Try again
			return min + PositiveModuloOrZero(randomOffset, numbersInRange);
		}

		public static long RandomLong(this Random rnd, long max)
		{
			return rnd.RandomLong(0, max);
		}

		private static bool IsModuloBiased(long randomOffset, long numbersInRange)
		{
			long greatestCompleteRange = numbersInRange * (long.MaxValue / numbersInRange);
			return randomOffset > greatestCompleteRange;
		}

		private static long PositiveModuloOrZero(long dividend, long divisor)
		{
			Math.DivRem(dividend, divisor, out long mod);
			if (mod < 0)
				mod += divisor;
			return mod;
		}

		private static void EnsureMinLEQMax(ref long min, ref long max)
		{
			if (min <= max)
				return;
			long temp = min;
			min = max;
			max = temp;
		}
	}

	/// <summary>
	/// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
	/// Provides a method for performing a deep copy of an object.
	/// Binary Serialization is used to perform the copy.
	/// </summary>
	public static class ObjectCopier
	{
		/// <summary>
		/// Perform a deep Copy of the object.
		/// </summary>
		/// <typeparam name="T">The type of object being copied.</typeparam>
		/// <param name="source">The object instance to copy.</param>
		/// <returns>The copied object.</returns>
		public static T Clone<T>(T source)
		{
			if (!typeof(T).IsSerializable)
			{
				throw new ArgumentException("The type must be serializable.", "source");
			}

			// Don't serialize a null object, simply return the default for that object
			if (Object.ReferenceEquals(source, null))
			{
				return default(T);
			}

			IFormatter formatter = new BinaryFormatter();
			Stream stream = new MemoryStream();
			using (stream)
			{
				formatter.Serialize(stream, source);
				stream.Seek(0, SeekOrigin.Begin);
				return (T)formatter.Deserialize(stream);
			}
		}
	}
	/// <summary>
	/// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
	/// Provides a method for performing a deep copy of an object.
	/// Binary Serialization is used to perform the copy.
	/// </summary>
	public static class ObjectCopierCeras
	{
		/// <summary>
		/// Perform a deep Copy of the object.
		/// </summary>
		/// <typeparam name="T">The type of object being copied.</typeparam>
		/// <param name="source">The object instance to copy.</param>
		/// <returns>The copied object.</returns>
		public static T Clone<T>(T source)
		{
			if (!typeof(T).IsSerializable)
			{
				throw new ArgumentException("The type must be serializable.", "source");
			}

			// Don't serialize a null object, simply return the default for that object
			if (Object.ReferenceEquals(source, null))
			{
				return default(T);
			}


			var config = new SerializerConfig();
			config.DefaultTargets = TargetMember.All;
			config.ShouldSerializeMember = m => SerializationOverride.ForceInclude;
			var s = new CerasSerializer(config);


			return s.Deserialize<T>(s.Serialize(source));
		}
	}


}
[Serializable]
	public class SerializableDico<T, T2> : ISerializable
	{
		[XmlIgnore]
		public Dictionary<T, T2> innerDico { get; set; } = new Dictionary<T, T2>();

		public List<T> keys = new List<T>();
		public List<T2> values = new List<T2>();

		public bool ContainsKey(T key)
		{
			return innerDico.ContainsKey(key);
		}

		public SerializableDico()
		{

		}
		protected SerializableDico(SerializationInfo info, StreamingContext context)
		{

		}

		public T2 this[T key]
		{
			get
			{

				if (innerDico.ContainsKey(key))
					return innerDico[key];
				return default(T2);
			}

			set
			{
				if (value == null && innerDico.ContainsKey(key))// A null value means a key removal in the Full Spec
					innerDico.Remove(key);
				else
					innerDico[key] = value;
			}
		}

		public void Reset() => innerDico.Clear();

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Keys", keys);
			info.AddValue("Values", values);
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			keys.Clear();
			keys.AddRange(innerDico.Keys);

			values.Clear();
			values.AddRange(innerDico.Values);
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			for (int i = 0; i < keys.Count; i++)
				innerDico[keys[i]] = values[i];
		}
	}

	public class WebClientTimeout : WebClient
	{
		public int Timeout = 5000;
		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest wr = base.GetWebRequest(address);
			wr.Timeout = Timeout;
			return wr;
		}
	}

	public static class JsonHelper
	{
		public static void Serialize(object value, Stream s, Formatting f = Formatting.None)
		{
			using (StreamWriter writer = new StreamWriter(s))
			using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
			{
				JsonSerializer ser = new JsonSerializer
				{
					Formatting = f
				};
				ser.Serialize(jsonWriter, value);
				jsonWriter.Flush();
			}
		}

		public static T Deserialize<T>(Stream s)
		{
			using (StreamReader reader = new StreamReader(s))
			using (JsonTextReader jsonReader = new JsonTextReader(reader))
			{
				JsonSerializer ser = new JsonSerializer();
				return ser.Deserialize<T>(jsonReader);
			}
		}
	}

