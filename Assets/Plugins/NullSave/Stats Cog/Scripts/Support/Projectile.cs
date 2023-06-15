using System.Collections;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [DefaultExecutionOrder(1)]
    public class Projectile : MonoBehaviour
    {

        #region Variables

        public float speed = 1f;
        public bool autoLaunch = false;
        public float maxLife = 6;

        private bool launched;
        private float elapsed;
        private DamageDealer[] dealers;
        private bool damageDealt;

        public bool removeKinematicOnLaunch;
        public bool destoryOnCollision = true;
        public LayerMask collisionLayer = 1;

        private StatsCog statsCog;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            dealers = GetComponentsInChildren<DamageDealer>();
            foreach (DamageDealer dealer in dealers)
            {
                if (statsCog == null) statsCog = dealer.StatsSource;
                dealer.onDamageDealt.AddListener(DamageDealt);
            }
        }

        private void Start()
        {
            if (autoLaunch)
            {
                StartCoroutine("ProjectileControl");
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + (transform.forward * speed));
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (destoryOnCollision && (collisionLayer == (collisionLayer | (1 << collision.gameObject.layer))))
            {
                damageDealt = true;
                elapsed = maxLife;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (destoryOnCollision && (collisionLayer == (collisionLayer | (1 << collision.gameObject.layer))))
            {
                damageDealt = true;
                elapsed = maxLife;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Launch()
        {
            transform.SetParent(null);
            if(removeKinematicOnLaunch)
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = false;

                Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
                if (rb2D != null) rb2D.isKinematic = false;
            }
            StartCoroutine("ProjectileControl");
        }

        #endregion

        #region Private Methods

        private void DamageDealt(float amount, DamageReceiver target)
        {
            if (amount == 0 || target.StatsParent == statsCog) return;
            damageDealt = true;
        }

        private IEnumerator ProjectileControl()
        {
            if (!launched)
            {
                launched = true;

                while (!damageDealt && elapsed < maxLife)
                {
                    transform.position += transform.forward * speed * Time.deltaTime;
                    elapsed += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                Destroy(gameObject);
            }
        }

        #endregion

    }
}