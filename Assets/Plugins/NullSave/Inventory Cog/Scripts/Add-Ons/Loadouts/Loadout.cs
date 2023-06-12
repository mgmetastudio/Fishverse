using System;
using System.Collections.Generic;
using System.IO;

namespace NullSave.TOCK.Inventory
{
    [Serializable]
    public class Loadout
    {

        #region Variables

        public string displayName;
        public bool clearInventory;
        public BooleanSource unlockedSource = BooleanSource.Static;
        public bool unlocked = true;
        public string statUnlockedName;

        public List<LoadoutItem> items = new List<LoadoutItem>();
        public List<LoadoutSkill> skills = new List<LoadoutSkill>();

        #endregion

        #region Public Methods

        public void Load(InventoryCog inventory, Stream stream)
        {
            displayName = stream.ReadStringPacket();
            clearInventory = stream.ReadBool();
            unlockedSource = (BooleanSource)stream.ReadInt();
            unlocked = stream.ReadBool();
            statUnlockedName = stream.ReadStringPacket();

            items.Clear();
            int count = stream.ReadInt();
            for(int i=0; i < count; i++)
            {
                LoadoutItem item = new LoadoutItem();
                item.Load(inventory, stream);
                items.Add(item);
            }

            skills.Clear();
            count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                LoadoutSkill skill = new LoadoutSkill();
                skill.Load(inventory, stream);
                skills.Add(skill);
            }
        }

        public void Save(Stream stream)
        {
            stream.WriteStringPacket(displayName);
            stream.WriteBool(clearInventory);
            stream.WriteInt((int)unlockedSource);
            stream.WriteBool(unlocked);
            stream.WriteStringPacket(statUnlockedName);

            stream.WriteInt(items.Count);
            foreach(LoadoutItem item in items)
            {
                item.Save(stream);
            }

            stream.WriteInt(skills.Count);
            foreach (LoadoutSkill skill in skills)
            {
                skill.Save(stream);
            }
        }

        #endregion

    }
}