using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [RequireComponent(typeof(StatsCog))]
    public class StatsTurnGameCharacter : MonoBehaviour
    {

        #region Variables

        public string displayName;
        public bool isNPC = true;
        public string playerOrder = "1 + rnd_i(20)";
        public GameObject turnIndicator;
        public GameObject targetIndicator;
        public GameObject targetLockedIndicator;

        public string interactRadius = "5";
        public LayerMask interactLayer;
        public string interactRequirement = "1 > 0";
        public List<string> interactCommands;

        private GameObject spawnedIndicator;
        private bool showInteractables, showTarget, showTargetLocked;

        #endregion

        #region Properties

        public StatAttackManager AttackManager { get; private set; }

        public float CurrentOrder { get; set; }

        public StatsTurnGameManager GameManager { get; private set; }

        public List<InteractableTile> InteratablesInRange
        {
            get
            {
                List<InteractableTile> interactables = new List<InteractableTile>();

                if(!StatSource.EvaluateCondition(interactRequirement))
                {
                    return interactables;
                }

                float radius = StatSource.GetExpressionValue(interactRadius);
                Vector3 castFrom = transform.position + new Vector3(0, 0.5f, 0);
                RaycastHit[] hits = Physics.SphereCastAll(castFrom, radius, transform.up, radius, interactLayer);
                InteractableTile tile;

                foreach(RaycastHit hit in hits)
                {
                    tile = hit.transform.gameObject.GetComponentInChildren<InteractableTile>();
                    if(tile == null) tile = hit.transform.gameObject.GetComponentInParent<InteractableTile>();

                    if(tile != null && !interactables.Contains(tile))
                    {
                        if (tile.maxInteractDistance >= Vector3.Distance(transform.position, hit.transform.position))
                        {
                            interactables.Add(tile);
                        }
                    }
                }

                return interactables;
            }
        }

        public float PlayOrder
        {
            get
            {
                return StatSource.GetExpressionValue(playerOrder);
            }
        }

        public bool ShowInteractables
        {
            get { return showInteractables; }
            set
            {
                if (showInteractables == value) return;
                showInteractables = value;

                foreach(InteractableTile tile in InteratablesInRange)
                {
                    tile.ShowInteractMarker = value;
                }
            }
        }

        public bool ShowTarget
        {
            get { return showTarget; }
            set
            {
                if (showTarget == value) return;
                showTarget = value;
                if (spawnedIndicator != null) Destroy(spawnedIndicator);
                if(value)
                {
                    showTargetLocked = false;
                    spawnedIndicator = Instantiate(targetIndicator, transform);
                }
            }
        }

        public bool ShowTargetLocked
        {
            get { return showTargetLocked; }
            set
            {
                if (showTargetLocked == value) return;
                showTargetLocked = value;
                if (spawnedIndicator != null) Destroy(spawnedIndicator);
                if (value)
                {
                    showTarget = false;
                    spawnedIndicator = Instantiate(targetLockedIndicator, transform);
                }
            }
        }

        public StatsCog StatSource { get; private set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            AttackManager = GetComponent<StatAttackManager>();
            StatSource = GetComponent<StatsCog>();
        }

        private void Start()
        {
            GameManager = FindObjectOfType<StatsTurnGameManager>();
            GameManager.AddCharacter(this);
            GameManager.onTurnChanged.AddListener(UpdateTurn);
        }

        #endregion

        #region Public Methods

        public List<StatsTurnGameCharacter> FindTargetsInRange(float minRange, float maxRange, LayerMask layerMask)
        {
            Vector3 yOffset = new Vector3(0, 0.5f, 0);
            Vector3 castFrom = transform.position + yOffset;
            List<StatsTurnGameCharacter> targets = new List<StatsTurnGameCharacter>();
            RaycastHit[] hits = Physics.SphereCastAll(castFrom, maxRange, transform.up, maxRange, layerMask);
            StatsTurnGameCharacter target;
            foreach (RaycastHit hit in hits)
            {
                target = hit.transform.gameObject.GetComponentInChildren<StatsTurnGameCharacter>();
                if(target == null) target = hit.transform.gameObject.GetComponentInParent<StatsTurnGameCharacter>();
                if(target != null && Vector3.Distance(transform.position, hit.transform.position) >= minRange)
                {
                    targets.Add(target);
                }
            }

            return targets;
        }

        public bool Interact(GameObject go)
        {
            InteractableTile tile;

            tile = go.GetComponentInChildren<InteractableTile>();
            if (tile == null) tile = go.GetComponentInParent<InteractableTile>();
            if (tile == null) return false;

            if(tile.Interact(StatSource))
            {
                foreach(string command in interactCommands)
                {
                    StatSource.SendCommand(command);
                }
                return true;
            }

            return false;
        }

        #endregion

        #region Private Methods

        private void UpdateTurn()
        {
            if (spawnedIndicator != null) Destroy(spawnedIndicator);
            if (GameManager.ActivePlayer == this)
            {
                if (turnIndicator != null)
                {
                    spawnedIndicator = Instantiate(turnIndicator, transform);
                }
            }
        }

        #endregion

    }
}