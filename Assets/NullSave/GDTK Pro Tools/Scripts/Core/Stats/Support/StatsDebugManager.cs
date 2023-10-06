#if GDTK
using System;
using System.Text;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class StatsDebugManager
    {

        #region Public Methods

        public static void GlobalStatsRequest(GlobalStats source, string message)
        {
            if (message == Messages.DEBUG_CONSOLE_IDENTIFY)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
                sb.Append("<color=#00B1FF><b>GlobalStats</b></color>\r\n");
                return;
            }
            if (!message.StartsWith(Messages.DEBUG_CONSOLE_REQUEST)) return;
            message = message.Substring(Messages.DEBUG_CONSOLE_REQUEST.Length).Trim();

            string matchTo = "globalstats";
            if (!message.StartsWith(matchTo, StringComparison.OrdinalIgnoreCase)) return;

            string[] parts = message.Substring(matchTo.Length).Trim().Split(' ');
            switch(parts[0].ToLower())
            {
                case "addeffect":
                case "+e":
                    AddEffect(source.gameObject, source, parts);
                    break;
                case "getstat":
                case "gs":
                    GetStat(source.gameObject, source, parts);
                    break;
                case "help":
                case "h":
                case "-h":
                    SendHelpGlobal(source);
                    break;
                case "manualheartbeat":
                case "mh":
                    ManualHeartbeat(source.gameObject, source, parts);
                    break;
                case "removeeffect":
                case "-e":
                    RemoveEffect(source.gameObject, source, parts);
                    break;
                case "setstat":
                case "ss":
                    SetStat(source.gameObject, source, parts);
                    break;
            }
        }

        public static void BasicStatsRequest(BasicStats source, GameObject gameObject, string message)
        {
            if (message == Messages.DEBUG_CONSOLE_IDENTIFY)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
                sb.Append("<color=#00B1FF><b>" + gameObject.name + ".stats</b></color>\r\n");
                Broadcaster.PublicBroadcast(gameObject, sb.ToString());
                return;
            }
            if (!message.StartsWith(Messages.DEBUG_CONSOLE_REQUEST)) return;
            message = message.Substring(Messages.DEBUG_CONSOLE_REQUEST.Length).Trim();

            string matchTo = gameObject.name + ".stats";
            if (!message.StartsWith(matchTo, StringComparison.OrdinalIgnoreCase)) return;

            string[] parts = message.Substring(matchTo.Length).Trim().Split(' ');
            switch (parts[0].ToLower())
            {
                case "addattribute":
                case "+a":
                    AddAttribute(gameObject, source, parts);
                    break;
                case "addstatuscondition":
                case "+sc":
                    AddStatusCondition(gameObject, source, parts);
                    break;
                case "getstat":
                case "gs":
                    GetStat(source.gameObject, source, parts);
                    break;
                case "help":
                case "h":
                case "-h":
                    SendHelpBasic(source);
                    break;
                case "load":
                    Load(gameObject, source, parts);
                    break;
                case "manualheartbeat":
                case "mh":
                    ManualHeartbeat(gameObject, source, parts);
                    break;
                case "removeattribute":
                case "-a":
                    RemoveAttribute(gameObject, source, parts);
                    break;
                case "removestatuscondition":
                case "-sc":
                    RemoveStatusCondition(gameObject, source, parts);
                    break;
                case "save":
                    Save(gameObject, source, parts);
                    break;
                case "setstat":
                case "ss":
                    SetStat(gameObject, source, parts);
                    break;
            }
        }

        public static void StatsAndEffectsRequest(StatsAndEffects source, GameObject gameObject, string message)
        {
            if (message == Messages.DEBUG_CONSOLE_IDENTIFY)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
                sb.Append("<color=#00B1FF><b>" + gameObject.name + ".stats</b></color>\r\n");
                Broadcaster.PublicBroadcast(gameObject, sb.ToString());
                return;
            }
            if (!message.StartsWith(Messages.DEBUG_CONSOLE_REQUEST)) return;
            message = message.Substring(Messages.DEBUG_CONSOLE_REQUEST.Length).Trim();

            string matchTo = gameObject.name + ".stats";
            if (!message.StartsWith(matchTo, StringComparison.OrdinalIgnoreCase)) return;

            string[] parts = message.Substring(matchTo.Length).Trim().Split(' ');
            switch (parts[0].ToLower())
            {
                case "addattribute":
                case "+a":
                    AddAttribute(gameObject, source, parts);
                    break;
                case "addeffect":
                case "+e":
                    AddEffect(source.gameObject, source, parts);
                    break;
                case "addstatuscondition":
                case "+sc":
                    AddStatusCondition(gameObject, source, parts);
                    break;
                case "getstat":
                case "gs":
                    GetStat(source.gameObject, source, parts);
                    break;
                case "help":
                case "h":
                case "-h":
                    SendHelpStatsAndEffects(source);
                    break;
                case "load":
                    Load(gameObject, source, parts);
                    break;
                case "manualheartbeat":
                case "mh":
                    ManualHeartbeat(gameObject, source, parts);
                    break;
                case "removeattribute":
                case "-a":
                    RemoveAttribute(gameObject, source, parts);
                    break;
                case "removeeffect":
                case "-e":
                    RemoveEffect(source.gameObject, source, parts);
                    break;
                case "removestatuscondition":
                case "-sc":
                    RemoveStatusCondition(gameObject, source, parts);
                    break;
                case "save":
                    Save(gameObject, source, parts);
                    break;
                case "setstat":
                case "ss":
                    SetStat(gameObject, source, parts);
                    break;
            }
        }

        public static void NPCStatsRequest(NPCStats source, GameObject gameObject, string message)
        {
            if (message == Messages.DEBUG_CONSOLE_IDENTIFY)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
                sb.Append("<color=#00B1FF><b>" + gameObject.name + ".stats</b></color>\r\n");
                Broadcaster.PublicBroadcast(gameObject, sb.ToString());
                return;
            }
            if (!message.StartsWith(Messages.DEBUG_CONSOLE_REQUEST)) return;
            message = message.Substring(Messages.DEBUG_CONSOLE_REQUEST.Length).Trim();

            string matchTo = gameObject.name + ".stats";
            if (!message.StartsWith(matchTo, StringComparison.OrdinalIgnoreCase)) return;

            string[] parts = message.Substring(matchTo.Length).Trim().Split(' ');
            switch (parts[0].ToLower())
            {
                case "addattribute":
                case "+a":
                    AddAttribute(gameObject, source, parts);
                    break;
                case "addeffect":
                case "+e":
                    AddEffect(source.gameObject, source, parts);
                    break;
                case "addlanguage":
                case "+l":
                    AddLanguage(gameObject, source, parts);
                    break;
                case "addstatuscondition":
                case "+sc":
                    AddStatusCondition(gameObject, source, parts);
                    break;
                case "getstat":
                case "gs":
                    GetStat(source.gameObject, source, parts);
                    break;
                case "help":
                case "h":
                case "-h":
                    SendHelpNPCStats(source);
                    break;
                case "load":
                    Load(gameObject, source, parts);
                    break;
                case "manualheartbeat":
                case "mh":
                    ManualHeartbeat(gameObject, source, parts);
                    break;
                case "removeattribute":
                case "-a":
                    RemoveAttribute(gameObject, source, parts);
                    break;
                case "removeeffect":
                case "-e":
                    RemoveEffect(source.gameObject, source, parts);
                    break;
                case "removelanguage":
                case "-l":
                    RemoveLanguage(gameObject, source, parts);
                    break;
                case "removestatuscondition":
                case "-sc":
                    RemoveStatusCondition(gameObject, source, parts);
                    break;
                case "save":
                    Save(gameObject, source, parts);
                    break;
                case "setstat":
                case "ss":
                    SetStat(gameObject, source, parts);
                    break;
            }
        }

        public static void PlayerCharacterStatsRequest(PlayerCharacterStats source, GameObject gameObject, string message)
        {
            if (message == Messages.DEBUG_CONSOLE_IDENTIFY)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
                sb.Append("<color=#00B1FF><b>" + gameObject.name + ".stats</b></color>\r\n");
                Broadcaster.PublicBroadcast(gameObject, sb.ToString());
                return;
            }
            if (!message.StartsWith(Messages.DEBUG_CONSOLE_REQUEST)) return;
            message = message.Substring(Messages.DEBUG_CONSOLE_REQUEST.Length).Trim();

            string matchTo = gameObject.name + ".stats";
            if (!message.StartsWith(matchTo, StringComparison.OrdinalIgnoreCase)) return;

            string[] parts = message.Substring(matchTo.Length).Trim().Split(' ');
            switch (parts[0].ToLower())
            {
                case "addattribute":
                case "+a":
                    AddAttribute(gameObject, source, parts);
                    break;
                case "addeffect":
                case "+e":
                    AddEffect(source.gameObject, source, parts);
                    break;
                case "addlanguage":
                case "+l":
                    AddLanguage(gameObject, source, parts);
                    break;
                case "addstatuscondition":
                case "+sc":
                    AddStatusCondition(gameObject, source, parts);
                    break;
                case "getstat":
                case "gs":
                    GetStat(source.gameObject, source, parts);
                    break;
                case "help":
                case "h":
                case "-h":
                    SendHelpPlayerCharacterStats(source);
                    break;
                case "load":
                    Load(gameObject, source, parts);
                    break;
                case "manualheartbeat":
                case "mh":
                    ManualHeartbeat(gameObject, source, parts);
                    break;
                case "removeattribute":
                case "-a":
                    RemoveAttribute(gameObject, source, parts);
                    break;
                case "removeeffect":
                case "-e":
                    RemoveEffect(source.gameObject, source, parts);
                    break;
                case "removelanguage":
                case "-l":
                    RemoveLanguage(gameObject, source, parts);
                    break;
                case "removestatuscondition":
                case "-sc":
                    RemoveStatusCondition(gameObject, source, parts);
                    break;
                case "save":
                    Save(gameObject, source, parts);
                    break;
                case "setbackground":
                case "sb":
                    SetBackground(gameObject, source, parts);
                    break;
                case "setrace":
                case "sr":
                    SetRace(gameObject, source, parts);
                    break;
                case "setstat":
                case "ss":
                    SetStat(gameObject, source, parts);
                    break;
            }
        }

        #endregion

        #region Private Methods

        private static void AddAttribute(GameObject go, BasicStats target, string[] parts)
        {
            if (parts.Length != 2)
            {
                SendError(go, "AddAttribute", "Invalid number of parameters");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>" + go.name + " AddAttribute</b></color>\r\n");

            if (target.AddAttribute(parts[1], null) != null)
            {
                sb.Append("Attribute added");
            }
            else
            {
                sb.Append("Attribute not added");
            }

            Broadcaster.PublicBroadcast(target, sb.ToString());
        }

        private static void AddEffect(GameObject go, StatsAndEffects target, string[] parts)
        {
            if (parts.Length != 2)
            {
                SendError(go, "AddEffect", "Invalid number of parameters");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>" + go.name + " AddEffect</b></color>\r\n");

            if(target.AddEffect(parts[1]))
            {
                sb.Append("Effect added");
            }
            else
            {
                sb.Append("Effect not added");
            }

            Broadcaster.PublicBroadcast(target, sb.ToString());
        }

        private static void AddLanguage(GameObject go, NPCStats target, string[] parts)
        {
            if (parts.Length != 2)
            {
                SendError(go, "AddLanguage", "Invalid number of parameters");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>" + go.name + " AddLanguage</b></color>\r\n");

            if (target.AddLanguage(parts[1], null) != null)
            {
                sb.Append("Language added");
            }
            else
            {
                sb.Append("Language not added");
            }

            Broadcaster.PublicBroadcast(target, sb.ToString());
        }

        private static void AddStatusCondition(GameObject go, BasicStats target, string[] parts)
        {
            if (parts.Length != 2)
            {
                SendError(go, "AddStatusCondition", "Invalid number of parameters");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>" + go.name + " AddStatusCondition</b></color>\r\n");

            if (target.AddStatusCondition(parts[1], null) != null)
            {
                sb.Append("Status Condition added");
            }
            else
            {
                sb.Append("Status Condition not added");
            }

            Broadcaster.PublicBroadcast(target, sb.ToString());
        }

        private static void GetStat(GameObject go, BasicStats target, string[] parts)
        {
            if (parts.Length != 3)
            {
                SendError(go, "GetStat", "Invalid number of parameters");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>" + go.name + " GetStat</b></color>\r\n");

            GDTKStat stat = target.GetStat(parts[1]);
            if(stat == null)
            {
                sb.Append(parts[1] + " stat not found");
            }
            else
            {
                switch(parts[2].ToLower())
                {
                    case "max":
                    case "maximum":
                        sb.Append(parts[1] + ".max = " + stat.maximum);
                        break;
                    case "min":
                    case "minimum":
                        sb.Append(parts[1] + ".min = " + stat.minimum);
                        break;
                    case "value":
                    case "val":
                        sb.Append(parts[1] + ".value = " + stat.value);
                        break;
                    case "special":
                        sb.Append(parts[1] + ".special = " + stat.special);
                        break;
                    default:
                        sb.Append("Invalid value '" + parts[2] + "' requested");
                        break;
                }
            }

            Broadcaster.PublicBroadcast(target, sb.ToString());
        }

        private static void Load(GameObject go, BasicStats target, string[] parts)
        {
            if (parts.Length != 2)
            {
                SendError(go, "Load", "Invalid number of parameters");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>" + go.name + " Load complete</b></color>\r\n");

            target.DataLoad(parts[1]);

            Broadcaster.PublicBroadcast(target, sb.ToString());
        }

        private static void ManualHeartbeat(GameObject go, BasicStats target, string[] parts)
        {
            if (parts.Length != 2)
            {
                SendError(go, "ManualHeartbeat", "Invalid number of parameters");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>" + go.name + " ManualHeartbeat</b></color>\r\n");
            target.RaiseTokenHeartbeat(float.Parse(parts[1]));
            sb.Append("Manual heartbeat raised: " + parts[1] + " tokens");
           

            Broadcaster.PublicBroadcast(target, sb.ToString());
        }

        private static void RemoveAttribute(GameObject go, BasicStats target, string[] parts)
        {
            if (parts.Length != 2)
            {
                SendError(go, "RemoveAttribute", "Invalid number of parameters");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>" + go.name + " RemoveEffect</b></color>\r\n");

            target.RemoveAttribute(parts[1]);
            sb.Append("Attribute Removed");

            Broadcaster.PublicBroadcast(target, sb.ToString());
        }

        private static void RemoveEffect(GameObject go, StatsAndEffects target, string[] parts)
        {
            if (parts.Length != 2)
            {
                SendError(go, "RemoveEffect", "Invalid number of parameters");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>" + go.name + " RemoveEffect</b></color>\r\n");

            if (target.RemoveEffect(parts[1]))
            {
                sb.Append("Effect removed");
            }
            else
            {
                sb.Append("Effect not removed");
            }

            Broadcaster.PublicBroadcast(target, sb.ToString());
        }

        private static void RemoveLanguage(GameObject go, NPCStats target, string[] parts)
        {
            if (parts.Length != 2)
            {
                SendError(go, "RemoveLanguage", "Invalid number of parameters");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>" + go.name + " RemoveLanguage</b></color>\r\n");

            if (target.RemoveLanguage(parts[1]))
            {
                sb.Append("Language removed");
            }
            else
            {
                sb.Append("Language not removed");
            }

            Broadcaster.PublicBroadcast(target, sb.ToString());
        }

        private static void RemoveStatusCondition(GameObject go, BasicStats target, string[] parts)
        {
            if (parts.Length != 2)
            {
                SendError(go, "RemoveStatusCondition", "Invalid number of parameters");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>" + go.name + " RemoveEffect</b></color>\r\n");

            target.RemoveStatusCondition(parts[1]);
            sb.Append("Status Condition Removed");

            Broadcaster.PublicBroadcast(target, sb.ToString());
        }

        private static void Save(GameObject go, BasicStats target, string[] parts)
        {
            if (parts.Length != 2)
            {
                SendError(go, "Save", "Invalid number of parameters");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>" + go.name + " Save complete</b></color>\r\n");

            target.DataSave(parts[1]);

            Broadcaster.PublicBroadcast(target, sb.ToString());
        }

        private static void SendError(GameObject go, string command, string error)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#FF0000><b>" + go.name + " " + command + "</b></color>\r\n");
            sb.Append(error + "\r\n");

            Broadcaster.PublicBroadcast(go, sb.ToString());
        }

        private static void SendHelpBasic(BasicStats source)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>Basic Stats Commands</b></color>\r\n");
            sb.Append("<i>AddAttributet</i> or <i>+a</i>: Adds an attribute from the database. Supply <i>[attributeId]</i>\r\n");
            sb.Append("<i>AddStatusCondition</i> or <i>+sc</i>: Adds a status condition from the database. Supply <i>[statusConditionId]</i>\r\n");
            sb.Append("<i>GetStat</i> or <i>gs</i>: Gets a stat value. Supply <i>[statId] [value | min | max | special]</i>\r\n");
            sb.Append("<i>Load</i>: Loads data from file. Supply <i>[filename]</i>\r\n");
            sb.Append("<i>ManualHeartbeat</i> or <i>mh</i>: Raises a manual heartbeat. Supply <i>[tokenCount]</i>\r\n");
            sb.Append("<i>RemoveAttribute</i> or <i>-a</i>: Removes an attribute. Supply <i>[attributeId]</i>\r\n");
            sb.Append("<i>RemoveStatusCondition</i> or <i>-sc</i>: Removes a status condition. Supply <i>[statusConditionId]</i>\r\n");
            sb.Append("<i>Save</i>: Saves data to file. Supply <i>[filename]</i>\r\n");
            sb.Append("<i>SetStat</i> or <i>ss</i>: Sets a stat value. Supply <i>[statId] [value | min | max | special] [value]</i>\r\n");

            Broadcaster.PublicBroadcast(source, sb.ToString());
        }

        private static void SendHelpGlobal(GlobalStats source)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>Global Stats Commands</b></color>\r\n");
            sb.Append("<i>AddEffect</i> or <i>+e</i>: Adds an effect from the database. Supply <i>[statusEffectId]</i>\r\n");
            sb.Append("<i>GetStat</i> or <i>gs</i>: Gets a stat value. Supply <i>[statId] [value | min | max | special]</i>\r\n");
            sb.Append("<i>ManualHeartbeat</i> or <i>mh</i>: Raises a manual heartbeat. Supply <i>[tokenCount]</i>\r\n");
            sb.Append("<i>RemoveEffect</i> or <i>-e</i>: Removes an active effect. Supply <i>[statusEffectId]</i>\r\n");
            sb.Append("<i>SetStat</i> or <i>ss</i>: Sets a stat value. Supply <i>[statId] [value | min | max | special] [value]</i>\r\n");

            Broadcaster.PublicBroadcast(source, sb.ToString());
        }

        private static void SendHelpNPCStats(BasicStats source)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>NPC Stats Commands</b></color>\r\n");
            sb.Append("<i>AddAttributet</i> or <i>+a</i>: Adds an attribute from the database. Supply <i>[attributeId]</i>\r\n");
            sb.Append("<i>AddEffect</i> or <i>+e</i>: Adds an effect from the database. Supply <i>[statusEffectId]</i>\r\n");
            sb.Append("<i>AddLanguage</i> or <i>+l</i>: Adds a language from the database. Supply <i>[languageId]</i>\r\n");
            sb.Append("<i>AddStatusCondition</i> or <i>+sc</i>: Adds a status condition from the database. Supply <i>[statusConditionId]</i>\r\n");
            sb.Append("<i>GetStat</i> or <i>gs</i>: Gets a stat value. Supply <i>[statId] [value | min | max | special]</i>\r\n");
            sb.Append("<i>Load</i>: Loads data from file. Supply <i>[filename]</i>\r\n");
            sb.Append("<i>ManualHeartbeat</i> or <i>mh</i>: Raises a manual heartbeat. Supply <i>[tokenCount]</i>\r\n");
            sb.Append("<i>RemoveAttribute</i> or <i>-a</i>: Removes an attribute. Supply <i>[attributeId]</i>\r\n");
            sb.Append("<i>RemoveEffect</i> or <i>-e</i>: Removes an active effect. Supply <i>[statusEffectId]</i>\r\n");
            sb.Append("<i>RemoveLanguage</i> or <i>-l</i>: Removes a language from the database. Supply <i>[languageId]</i>\r\n");
            sb.Append("<i>RemoveStatusCondition</i> or <i>-sc</i>: Removes a status condition. Supply <i>[statusConditionId]</i>\r\n");
            sb.Append("<i>Save</i>: Saves data to file. Supply <i>[filename]</i>\r\n");
            sb.Append("<i>SetStat</i> or <i>ss</i>: Sets a stat value. Supply <i>[statId] [value | min | max | special] [value]</i>\r\n");

            Broadcaster.PublicBroadcast(source, sb.ToString());
        }

        private static void SendHelpPlayerCharacterStats(BasicStats source)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>Player Character Stats Commands</b></color>\r\n");
            sb.Append("<i>AddAttributet</i> or <i>+a</i>: Adds an attribute from the database. Supply <i>[attributeId]</i>\r\n");
            sb.Append("<i>AddEffect</i> or <i>+e</i>: Adds an effect from the database. Supply <i>[statusEffectId]</i>\r\n");
            sb.Append("<i>AddLanguage</i> or <i>+l</i>: Adds a language from the database. Supply <i>[languageId]</i>\r\n");
            sb.Append("<i>AddStatusCondition</i> or <i>+sc</i>: Adds a status condition from the database. Supply <i>[statusConditionId]</i>\r\n");
            sb.Append("<i>GetStat</i> or <i>gs</i>: Gets a stat value. Supply <i>[statId] [value | min | max | special]</i>\r\n");
            sb.Append("<i>Load</i>: Loads data from file. Supply <i>[filename]</i>\r\n");
            sb.Append("<i>ManualHeartbeat</i> or <i>mh</i>: Raises a manual heartbeat. Supply <i>[tokenCount]</i>\r\n");
            sb.Append("<i>RemoveAttribute</i> or <i>-a</i>: Removes an attribute. Supply <i>[attributeId]</i>\r\n");
            sb.Append("<i>RemoveEffect</i> or <i>-e</i>: Removes an active effect. Supply <i>[statusEffectId]</i>\r\n");
            sb.Append("<i>RemoveLanguage</i> or <i>-l</i>: Removes a language from the database. Supply <i>[languageId]</i>\r\n");
            sb.Append("<i>RemoveStatusCondition</i> or <i>-sc</i>: Removes a status condition. Supply <i>[statusConditionId]</i>\r\n");
            sb.Append("<i>Save</i>: Saves data to file. Supply <i>[filename]</i>\r\n");
            sb.Append("<i>SetBackground</i> or <i>sb</i>: Sets player background. Supply <i>[backgroundId]</i>\r\n");
            sb.Append("<i>SetRace</i> or <i>sr</i>: Sets player race. Supply <i>[raceId]</i>\r\n");
            sb.Append("<i>SetStat</i> or <i>ss</i>: Sets a stat value. Supply <i>[statId] [value | min | max | special] [value]</i>\r\n");

            Broadcaster.PublicBroadcast(source, sb.ToString());
        }

        private static void SendHelpStatsAndEffects(BasicStats source)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>Stats And Effects Commands</b></color>\r\n");
            sb.Append("<i>AddAttributet</i> or <i>+a</i>: Adds an attribute from the database. Supply <i>[attributeId]</i>\r\n");
            sb.Append("<i>AddEffect</i> or <i>+e</i>: Adds an effect from the database. Supply <i>[statusEffectId]</i>\r\n");
            sb.Append("<i>AddStatusCondition</i> or <i>+sc</i>: Adds a status condition from the database. Supply <i>[statusConditionId]</i>\r\n");
            sb.Append("<i>GetStat</i> or <i>gs</i>: Gets a stat value. Supply <i>[statId] [value | min | max | special]</i>\r\n");
            sb.Append("<i>Load</i>: Loads data from file. Supply <i>[filename]</i>\r\n");
            sb.Append("<i>ManualHeartbeat</i> or <i>mh</i>: Raises a manual heartbeat. Supply <i>[tokenCount]</i>\r\n");
            sb.Append("<i>RemoveAttribute</i> or <i>-a</i>: Removes an attribute. Supply <i>[attributeId]</i>\r\n");
            sb.Append("<i>RemoveEffect</i> or <i>-e</i>: Removes an active effect. Supply <i>[statusEffectId]</i>\r\n");
            sb.Append("<i>RemoveStatusCondition</i> or <i>-sc</i>: Removes a status condition. Supply <i>[statusConditionId]</i>\r\n");
            sb.Append("<i>Save</i>: Saves data to file. Supply <i>[filename]</i>\r\n");
            sb.Append("<i>SetStat</i> or <i>ss</i>: Sets a stat value. Supply <i>[statId] [value | min | max | special] [value]</i>\r\n");

            Broadcaster.PublicBroadcast(source, sb.ToString());
        }

        private static void SetBackground(GameObject go, PlayerCharacterStats target, string[] parts)
        {
            if (parts.Length != 2)
            {
                SendError(go, "SetBackground", "Invalid number of parameters");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>" + go.name + " SetBackground</b></color>\r\n");

            target.SetBackground(parts[1]);

            Broadcaster.PublicBroadcast(target, sb.ToString());
        }

        private static void SetRace(GameObject go, PlayerCharacterStats target, string[] parts)
        {
            if (parts.Length != 2)
            {
                SendError(go, "SetRace", "Invalid number of parameters");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>" + go.name + " SetRace</b></color>\r\n");

            target.SetRace(parts[1]);

            Broadcaster.PublicBroadcast(target, sb.ToString());
        }

        private static void SetStat(GameObject go, BasicStats target, string[] parts)
        {
            if (parts.Length != 4)
            {
                SendError(go, "SetStat", "Invalid number of parameters");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(Messages.DEBUG_CONSOLE_RESPONSE);
            sb.Append("<color=#00B1FF><b>" + go.name + " SetStat</b></color>\r\n");

            GDTKStat stat = target.GetStat(parts[1]);
            if (stat == null)
            {
                sb.Append(parts[1] + " stat not found");
            }
            else
            {
                switch (parts[2].ToLower())
                {
                    case "max":
                    case "maximum":
                        stat.maximum = target.GetValue(parts[3], null);
                        sb.Append(parts[1] + ".max = " + stat.maximum);
                        break;
                    case "min":
                    case "minimum":
                        stat.minimum = target.GetValue(parts[3], null);
                        sb.Append(parts[1] + ".min = " + stat.minimum);
                        break;
                    case "value":
                    case "val":
                        stat.value = target.GetValue(parts[3], null);
                        sb.Append(parts[1] + ".value = " + stat.value);
                        break;
                    case "special":
                        stat.special = target.GetValue(parts[3], null);
                        sb.Append(parts[1] + ".special = " + stat.special);
                        break;
                    default:
                        sb.Append("Invalid value '" + parts[2] + "' requested");
                        break;
                }
            }

            Broadcaster.PublicBroadcast(target, sb.ToString());
        }

        #endregion

    }
}
#endif