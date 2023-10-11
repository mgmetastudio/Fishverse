#if GDTK
using System.Collections.Generic;

namespace NullSave.GDTK
{
    public class StatsDefinition : ToolDefinition
    {
        public override List<string> GetSymbols
        {
            get
            {
                return new List<string>() { "GDTK_Stats2" };
            }
        }
    }
}
#else
using UnityEditor;

namespace NullSave.GDTK.Stats
{
    [InitializeOnLoad]
    public class GDTKStatsWarning
    {

        static GDTKStatsWarning()
        {
            EditorUtility.DisplayDialog("GDTK Stats", "The free Game Developer Toolkit (GDTK) must be installed to use this product.", "OK");
        }

    }
}
#endif