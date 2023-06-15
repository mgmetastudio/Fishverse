using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class DamageShield : MonoBehaviour
    {

        #region Variables

        public string shieldId = "Shield";

        #endregion

        #region Properties

        public virtual StatsCog StatsSource { get; set; }

        #endregion

        #region Public Methods

        public bool GetColliderState()
        {
            Collider collider = GetComponentInChildren<Collider>();
            if (collider != null) return collider.enabled;

            Collider2D collider2d = GetComponentInChildren<Collider2D>();
            if (collider2d != null) return collider2d.enabled;

            return false;
        }

        public void SetCollider(bool state)
        {
            Collider collider = GetComponentInChildren<Collider>();
            if (collider != null) collider.enabled = state;

            Collider2D collider2d = GetComponentInChildren<Collider2D>();
            if (collider2d != null) collider2d.enabled = state;
        }

        #endregion


    }
}