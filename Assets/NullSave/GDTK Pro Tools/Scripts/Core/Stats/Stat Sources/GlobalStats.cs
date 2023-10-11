#if GDTK
using NullSave.GDTK.JSON;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [DefaultExecutionOrder(-590)]
    public class GlobalStats : StatsAndEffects
    {

        #region Unity Methods

        public override void Awake()
        {
            ToolRegistry.RegisterComponent(this, "GlobalStats");
            Broadcaster.SubscribeToPublic(this);

            base.Awake();
        }

        private void OnDestroy()
        {
            ToolRegistry.RemoveComponent(this);
        }

        #endregion

        #region Public Methods

        public override string JSONExport()
        {
            jsonStatsList result = new jsonStatsList();

            foreach (var entry in stats)
            {
                result.stats.Add(entry.Value);
            }

            return SimpleJson.ToJSON(result);
        }

        public override void JSONImport(string json, bool clearExisting)
        {
            if (clearExisting) ClearStats();

            jsonStatsList result = SimpleJson.FromJSON<jsonStatsList>(json);
            foreach (GDTKStat stat in result.stats)
            {
                AddStat(stat);
            }
        }

        #endregion

        #region Broadcast Methods

        public override void BroadcastReceived(object sender, string channel, string message, object[] args) { }

        public override void PublicBroadcastReceived(object sender, string message)
        {
            StatsDebugManager.GlobalStatsRequest(this, message);
        }

        #endregion

    }
}
#endif