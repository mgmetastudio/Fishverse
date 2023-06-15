using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class ActionButtonList : MonoBehaviour
    {

        #region Enumerations

        public enum TargetSelectMode
        {
            ByCharacter = 0,
            ByTile = 1
        }

        #endregion

        #region Variables

        public ActionButton buttonPrefab;
        public bool showMovement = true;
        public bool showInteract = true;
        public bool showDisabled = false;
        public bool showInGroups = false;
        public TargetSelectMode targetMode;

        private TileMovingCharacter tmc;
        private StatsTurnGameManager manager;
        private List<ActionButton> loaded = new List<ActionButton>();
        private List<StatsTurnGameCharacter> possibleTargets, lockedTargets;

        private bool updateRequested;
        private bool cancelOp;
        private StatAttack selAction;
        private ActionButton confirm;

        #endregion

        #region Properties

        public StatsTurnGameManager Manager
        {
            get { return manager; }
            set
            {
                if (manager != null)
                {
                    manager.onTurnChanged.RemoveListener(UpdateUI);
                }
                manager = value;
                if (manager != null)
                {
                    manager.onTurnChanged.AddListener(UpdateUI);
                }
            }
        }

        #endregion

        #region Public Methods

        public virtual void Clear()
        {
            foreach (ActionButton button in loaded)
            {
                Destroy(button.gameObject);
            }
            loaded.Clear();
        }

        public virtual void UpdateUI()
        {
            if (!updateRequested)
            {
                updateRequested = true;
                StartCoroutine(DoUpdateUI());
            }
        }

        #endregion

        #region Private Methods

        private void CommitAction()
        {
            List<StatsCog> stats = new List<StatsCog>();
            foreach (StatsTurnGameCharacter target in lockedTargets)
            {
                stats.Add(target.GetComponent<StatsCog>());
            }

            selAction.Activate(Manager.ActivePlayer.AttackManager, stats);

            foreach (StatsTurnGameCharacter target in possibleTargets)
            {
                target.ShowTarget = false;
                target.ShowTargetLocked = false;
            }

            cancelOp = true;
            manager.ActivePlayer.ShowInteractables = false;
            UpdateUI();
        }

        private IEnumerator DoUpdateUI()
        {
            ActionButton ab;

            yield return new WaitForEndOfFrame();

            Clear();
            if (!manager.ActivePlayer.isNPC)
            {
                if (showMovement)
                {
                    // Movement
                    tmc = manager.ActivePlayer.gameObject.GetComponentInChildren<TileMovingCharacter>();
                    if (tmc != null)
                    {
                        if (tmc.Movement.CurrentValue > 0 || showDisabled)
                        {
                            ab = Instantiate(buttonPrefab, transform);
                            ab.ActionName = "Move (" + tmc.Movement.CurrentValue + ")";
                            ab.onClick.AddListener(StartMovement);
                            loaded.Add(ab);
                            if (tmc.Movement.CurrentValue <= 0)
                            {
                                ab.onDisable?.Invoke();
                            }
                        }
                    }
                }

                if (showInteract)
                {
                    bool hasInRange = manager.ActivePlayer.InteratablesInRange.Count > 0;
                    if (hasInRange || showDisabled)
                    {
                        ab = Instantiate(buttonPrefab, transform);
                        ab.ActionName = "Interact";
                        ab.onClick.AddListener(StartInteract);
                        loaded.Add(ab);
                        if (!hasInRange)
                        {
                            ab.onDisable?.Invoke();
                        }
                    }
                }

                if (showInGroups)
                {
                    bool isAvail;
                    List<string> categories = new List<string>();
                    foreach (StatAttack attack in Manager.ActivePlayer.AttackManager.availableAttacks)
                    {
                        isAvail = attack.IsAvailable(Manager.ActivePlayer.AttackManager);

                        if (!categories.Contains(attack.category) && (isAvail || showDisabled))
                        {
                            categories.Add(attack.category);
                        }
                    }

                    categories.Sort();
                    foreach (string cat in categories)
                    {
                        ab = Instantiate(buttonPrefab, transform);
                        ab.ActionName = cat;
                        ab.onClick.AddListener((ActionButton button) => LoadGroup(button.ActionName));
                        loaded.Add(ab);
                    }
                }
                else
                {

                }

                // End Turn
                ab = Instantiate(buttonPrefab, transform);
                ab.ActionName = "End Turn";
                ab.onClick.AddListener((ActionButton button) => Manager.EndTurn());
                loaded.Add(ab);
            }

            updateRequested = false;
        }

        private void LoadGroup(string groupName)
        {
            ActionButton ab;

            Clear();

            List<StatAttack> actions = new List<StatAttack>();
            foreach (StatAttack attack in Manager.ActivePlayer.AttackManager.availableAttacks)
            {
                if (!actions.Contains(attack) && attack.category == groupName)
                {
                    actions.Add(attack);
                }
            }

            bool isAvail;
            actions = actions.OrderBy(_ => _.displayName).ToList();
            foreach (StatAttack attack in actions)
            {
                isAvail = attack.IsAvailable(Manager.ActivePlayer.AttackManager);
                if (showDisabled || isAvail)
                {
                    ab = Instantiate(buttonPrefab, transform);
                    ab.ActionName = attack.displayName;
                    ab.Action = attack;
                    if (!isAvail) ab.onDisable?.Invoke();
                    ab.onClick.AddListener((ActionButton btn) => PerformAction(btn.Action));
                    loaded.Add(ab);
                }
            }

            ab = Instantiate(buttonPrefab, transform);
            ab.ActionName = "Cancel";
            ab.onClick.AddListener((ActionButton button) => { cancelOp = true; manager.ActivePlayer.ShowInteractables = false; UpdateUI(); });
            loaded.Add(ab);
        }

        private IEnumerator MonitorInteract()
        {
            while (!cancelOp)
            {
                yield return new WaitForEndOfFrame();

                while (!Input.GetMouseButtonDown(0))
                    yield return null;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 500, tmc.collisionMask))
                {
                    if (manager.ActivePlayer.Interact(hit.transform.gameObject))
                    {
                        cancelOp = true;
                        manager.ActivePlayer.ShowInteractables = false;
                        UpdateUI();
                    }
                }
            }
        }

        private IEnumerator MonitorMovement()
        {
            while (!cancelOp)
            {
                yield return new WaitForEndOfFrame();

                while (!Input.GetMouseButtonDown(0))
                    yield return null;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 500, tmc.collisionMask))
                {
                    if (tmc.MoveTo(hit))
                    {
                        tmc.ShowMovement = false;
                        cancelOp = true;
                        Clear();
                        StartCoroutine(WaitForMovement());
                    }
                }
            }
        }

        private IEnumerator MonitorTarget()
        {
            while (!cancelOp)
            {
                yield return new WaitForEndOfFrame();

                while (!Input.GetMouseButtonDown(0))
                    yield return null;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 500, selAction.targetLayer))
                {
                    if (selAction.targetsArea)
                    {
                        //!! TODO
                    }
                    else
                    {
                        switch(targetMode)
                        {
                            case TargetSelectMode.ByCharacter:
                                StatsTurnGameCharacter tc = hit.transform.GetComponentInChildren<StatsTurnGameCharacter>();
                                if(tc == null) tc = hit.transform.GetComponentInParent<StatsTurnGameCharacter>();
                                if(tc!=null)
                                {
                                    if(lockedTargets.Contains(tc))
                                    {
                                        tc.ShowTarget = true;
                                        lockedTargets.Remove(tc);
                                    }
                                    else
                                    {
                                        tc.ShowTargetLocked = true;
                                        lockedTargets.Add(tc);

                                        if(lockedTargets.Count > selAction.maxTargets)
                                        {
                                            lockedTargets[0].ShowTarget = true;
                                            lockedTargets.RemoveAt(0);
                                        }
                                    }

                                    if (lockedTargets.Count >= selAction.minTargets)
                                    {
                                        confirm.onEnable?.Invoke();
                                    }
                                    else
                                    {
                                        confirm.onDisable?.Invoke();
                                    }
                                }
                                break;
                            case TargetSelectMode.ByTile:
                                break;
                        }
                    }
                }
            }
        }

        private void PerformAction(StatAttack attack)
        {
            ActionButton ab;

            selAction = attack;
            Clear();

            if (attack.targetsArea)
            {
                // AOE
                Debug.Log("Select area for effect");
            }
            else
            {
                Debug.Log("Select " + attack.minTargets + " - " + attack.maxTargets);
                switch (targetMode)
                {
                    case TargetSelectMode.ByCharacter:
                        possibleTargets = Manager.ActivePlayer.FindTargetsInRange(attack.minRange, attack.maxRange, attack.targetLayer);

                        if(!attack.canTargetSelf)
                        {
                            possibleTargets.Remove(Manager.ActivePlayer);
                        }

                        foreach(StatsTurnGameCharacter target in possibleTargets)
                        {
                            target.ShowTarget = true;
                        }

                        Debug.Log("Found " + possibleTargets.Count + " possible targets");
                        break;
                    case TargetSelectMode.ByTile:
                        break;
                }
            }

            confirm = Instantiate(buttonPrefab, transform);
            confirm.ActionName = "Confirm";
            confirm.onDisable?.Invoke();
            confirm.onClick.AddListener((ActionButton button) => { CommitAction(); });
            loaded.Add(confirm);

            ab = Instantiate(buttonPrefab, transform);
            ab.ActionName = "Cancel";
            ab.onClick.AddListener((ActionButton button) => { cancelOp = true; manager.ActivePlayer.ShowInteractables = false; UpdateUI(); });
            loaded.Add(ab);

            lockedTargets = new List<StatsTurnGameCharacter>();
            cancelOp = false;
            StartCoroutine(MonitorTarget());
        }

        private void StartMovement(ActionButton button)
        {
            Clear();
            ActionButton ab = Instantiate(buttonPrefab, transform);
            ab.ActionName = "Cancel";
            ab.onClick.AddListener((ActionButton btn) => { cancelOp = true; tmc.ShowMovement = false; UpdateUI(); });
            loaded.Add(ab);

            cancelOp = false;
            tmc.ShowMovement = true;

            StartCoroutine(MonitorMovement());
        }

        private void StartInteract(ActionButton button)
        {
            Clear();
            ActionButton ab = Instantiate(buttonPrefab, transform);
            ab.ActionName = "Cancel";
            ab.onClick.AddListener((ActionButton btn) => { cancelOp = true; manager.ActivePlayer.ShowInteractables = false; UpdateUI(); });
            loaded.Add(ab);

            cancelOp = false;
            manager.ActivePlayer.ShowInteractables = true;

            StartCoroutine(MonitorInteract());
        }

        private IEnumerator WaitForMovement()
        {
            while (tmc.IsMoving)
            {
                yield return new WaitForEndOfFrame();
            }

            UpdateUI();
        }

        #endregion

    }
}