﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorruptCore
{
	public static class NetcoreCommands
	{
		public const string CORRUPTCORE = "CORRUPTCORE";
		public const string VANGUARD = "VANGUARD";
		public const string UI = "UI";
		public const string KILLSWITCH_PULSE = "KILLSWITCH_PULSE";


		public const string REMOTE_PUSHEMUSPEC = "REMOTE_PUSHEMUSPEC";
		public const string REMOTE_PUSHEMUSPECUPDATE = "REMOTE_PUSHEMUSPECUPDATE";
		public const string REMOTE_PUSHCORRUPTCORESPEC = "REMOTE_PUSHCORRUPTCORESPEC";
		public const string REMOTE_PUSHCORRUPTCORESPECUPDATE = "REMOTE_PUSHCORRUPTCORESPECUPDATE";
		public const string REMOTE_PUSHUISPEC = "REMOTE_PUSHUISPEC";
		public const string REMOTE_PUSHUISPECUPDATE = "REMOTE_PUSHUISPECUPDATE";
		public const string REMOTE_ALLSPECSSENT = "REMOTE_ALLSPECSSENT";
		
		public const string REMOTE_RENDER_STOP = "REMOTE_RENDER_STOP";
		public const string REMOTE_RENDER_START = "REMOTE_RENDER_START";

		public const string REMOTE_EVENT_DOMAINSUPDATED = "REMOTE_EVENT_DOMAINSUPDATED";
		public const string ASYNCBLAST = "ASYNCBLAST";
		public const string APPLYBLASTLAYER = "APPLYBLASTLAYER";
		public const string BLAST = "BLAST";
		public const string STASHKEY = "STASHKEY";
		public const string REMOTE_PUSHRTCSPEC = "REMOTE_PUSHRTCSPEC";
		public const string REMOTE_PUSHRTCSPECUPDATE = "REMOTE_PUSHRTCSPECUPDATE";
		public const string REMOTE_PUSHVMDPROTOS = "REMOTE_PUSHVMDS";
		public const string BLASTGENERATOR_BLAST = "BLASTGENERATOR_BLAST";
		public const string REMOTE_MERGECONFIG = "REMOTE_MERGECONFIG";
		public const string REMOTE_IMPORTKEYBINDS = "REMOTE_IMPORTKEYBINDS";
		public const string REMOTE_LOADSTATE = "REMOTE_LOADSTATE";
		public const string REMOTE_SAVESTATE = "REMOTE_SAVESTATE";

		public const string REMOTE_BACKUPKEY_REQUEST = "REMOTE_BACKUPKEY_REQUEST";
		public const string REMOTE_BACKUPKEY_STASH = "REMOTE_BACKUPKEY_STASH";
		public const string REMOTE_ISNORMALADVANCE = "REMOTE_ISNORMALADVANCE"; 

		public const string REMOTE_DOMAIN_PEEKBYTE = "REMOTE_DOMAIN_PEEKBYTE";
		public const string REMOTE_DOMAIN_POKEBYTE = "REMOTE_DOMAIN_POKEBYTE";
		public const string REMOTE_DOMAIN_GETDOMAINS = "REMOTE_DOMAIN_GETDOMAINS";
		public const string REMOTE_DOMAIN_VMD_ADD = "REMOTE_DOMAIN_VMD_ADD";
		public const string REMOTE_DOMAIN_VMD_REMOVE = "REMOTE_DOMAIN_VMD_REMOVE";
		public const string REMOTE_DOMAIN_ACTIVETABLE_MAKEDUMP = "REMOTE_DOMAIN_ACTIVETABLE_MAKEDUMP";
		public const string REMOTE_KEY_PUSHSAVESTATEDICO = "REMOTE_KEY_PUSHSAVESTATEDICO";
		public const string REMOTE_KEY_GETRAWBLASTLAYER = "REMOTE_KEY_GETRAWBLASTLAYER";

		public const string REMOTE_SET_APPLYUNCORRUPTBL = "REMOTE_SET_APPLYUNCORRUPTBL";
		public const string REMOTE_SET_APPLYCORRUPTBL = "REMOTE_SET_APPLYCORRUPTBL";

		public const string REMOTE_SET_STEPACTIONS_CLEARALLBLASTUNITS = "REMOTE_SET_STEPACTIONS_CLEARALLBLASTUNITS";
		public const string REMOTE_SET_STEPACTIONS_REMOVEEXCESSINFINITEUNITS = "REMOTE_SET_STEPACTIONS_REMOVEEXCESSINFINITEUNITS";
		public const string REMOTE_EVENT_LOADGAMEDONE_NEWGAME = "REMOTE_EVENT_LOADGAMEDONE_NEWGAME";
		public const string REMOTE_EVENT_LOADGAMEDONE_SAMEGAME = "REMOTE_EVENT_LOADGAMEDONE_SAMEGAME";
		public const string REMOTE_EVENT_CLOSEEMULATOR = "REMOTE_EVENT_CLOSEEMULATOR";

		public const string GENERATEBLASTLAYER = "GENERATEBLASTLAYER";
		public const string SAVESAVESTATE = "SAVESAVESTATE";
		public const string LOADSAVESTATE = "LOADSAVESTATE";
		public const string REMOTE_LOADROM = "REMOTE_LOADROM";


		public const string ERROR_DISABLE_AUTOCORRUPT = "ERROR_DISABLE_AUTOCORRUPT";



		public const string REMOTE_KEY_SETSYNCSETTINGS = "REMOTE_KEY_SETSYNCSETTINGS";
		public const string REMOTE_KEY_SETSYSTEMCORE = "REMOTE_KEY_SETSYSTEMCORE";

		public const string BIZHAWK_OPEN_HEXEDITOR_ADDRESS = "BIZHAWK_OPEN_HEXEDITOR_ADDRESS";
		public const string REMOTE_EVENT_BIZHAWK_MAINFORM_CLOSE = "REMOTE_EVENT_BIZHAWK_MAINFORM_CLOSE";
		public const string REMOTE_EVENT_SAVEBIZHAWKCONFIG = "REMOTE_EVENT_SAVEBIZHAWKCONFIG";
		public const string REMOTE_EVENT_BIZHAWKSTARTED = "REMOTE_EVENT_BIZHAWKSTARTED";

		public const string REMOTE_RESTOREBIZHAWKCONFIG = "REMOTE_RESTOREBIZHAWKCONFIG";
		


		public const string REMOTE_HOTKEY_MANUALBLAST = "REMOTE_HOTKEY_MANUALBLAST";
		public const string REMOTE_HOTKEY_AUTOCORRUPTTOGGLE = "REMOTE_HOTKEY_AUTOCORRUPTTOGGLE";
		public const string REMOTE_HOTKEY_ERRORDELAYDECREASE = "REMOTE_HOTKEY_ERRORDELAYDECREASE";
		public const string REMOTE_HOTKEY_ERRORDELAYINCREASE = "REMOTE_HOTKEY_ERRORDELAYINCREASE";
		public const string REMOTE_HOTKEY_INTENSITYDECREASE = "REMOTE_HOTKEY_INTENSITYDECREASE";
		public const string REMOTE_HOTKEY_INTENSITYINCREASE = "REMOTE_HOTKEY_INTENSITYINCREASE";
		public const string REMOTE_HOTKEY_GHLOADCORRUPT = "REMOTE_HOTKEY_GHLOADCORRUPT";
		public const string REMOTE_HOTKEY_GHCORRUPT = "REMOTE_HOTKEY_GHCORRUPT";
		public const string REMOTE_HOTKEY_GHLOAD = "REMOTE_HOTKEY_GHLOAD";
		public const string REMOTE_HOTKEY_GHSAVE = "REMOTE_HOTKEY_GHSAVE";
		public const string REMOTE_HOTKEY_GHSTASHTOSTOCKPILE = "REMOTE_HOTKEY_GHSTASHTOSTOCKPILE";
		public const string REMOTE_HOTKEY_BLASTRAWSTASH = "REMOTE_HOTKEY_BLASTRAWSTASH";
		public const string REMOTE_HOTKEY_SENDRAWSTASH = "REMOTE_HOTKEY_SENDRAWSTASH";
		public const string REMOTE_HOTKEY_BLASTLAYERTOGGLE = "REMOTE_HOTKEY_BLASTLAYERTOGGLE";
		public const string REMOTE_HOTKEY_BLASTLAYERREBLAST = "REMOTE_HOTKEY_BLASTLAYERREBLAST";
	}
}