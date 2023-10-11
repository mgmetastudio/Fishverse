#if GDTK
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class MenuItems
    {

        [MenuItem("GameObject/GDTK/Stats/Stats Database", false)]
        private static void AddStatsDatabase()
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<StatsDatabase>() == null)
            {
                Selection.activeGameObject.AddComponent<StatsDatabase>();
                return;
            }

            GameObject go = new GameObject("Stats Database");
            go.AddComponent<StatsDatabase>();
            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/GDTK/Stats/Global Stats", false)]
        private static void AddGlobalStats()
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<GlobalStats>() == null)
            {
                Selection.activeGameObject.AddComponent<GlobalStats>();
                return;
            }

            GameObject go = new GameObject("Global Stats");
            go.AddComponent<GlobalStats>();
            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/GDTK/Stats/Basic Stats", false)]
        private static void AddBasicStats()
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<BasicStats>() == null)
            {
                Selection.activeGameObject.AddComponent<BasicStats>();
                return;
            }

            GameObject go = new GameObject("Basic Stats");
            go.AddComponent<BasicStats>();
            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/GDTK/Stats/Stats and Effects", false)]
        private static void AddStatsAndEffects()
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<StatsAndEffects>() == null)
            {
                Selection.activeGameObject.AddComponent<StatsAndEffects>();
                return;
            }

            GameObject go = new GameObject("Stats and Effects");
            go.AddComponent<StatsAndEffects>();
            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/GDTK/Stats/NPC Stats", false)]
        private static void AddNPCStats()
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<NPCStats>() == null)
            {
                Selection.activeGameObject.AddComponent<NPCStats>();
                return;
            }

            GameObject go = new GameObject("NPC Stats");
            go.AddComponent<NPCStats>();
            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/GDTK/Stats/Player Character Stats", false)]
        private static void AddPlayerCharacterStats()
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<PlayerCharacterStats>() == null)
            {
                Selection.activeGameObject.AddComponent<PlayerCharacterStats>();
                return;
            }

            GameObject go = new GameObject("Player Character Stats");
            go.AddComponent<PlayerCharacterStats>();
            Selection.activeGameObject = go;
        }


    }
}
#endif