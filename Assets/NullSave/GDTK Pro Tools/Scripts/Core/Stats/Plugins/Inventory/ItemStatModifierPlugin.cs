#if GDTK_Inventory2
using NullSave.GDTK.Inventory;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class StatModifierPlugin : ItemPlugin
    {

        #region Fields

        public List<GDTKStatModifier> statModifiers;
        private List<GDTKStatModifier> activeMods;
        private BasicStats statSource;

        #endregion

        #region Properties

        public override string title { get { return "Add Stat Modifiers"; } }

        public override string description { get { return "Add Stat Modifiers when item is equipped or consumed."; } }

        public override Texture2D icon { get { return GetResourceImage("icons/stats"); } }

        #endregion

        #region Constructor

        public StatModifierPlugin()
        {
            statModifiers = new List<GDTKStatModifier>();
        }

        #endregion

        #region Overrides

        public override void Initialize(Inventory.Inventory host, GDTKItem item)
        {
            //!!
            //base.Initialize(host, item);

            //activeMods = new List<GDTKStatModifier>();
            //statSource = host.statSource;

            //if (statSource == null)
            //{
            //    StringExtensions.LogError(host.name, "StatModifierPlugin", "No stat source provided.");
            //}

            //foreach (GDTKStatModifier mod in statModifiers)
            //{
            //    activeMods.Add(mod.Clone());
            //}
        }

        //public override void PostEquipState(GDTKItem item, bool autoEquip, bool isSwapping, ref bool complete)
        //{
        //    complete = true;
        //    if (statSource == null) return;
        //    foreach (GDTKStatModifier mod in activeMods)
        //    {
        //        statSource.AddStatModifier(mod, item);
        //    }
        //}

        //public override void PostUnEquipState(GDTKItem item, bool autoEquip, bool isSwapping, ref bool complete)
        //{
        //    complete = true;
        //    if (statSource == null) return;
        //    statSource.RemoveStatModifiersFromSource(item);
        //}

        #endregion

    }
}
#endif