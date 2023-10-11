#if GDTK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NullSave.GDTK.Stats
{
    public static class StatFunctions
    {

        #region Fields

        private static Dictionary<string, StatFunction> m_functionList;
        private static Dictionary<string, StatFunctionSubscription> m_subscriptionList;

        private static Dictionary<string, StatFunction> m_customFunctionList;
        private static Dictionary<string, StatFunctionSubscription> m_customSubscriptionList;

        #endregion

        #region Properties

        public static Dictionary<string, StatFunction> customFunctions
        {
            get
            {
                if (m_customFunctionList == null)
                {
                    m_customFunctionList = new Dictionary<string, StatFunction>();
                }
                return m_customFunctionList;
            }
        }

        public static Dictionary<string, StatFunctionSubscription> customFunctionSubscriptions
        {
            get { return m_customSubscriptionList; }
        }

        public static Dictionary<string, StatFunction> functions
        {
            get
            {
                if (m_functionList == null)
                {
                    m_functionList = new Dictionary<string, StatFunction>();
                    RegisterFunctions();

                    var sorted = m_functionList.OrderByDescending(m => m.Key.Length);

                    Dictionary<string, StatFunction> tempList = new Dictionary<string, StatFunction>();
                    foreach (var sort in sorted)
                    {
                        tempList.Add(sort.Key, sort.Value);
                    }

                    m_functionList = tempList;
                }

                return m_functionList;
            }
        }

        public static Dictionary<string, StatFunctionSubscription> functionSubscriptions
        {
            get
            {
                if (m_subscriptionList == null)
                {
                    m_subscriptionList = new Dictionary<string, StatFunctionSubscription>();
                    RegisterFunctionSubscriptions();

                    var sorted = m_subscriptionList.OrderByDescending(m => m.Key.Length);

                    Dictionary<string, StatFunctionSubscription> tempList = new Dictionary<string, StatFunctionSubscription>();
                    foreach (var sort in sorted)
                    {
                        tempList.Add(sort.Key, sort.Value);
                    }

                    m_subscriptionList = tempList;
                }

                return m_subscriptionList;
            }
        }

        #endregion

        #region Function Methods

        public static void AddCustomFunction(string functionId, StatFunction function)
        {
            if (m_customFunctionList == null) m_customFunctionList = new Dictionary<string, StatFunction>();
            m_customFunctionList.Add(functionId, function);
        }

        public static void AddCustomFunctionSubscripiton(string functionId, StatFunctionSubscription subscription)
        {
            if (m_customSubscriptionList == null) m_customSubscriptionList = new Dictionary<string, StatFunctionSubscription>();
            m_customSubscriptionList.Add(functionId, subscription);
        }

        public static void ClassLevel(string request, Dictionary<string, StatSource> sources, out string result)
        {
            bool hasCol = request.IndexOf(':') >= 0;
            foreach(var entry in sources)
            {
                if((entry.Key != "" && request.StartsWith(entry.Key)) || (entry.Key == string.Empty && !hasCol))
                {
                    result = ClassLevel(entry.Value, request.Substring(entry.Key.Length));
                }
            }

            result = "0";
        }

        public static void HasAttribute(string request, Dictionary<string, StatSource> sources, out string result)
        {
            bool hasCol = request.IndexOf(':') >= 0;
            foreach (var entry in sources)
            {
                if ((entry.Key != "" && request.StartsWith(entry.Key)) || (entry.Key == string.Empty && !hasCol))
                {
                    result = HasAttribute(entry.Value, request.Substring(entry.Key.Length));
                }
            }

            result = "0";
        }

        public static void HasClass(string request, Dictionary<string, StatSource> sources, out string result)
        {
            bool hasCol = request.IndexOf(':') >= 0;
            foreach (var entry in sources)
            {
                if ((entry.Key != "" && request.StartsWith(entry.Key)) || (entry.Key == string.Empty && !hasCol))
                {
                    result = HasClass(entry.Value, request.Substring(entry.Key.Length));
                }
            }

            result = "0";
        }

        public static void HasPerk(string request, Dictionary<string, StatSource> sources, out string result)
        {
            bool hasCol = request.IndexOf(':') >= 0;
            foreach (var entry in sources)
            {
                if ((entry.Key != "" && request.StartsWith(entry.Key)) || (entry.Key == string.Empty && !hasCol))
                {
                    result = HasPerk(entry.Value, request.Substring(entry.Key.Length));
                }
            }

            result = "0";
        }

        public static void IsRace(string request, Dictionary<string, StatSource> sources, out string result)
        {
            bool hasCol = request.IndexOf(':') >= 0;
            foreach (var entry in sources)
            {
                if ((entry.Key != "" && request.StartsWith(entry.Key)) || (entry.Key == string.Empty && !hasCol))
                {
                    result = IsRace(entry.Value, request.Substring(entry.Key.Length));
                }
            }

            result = "0";
        }

        #endregion

        #region Subscription Methods

        public static void SubClassLevel(Dictionary<string, StatSource> sources, string request, SimpleEvent requester, List<SimpleEvent> subscriptionList)
        {
            bool hasCol = request.IndexOf(':') >= 0;
            foreach (var entry in sources)
            {
                if ((entry.Key != "" && request.StartsWith(entry.Key)) || (entry.Key == string.Empty && !hasCol))
                {
                    object pc = entry.Value.RunAction("getClass", new object[] { request.Substring(6) });
                    if (pc is GDTKClass @class)
                    {
                        @class.onLevelChanged += requester;
                        subscriptionList.Add(@class.onLevelChanged);
                    }
                }
            }
        }

        public static void SubHasAttribute(Dictionary<string, StatSource> sources, string request, SimpleEvent requester, List<SimpleEvent> subscriptionList)
        {
            bool hasCol = request.IndexOf(':') >= 0;
            foreach (var entry in sources)
            {
                if ((entry.Key != "" && request.StartsWith(entry.Key)) || (entry.Key == string.Empty && !hasCol))
                {
                    entry.Value.RunAction("SubscribeToAttributeChange", new object[] { requester, request });
                }
            }

        }

        public static void SubHasClass(Dictionary<string, StatSource> sources, string request, SimpleEvent requester, List<SimpleEvent> subscriptionList)
        {
            bool hasCol = request.IndexOf(':') >= 0;
            foreach (var entry in sources)
            {
                if ((entry.Key != "" && request.StartsWith(entry.Key)) || (entry.Key == string.Empty && !hasCol))
                {
                    object result = entry.Value.RunAction("classesChanged", new object[] { requester });
                    if (result != null)
                    {
                        subscriptionList.Add((SimpleEvent)result);
                    }
                }
            }
        }

        public static void SubHasPerk(Dictionary<string, StatSource> sources, string request, SimpleEvent requester, List<SimpleEvent> subscriptionList)
        {
            bool hasCol = request.IndexOf(':') >= 0;
            foreach (var entry in sources)
            {
                if ((entry.Key != "" && request.StartsWith(entry.Key)) || (entry.Key == string.Empty && !hasCol))
                {
                    object result = entry.Value.RunAction("perksChanged", new object[] { requester });
                    if (result != null)
                    {
                        subscriptionList.Add((SimpleEvent)result);
                    }
                }
            }
        }

        public static void SubIsRace(Dictionary<string, StatSource> sources, string request, SimpleEvent requester, List<SimpleEvent> subscriptionList)
        {
            bool hasCol = request.IndexOf(':') >= 0;
            //!! TODO: These loops could be replaced with a single method to reduce code.
            foreach (var entry in sources)
            {
                if ((entry.Key != "" && request.StartsWith(entry.Key)) || (entry.Key == string.Empty && !hasCol))
                {
                    object result = entry.Value.RunAction("raceChanged", new object[] { requester });
                    if (result != null)
                    {
                        subscriptionList.Add((SimpleEvent)result);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private static string ClassLevel(StatSource target, string id)
        {
            object pc = target.RunAction("getClass", new object[] { id });
            if (pc is GDTKClass @class)
            {
                return @class.level.ToString();
            }

            return "0";
        }

        private static string HasAttribute(StatSource source, string request)
        {
            object pc = source.RunAction("hasAttribute", new object[] { request });
            if (pc != null)
            {
                return (bool)pc ? "1" : "0";
            }

            return "0";
        }

        private static string HasClass(StatSource target, string id)
        {
            object pc = target.RunAction("getClass", new object[] { id });
            if (pc is GDTKClass)
            {
                return "1";
            }

            return "0";
        }

        private static string HasPerk(StatSource target, string id)
        {
            object pc = target.RunAction("hasPerk", new object[] { id });
            if (pc != null)
            {
                return (bool)pc ? "1" : "0";
            }

            return "0";
        }

        private static string IsRace(StatSource target, string id)
        {
            object pc = target.RunAction("isRace", new object[] { id });
            if (pc != null)
            {
                return (bool)pc ? "1" : "0";
            }

            return "0";
        }

        private static void RegisterFunctions()
        {
            m_functionList.Add("classLevel", ClassLevel);
            m_functionList.Add("hasAttrib", HasAttribute);
            m_functionList.Add("hasClass", HasClass);
            m_functionList.Add("hasPerk", HasPerk);
            m_functionList.Add("isRace", IsRace);
        }

        private static void RegisterFunctionSubscriptions()
        {
            m_subscriptionList.Add("classLevel", SubClassLevel);
            m_subscriptionList.Add("hasAttrib", SubHasAttribute);
            m_subscriptionList.Add("hasClass", SubHasClass);
            m_subscriptionList.Add("hasPerk", SubHasPerk);
            m_subscriptionList.Add("isRace", SubIsRace);
        }

        #endregion

    }
}
#endif