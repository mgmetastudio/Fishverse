using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Stats
{
    [HierarchyIcon("weapon", false)]
    public class DamageDealer : MonoBehaviour
    {

        #region Variables

        public float minBetweenDamage = 0.1f;
        public List<Damage> damage;
        public bool overrideDamageSource;
        public GameObject damageSource;

        public List<StatEffect> effects;
        public DamageDealt onDamageDealt;
        public UnityEvent onColliderEnabled, onColliderDisabled;

        private Dictionary<DamageReceiver, float> waitList;

        #endregion

        #region Properties

        public virtual StatsCog StatsSource { get; set; }

        #endregion

        #region Unity Methods

        private void OnTriggerEnter(Collider other)
        {
            ApplyDamage(other.gameObject);
        }

        private void OnColliderEnter(Collider other)
        {
            ApplyDamage(other.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            ApplyDamage(other.gameObject);
        }

        private void OnColliderEnter2D(Collider2D other)
        {
            ApplyDamage(other.gameObject);
        }

        #endregion

        #region Public Methods

        public virtual void DealDamage(DamageReceiver receiver)
        {
            GameObject source = overrideDamageSource ? damageSource : gameObject;
            float applieDamage = receiver.TakeDamage(this, source);
            onDamageDealt?.Invoke(applieDamage, receiver);
        }

        public virtual void SetColliderEnabled(bool enabled)
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null) collider.enabled = enabled;

            Collider2D collider2d = GetComponent<Collider2D>();
            if (collider2d != null) collider2d.enabled = enabled;

            if (enabled)
            {
                onColliderEnabled?.Invoke();
            }
            else
            {
                onColliderDisabled.Invoke();
            }
        }

        #endregion

        #region Private Methods

        private void ApplyDamage(GameObject otherObject)
        {
            if (waitList == null) waitList = new Dictionary<DamageReceiver, float>();

            // Check for shield first
            DamageShield shield = otherObject.GetComponentInChildren<DamageShield>();
            if (shield != null && shield.GetColliderState())
            {
                if (shield.StatsSource != StatsSource)
                {
                    Destroy(gameObject);
                }
            }

            // Check for receiver
            DamageReceiver receiver = otherObject.GetComponentInChildren<DamageReceiver>();
            if (receiver != null)
            {
                // Check for self
                if (receiver.gameObject == gameObject) return;
                StatsCog statsCog = receiver.gameObject.GetComponent<StatsCog>();
                if (statsCog != null && statsCog == StatsSource) return;

                if (waitList.ContainsKey(receiver))
                {
                    if (Time.time >= waitList[receiver])
                    {
                        DealDamage(receiver);
                        waitList[receiver] = Time.time + minBetweenDamage;
                    }
                }
                else
                {
                    DealDamage(receiver);
                    waitList.Add(receiver, Time.time + minBetweenDamage);
                }
            }
        }

        #endregion

    }
}