using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class InventoryLoadouts : MonoBehaviour
    {

        #region Variables

        public List<Loadout> availableLoadouts;


        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (Inventory == null)
            {
                Inventory = GetComponent<InventoryCog>();
                if (Inventory == null)
                {
                    Transform t = transform;
                    while (t.parent != null)
                    {
                        t = t.parent;
                        Inventory = t.GetComponent<InventoryCog>();
                        if (Inventory != null) return;
                    }

                    Debug.LogError("No Inventory Cog found on " + name + " or parent");
                }
            }
        }

        #endregion

        #region Public Methods

        public void ActivateLoadout(int index, bool overrideForce = false)
        {
            if(index >= 0 && index < availableLoadouts.Count)
            {
                Loadout loadout = availableLoadouts[index];

                if (loadout.clearInventory)
                {
                    Inventory.ClearInventory();
                }
                else
                {
                    foreach (EquipPoint point in Inventory.EquipPoints)
                    {
                        point.UnequipItem();
                    }
                }

                for (int i=0; i < loadout.items.Count; i++)
                {
                    InventoryItem item = Inventory.GetItemFromInventory(loadout.items[i].item);
                    if(item == null)
                    {
                        if (loadout.items[i].forceItem || overrideForce)
                        {
                            Inventory.AddToInventory(loadout.items[i].item, loadout.items[i].count);
                            item = Inventory.GetItemFromInventory(loadout.items[i].item);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    
                    if(loadout.items[i].autoEquip)
                    {
                        if(!string.IsNullOrEmpty(loadout.items[i].equipPointId))
                        {
                            Inventory.EquipItem(item, loadout.items[i].equipPointId);
                        }
                        else
                        {
                            Inventory.EquipItem(item);
                        }
                    }
                }

            }
        }

        public void ActivateLoadout(Loadout loadout, bool overrideForce = false)
        {
            for(int i=0; i < availableLoadouts.Count; i++)
            {
                if(availableLoadouts[i] == loadout)
                {
                    ActivateLoadout(i, overrideForce);
                    return;
                }
            }

            Debug.Log("Loadout not found");
        }

        public void Load(Stream stream)
        {
            availableLoadouts.Clear();
            int count = stream.ReadInt();
            for(int i=0; i < count; i++)
            {
                Loadout loadout = new Loadout();
                loadout.Load(Inventory, stream);
                availableLoadouts.Add(loadout);
            }
        }

        public void Save(Stream stream)
        {
            stream.WriteInt(availableLoadouts.Count);
            foreach(Loadout loadout in availableLoadouts)
            {
                loadout.Save(stream);
            }
        }

        #endregion

    }
}