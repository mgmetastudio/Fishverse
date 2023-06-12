using NullSave.TOCK.Stats;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class ProjectileWeapon : MonoBehaviour
    {

        #region Variables

        public Transform spawnPoint;
        public bool consumeAmmoOnFire = true;
        public bool alignOnLaunch;

        private Projectile spawnedObj;
        private InventoryItem ammo;

        #endregion

        #region Properties

        public InventoryItem Item { get; set; }

        #endregion

        #region Public Methods

        public virtual void FireProjectile()
        {
            if (spawnedObj == null) SpawnProjectile();

            if (consumeAmmoOnFire)
            {
                ammo = Item.InventoryCog.GetSelectedAmmo(Item.ammoType);
                if(ammo != null)
                {
                    Item.InventoryCog.RemoveItem(ammo, Item.ammoPerUse);
                }
            }

            if(alignOnLaunch)
            {
                spawnedObj.transform.SetParent(null);
                spawnedObj.transform.rotation = ammo.InventoryCog.gameObject.transform.rotation;
            }

            spawnedObj.Launch();
            spawnedObj = null;
        }

        public void SpawnProjectile()
        {
            if (spawnedObj != null) return;
            ammo = Item.InventoryCog.GetSelectedAmmo(Item.ammoType);
            if (ammo.projectile == null)
            {
                Debug.LogWarning("No projectile assigned to ammo " + ammo.name);
            }
            else
            {
                spawnedObj = Instantiate(ammo.projectile, spawnPoint);
            }

            DamageDealer[] dds = spawnedObj.gameObject.GetComponentsInChildren<DamageDealer>();
            foreach(DamageDealer dd in dds)
            {
                dd.StatsSource = ammo.InventoryCog.StatsCog;
            }

        }

        #endregion

    }
}