#if GDTK
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKStatsManager
    {

        #region Enumerations

        public enum ParseFor
        {
            Value = 0,
            Max = 1,
            Min = 2,
            BaseValue = 3,
            BaseMax = 4,
            BaseMin = 5,
            TagLevel = 6
        }

        #endregion

        #region Public Methods

        public static List<SimpleEvent> AutoSubscribe(string formula, SimpleEvent changeEvent, StatSource target, StatSource other = null)
        {
            Dictionary<string, StatSource> sources = new Dictionary<string, StatSource>();
            sources.Add("", target);

            GlobalStats global = ToolRegistry.GetComponent<GlobalStats>();
            if (global != null) sources.Add("global:", global.source);
            if (other != null) sources.Add("other:", other);

            return AutoSubscribe(formula, changeEvent, sources);
        }

        public static List<SimpleEvent> AutoSubscribe(string formula, SimpleEvent changeEvent, Dictionary<string, StatSource> sources)
        {
            GlobalStats gs = ToolRegistry.GetComponent<GlobalStats>();
            List<SimpleEvent> subscriptions = new List<SimpleEvent>();

            if (formula.IsNumeric())
            {
                return subscriptions;
            }

            // Stats
            List<string> requirements = GetStatSubscriptionRequirements(formula);
            GDTKStat stat;
            foreach (string requirement in requirements)
            {
                if (requirement.EndsWith(":max", StringComparison.OrdinalIgnoreCase))
                {
                    stat = GetStatForSubscription(requirement.Substring(0, requirement.Length - 4), sources);
                    if (stat != null)
                    {
                        stat.expressions.maximum.onValueChanged += changeEvent;
                        subscriptions.Add(stat.expressions.maximum.onValueChanged);
                    }
                }
                if (requirement.EndsWith(":min", StringComparison.OrdinalIgnoreCase))
                {
                    stat = GetStatForSubscription(requirement.Substring(0, requirement.Length - 4), sources);
                    if (stat != null)
                    {
                        stat.expressions.minimum.onValueChanged += changeEvent;
                        subscriptions.Add(stat.expressions.minimum.onValueChanged);
                    }
                }
                if (requirement.EndsWith(":special", StringComparison.OrdinalIgnoreCase))
                {
                    stat = GetStatForSubscription(requirement.Substring(0, requirement.Length - 8), sources);
                    if (stat != null)
                    {
                        stat.expressions.special.onValueChanged += changeEvent;
                        subscriptions.Add(stat.expressions.special.onValueChanged);
                    }
                }
                else
                {
                    stat = GetStatForSubscription(requirement, sources);
                    if (stat != null)
                    {
                        stat.expressions.value.onValueChanged += changeEvent;
                        subscriptions.Add(stat.expressions.value.onValueChanged);
                    }
                }
            }

            // Functions
            int start, end, len;
            bool wasFound;
            int startFrom = 0;
            while (true)
            {
                start = formula.LastIndexOf('[', startFrom);   // Start of function params
                if (start < 0) break;   // No mo functions
                end = formula.IndexOf(']', start);  // End of function params

                // Find correct function
                wasFound = false;
                foreach (var functionEntry in StatFunctions.functionSubscriptions)
                {
                    len = functionEntry.Key.Length;
                    if (len <= start)
                    {
                        if (formula.Substring(start - len, len) == functionEntry.Key)
                        {
                            // Perform this function
                            functionEntry.Value.Invoke(sources, formula.Substring(start + 1, end - start - 1), changeEvent, subscriptions);
                            formula = formula.Substring(0, start - len) + formula.Substring(end + 1);
                            wasFound = true;
                            break;
                        }
                    }
                }

                if (!wasFound)
                {
                    foreach (var functionEntry in StatFunctions.customFunctionSubscriptions)
                    {
                        len = functionEntry.Key.Length;
                        if (len <= start)
                        {
                            if (formula.Substring(start - len, len) == functionEntry.Key)
                            {
                                // Perform this function
                                functionEntry.Value.Invoke(sources, formula.Substring(start + 1, end - start - 1), changeEvent, subscriptions);
                                formula = formula.Substring(0, start - len) + formula.Substring(end + 1);
                                wasFound = true;
                                break;
                            }
                        }
                    }
                }

                // Not all functions support subscription
                if (!wasFound)
                {
                    startFrom = end;
                }
            }

            return subscriptions;

        }

        public static List<string> GetStatSubscriptionRequirements(string formula)
        {
            List<string> res = new List<string>();
            if (string.IsNullOrEmpty(formula)) return res;

            if (!formula.IsNumeric())
            {
                foreach (string part in MathSolver.SplitMath(formula, true, true))
                {
                    if (!part.IsNumeric())
                    {
                        if (part.IndexOf('[') == -1)
                        {
                            res.Add(part);
                        }
                    }
                }
            }

            return res;
        }

        public static double GetValue(string formula, StatSource statSource, StatSource other = null)
        {
            Dictionary<string, StatSource> sources = new Dictionary<string, StatSource>();
            sources.Add("", statSource);

            GlobalStats global = ToolRegistry.GetComponent<GlobalStats>();
            if (global != null) sources.Add("global:", global.source);

            if (other != null) sources.Add("other:", other);
            return GetValue(formula, sources);

        }

        public static double GetValue(string formula, Dictionary<string, StatSource> sources)
        {
            if(!sources.ContainsKey("global:"))
            {
                GlobalStats global = ToolRegistry.GetComponent<GlobalStats>();
                if (global != null) sources.Add("global:", global.source);
            }

            MathSolver ms = new MathSolver();
            ms.unknownFunctionHandler = (string function, string value, out string result) =>
            {
                result = HandleUnknownFunction(function, value, sources);
            };

            ms.unknownValueHandler = (string value, out string result) =>
            {
                result = GetStatValue(value, sources).ToString();
            };

            return ms.Parse(formula);
        }

        public static bool IsConditionTrue(string condition, StatSource statSource, StatSource other = null)
        {
            if (string.IsNullOrEmpty(condition)) return true;

            GlobalStats global = ToolRegistry.GetComponent<GlobalStats>();
            
            Dictionary<string, StatSource> sources = new Dictionary<string, StatSource>();
            sources.Add("", statSource);
            if (global != null) sources.Add("global:", global.source);
            if (other != null) sources.Add("other:", other);


            return IsConditionTrue(condition, sources);

        }

        public static bool IsConditionTrue(string condition, Dictionary<string, StatSource> sources)
        {
            if (string.IsNullOrEmpty(condition)) return true;

            GlobalStats gs = ToolRegistry.GetComponent<GlobalStats>();

            MathSolver ms = new MathSolver();

            ms.unknownFunctionHandler = (string function, string value, out string result) =>
            {
                result = HandleUnknownFunction(function, value, sources);
            };

            ms.unknownValueHandler = (string value, out string result) =>
            {
                result = GetStatValue(value, sources).ToString();
            };

            return ms.IsTrue(condition);
        }

        public static bool StatMeetsConditions(GDTKStatusEffect effect, List<GDTKStatusEffect> activeEffects)
        {
            int allowedCount = Mathf.Max(1, effect.maxStack);
            int activeCount = 0;

            foreach (GDTKStatusEffect activeEffect in activeEffects)
            {
                /// Check max count
                if (activeEffect.info.id == effect.info.id)
                {
                    activeCount += 1;
                    if (activeCount >= allowedCount) return false;
                }
            }

            return true;
        }

        #endregion

        #region Private Methods

        private static double GetStatValue(string formula, Dictionary<string, StatSource> sources)
        {
            ParseFor parseFor = ParseFor.Value;
            string check = formula.ToLower();
            string statName;
            GDTKStat stat = null;
            StatSource statSource = null;

            if (check.EndsWith(":max"))
            {
                statName = formula.Substring(0, formula.Length - 4);
                parseFor = ParseFor.Max;
            }
            else if (check.EndsWith(":min"))
            {
                statName = formula.Substring(0, formula.Length - 4);
                parseFor = ParseFor.Min;
            }
            else if (check.EndsWith(":base"))
            {
                statName = formula.Substring(0, formula.Length - 5);
                parseFor = ParseFor.BaseValue;
            }
            else if (check.EndsWith(":basemax"))
            {
                statName = formula.Substring(0, formula.Length - 8);
                parseFor = ParseFor.BaseMax;
            }
            else if (check.EndsWith(":basemin"))
            {
                statName = formula.Substring(0, formula.Length - 8);
                parseFor = ParseFor.BaseMin;
            }
            else if (check.EndsWith(":special"))
            {
                statName = formula.Substring(0, formula.Length - 8);
                parseFor = ParseFor.TagLevel;
            }
            else
            {
                statName = formula;
            }

            foreach(var entry in sources)
            {
                if(entry.Key == string.Empty)
                {
                    if(entry.Value.stats.TryGetValue(statName, out stat))
                    {
                        statSource = entry.Value;
                        break;
                    }
                }
                else if(statName.StartsWith(entry.Key))
                {
                    entry.Value.stats.TryGetValue(statName.Substring(entry.Key.Length), out stat);
                    statSource = entry.Value;
                    break;
                }
            }

            if (stat == null)
            {
                StringExtensions.LogError("GDTKStatManager", "ParseValue", "Could not find stat named '" + statName + "'!");
                return 0;
            }
            else
            {
                // Make sure we've initialized the stat
                if (!stat.initialized)
                {
                    stat.Initialize(statSource);
                }

                return parseFor switch
                {
                    ParseFor.Max => stat.maximum,
                    ParseFor.Min => stat.minimum,
                    ParseFor.BaseMax => stat.expressions.maximum.value,
                    ParseFor.BaseMin => stat.expressions.minimum.value,
                    ParseFor.BaseValue => stat.expressions.value.value,
                    ParseFor.TagLevel => stat.special,
                    _ => stat.value,
                };
            }

            throw new Exception($"Invalid command: {formula}");
        }

        private static GDTKStat GetStatForSubscription(string statId, StatSource target, StatSource global, StatSource other)
        {
            GDTKStat stat;

            if (statId.IndexOf("global:", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (global == null) return null;
                global.stats.TryGetValue(statId.Substring(7), out stat);
            }
            else if (statId.IndexOf("other:", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (other == null) return null;
                other.stats.TryGetValue(statId.Substring(6), out stat);
            }
            else
            {
                if (target == null)
                {
                    Debug.Log("Could not subscribe to target '" + statId + "'");
                }
                target.stats.TryGetValue(statId, out stat);
            }

            return stat;
        }

        private static GDTKStat GetStatForSubscription(string statId, Dictionary<string, StatSource> sources)
        {
            GDTKStat stat;

            foreach (var entry in sources)
            {
                if (entry.Key == string.Empty || statId.StartsWith(entry.Key))
                {
                    if(entry.Value.stats.TryGetValue(statId.Substring(entry.Key.Length), out stat))
                    {
                        return stat;
                    }
                }
            }

            return null;
        }


        private static string HandleUnknownFunction(string function, string value, Dictionary<string, StatSource> sources)
        {
            string result;

            if(StatFunctions.functions.ContainsKey(function))
            {
                StatFunctions.functions[function].Invoke(value, sources, out result);
                return result;
            }

            if (StatFunctions.customFunctions.ContainsKey(function))
            {
                StatFunctions.customFunctions[function].Invoke(value, sources, out result);
                return result;
            }

            StringExtensions.LogError("GDTKStatsManager", "SolveSystemFunctions", "Unknown function in " + function + "[" + value + "]");
            return "0";
        }

        #endregion

    }
}
#endif