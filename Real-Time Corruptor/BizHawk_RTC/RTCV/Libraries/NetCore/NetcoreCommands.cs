﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTCV.NetCore
{
	public static class NetcoreCommands
	{
		public const string CORRUPTCORE = "CORRUPTCORE";
		public const string VANGUARD = "VANGUARD";
		public const string UI = "UI";
		public const string KILLSWITCH_PULSE = "KILLSWITCH_PULSE";


		public const string REMOTE_PUSHVANGUARDSPEC = "REMOTE_PUSHVANGUARDSPEC";
		public const string REMOTE_PUSHVANGUARDSPECUPDATE = "REMOTE_PUSHVANGUARDSPECUPDATE";
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
		public const string APPLYCACHEDBLASTLAYER = "APPLYCACHEDBLASTLAYER";
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

		public const string REMOTE_CLEARSTEPBLASTUNITS = "REMOTE_CLEARSTEPBLASTUNITS";
		public const string REMOTE_REMOVEEXCESSINFINITESTEPUNITS = "REMOTE_REMOVEEXCESSINFINITESTEPUNITS";
		public const string REMOTE_EVENT_LOADGAMEDONE_NEWGAME = "REMOTE_EVENT_LOADGAMEDONE_NEWGAME";
		public const string REMOTE_EVENT_LOADGAMEDONE_SAMEGAME = "REMOTE_EVENT_LOADGAMEDONE_SAMEGAME";
		public const string REMOTE_EVENT_CLOSEEMULATOR = "REMOTE_EVENT_CLOSEEMULATOR";

		public const string GENERATEBLASTLAYER = "GENERATEBLASTLAYER";
		public const string SAVESAVESTATE = "SAVESAVESTATE";
		public const string LOADSAVESTATE = "LOADSAVESTATE";
		public const string REMOTE_LOADROM = "REMOTE_LOADROM";
		public const string REMOTE_CLOSEGAME = "REMOTE_CLOSEGAME";

		public const string REMOTE_BLASTTOOLS_GETAPPLIEDBACKUPLAYER = "REMOTE_BLASTTOOLS_GETAPPLIEDBACKUPLAYER";


		public const string ERROR_DISABLE_AUTOCORRUPT = "ERROR_DISABLE_AUTOCORRUPT";



		public const string REMOTE_KEY_SETSYNCSETTINGS = "REMOTE_KEY_SETSYNCSETTINGS";
		public const string REMOTE_KEY_SETSYSTEMCORE = "REMOTE_KEY_SETSYSTEMCORE";

		public const string EMU_OPEN_HEXEDITOR_ADDRESS = "EMU_OPEN_HEXEDITOR_ADDRESS";
		public const string REMOTE_EVENT_EMU_MAINFORM_CLOSE = "REMOTE_EVENT_EMU_MAINFORM_CLOSE";
		public const string REMOTE_EVENT_EMUSTARTED = "REMOTE_EVENT_EMUSTARTED";

		public const string REMOTE_RESTOREBIZHAWKCONFIG = "REMOTE_RESTOREBIZHAWKCONFIG";
		public const string REMOTE_EVENT_SAVEBIZHAWKCONFIG = "REMOTE_EVENT_SAVEBIZHAWKCONFIG";

		public const string RTC_INFOCUS = "RTC_INFOCUS";

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
		public const string REMOTE_HOTKEY_GAMEPROTECTIONBACK = "REMOTE_HOTKEY_GAMEPROTECTIONBACK";
		public const string REMOTE_HOTKEY_GAMEPROTECTIONNOW = "REMOTE_HOTKEY_GAMEPROTECTIONNOW";
		public const string REMOTE_HOTKEY_BEINVERTDISABLED = "REMOTE_HOTKEY_BEINVERTDISABLED";
		public const string REMOTE_HOTKEY_BEREMOVEDISABLED = "REMOTE_HOTKEY_BEREMOVEDISABLED";
		public const string REMOTE_HOTKEY_BEDISABLE50 = "REMOTE_HOTKEY_BEDISABLE50";
		public const string REMOTE_HOTKEY_BESHIFTUP = "REMOTE_HOTKEY_BESHIFTUP";
		public const string REMOTE_HOTKEY_BESHIFTDOWN = "REMOTE_HOTKEY_BESHIFTDOWN";
		public const string REMOTE_HOTKEY_BELOADCORRUPT = "REMOTE_HOTKEY_BELOADCORRUPT";
		public const string REMOTE_HOTKEY_BEAPPLY = "REMOTE_HOTKEY_BEAPPLY";
		public const string REMOTE_HOTKEY_BESENDSTASH = "REMOTE_HOTKEY_BESENDSTASH";

	}
}