#if GDTK
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class StatArea : MonoBehaviour
    {

        #region Fields

        [Tooltip("Method used to detect hit")] public DetectionType detection;
        [Tooltip("Radius")] public float radius;
        [Tooltip("Volume")] public Vector3 extends;
        [Tooltip("Angle of cone")] public float coneAngle;
        [Tooltip("Range in units")] public float range;
        [Tooltip("Raycast layermask")] public LayerMask layerMask;
        [Tooltip("Reapply actions every update")] public bool applyContinually;

        [Tooltip("Action(s) to take on interact")] public InteractableActions actions;
        [Tooltip("Id of Background to set")] public string backgroundId;
        [Tooltip("Id of Race to set")] public string raceId;
        [Tooltip("Id of Class to add")] public string addClassId;
        [Tooltip("Id of Class to remove")] public string removeClassId;
        [Tooltip("Id of Status Condition to add")] public string addStatusConditionId;
        [Tooltip("Id of Status Condition to remove")] public string removeStatusConditionId;
        [Tooltip("List of Stat Modifiers to add")] public List<GDTKStatModifier> statModifiers;
        [Tooltip("Id of Status Effect to add")] public string addStatusEffectId;
        [Tooltip("Id of Status Effect to remove")] public string removeStatusEffectId;
        [Tooltip("Object to Activate")] public GameObject activateTarget;
        [Tooltip("Prefab to Spawn")] public GameObject prefab;
        [Tooltip("Parent Transform for spawned object")] public Transform parent;
        [Tooltip("Id of Event to raise")] public string raiseEventId;

        [Tooltip("Enable Gizmos")] public bool visualize;
        [Tooltip("Number of spheres to draw representing cone")] public int steps;

        private readonly Color noHit = new Color(0, 1, 0, 0.4f);
        private readonly Color hit = new Color(1, 0, 0, 0.4f);
        private readonly Color noHitSolid = new Color(0, 1, 0, 0.2f);
        private readonly Color hitSolid = new Color(1, 0, 0, 0.2f);
        private List<BasicStats> excludeFromHit;
        private GameObject colliderObject;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            excludeFromHit = new List<BasicStats>();
        }

        private void OnDrawGizmos()
        {
            if (!visualize) return;
            if (steps < 1) steps = 1;

            bool hasHits = excludeFromHit != null && excludeFromHit.Count > 0;

            switch (detection)
            {
                case DetectionType.SphereConeCast:

#if UNITY_EDITOR
                    Gizmos.color = hasHits ? hitSolid : noHitSolid;
                    float percent;
                    Gizmos.DrawSphere(transform.position, radius / (steps + 1) / 2);
                    for (int i = 0; i < steps; i++)
                    {
                        percent = (float)(i + 1) / steps;
                        Gizmos.DrawSphere(transform.position + (transform.forward * (range * percent)), radius * percent);
                    }
#endif

                    break;
                case DetectionType.CubeCast:
                    Gizmos.color = hasHits ? hitSolid : noHitSolid;
                    Gizmos.DrawCube(transform.position, extends);
                    Gizmos.color = hasHits ? hit : noHit;
                    Gizmos.DrawWireCube(transform.position, extends);
                    break;
                case DetectionType.RayCast:
                    Gizmos.color = hasHits ? hit : noHit;
                    Gizmos.DrawLine(transform.position, transform.position + (transform.forward * range));
                    break;
                case DetectionType.SphereCast:
                    Gizmos.color = hasHits ? hitSolid : noHitSolid;
                    Gizmos.DrawSphere(transform.position, radius);
                    Gizmos.color = hasHits ? hit : noHit;
                    Gizmos.DrawWireSphere(transform.position, radius);
                    break;
            }

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (detection != DetectionType.ColliderHit) return;
            colliderObject = collision.gameObject;
            ColliderHit(colliderObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (detection != DetectionType.ColliderHit) return;
            colliderObject = collision.gameObject;
            ColliderHit(colliderObject);
        }

        private void OnCollisionExit(Collision collision)
        {
            colliderObject = null;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            colliderObject = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (detection != DetectionType.ColliderHit) return;
            colliderObject = other.gameObject;
            ColliderHit(colliderObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (detection != DetectionType.ColliderHit) return;
            colliderObject = collision.gameObject;
            ColliderHit(colliderObject);
        }

        private void OnTriggerExit(Collider other)
        {
            colliderObject = null;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            colliderObject = null;
        }

        private void Reset()
        {
            radius = 1;
            extends = new Vector3(1, 1, 1);
            coneAngle = 45;
            range = 2;
            layerMask = 1;
        }

        private void Update()
        {
            if (Time.timeScale == 0) return;

            switch (detection)
            {
                case DetectionType.ColliderHit:
                    if (applyContinually && colliderObject != null)
                    {
                        ColliderHit(colliderObject);
                    }
                    break;
                case DetectionType.SphereConeCast:
                    ManageHits(RaycastExtensions.SphereConeCastAll(transform.position, radius, transform.forward, range, coneAngle, layerMask));
                    break;
                case DetectionType.CubeCast:
                    ManageHits(Physics.BoxCastAll(transform.position, extends / 2, transform.forward, transform.rotation, 0, layerMask));
                    break;
                case DetectionType.RayCast:
                    ManageHits(Physics.RaycastAll(transform.position, transform.forward, range, layerMask));
                    break;
                case DetectionType.SphereCast:
                    ManageHits(Physics.SphereCastAll(transform.position, radius, transform.forward, 0, layerMask));
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void ColliderHit(GameObject other)
        {
            if (other == null)
            {
                if (excludeFromHit.Count > 0) excludeFromHit.Clear();
                return;
            }

            bool isNewTarget;
            List<BasicStats> updatedExclusion = new List<BasicStats>();

            foreach (BasicStats target in other.GetComponents<BasicStats>())
            {
                if (updatedExclusion.Contains(target))
                {
                    isNewTarget = false;
                }
                else
                {
                    updatedExclusion.Add(target);

                    if (!excludeFromHit.Contains(target))
                    {
                        isNewTarget = true;
                    }
                    else
                    {
                        isNewTarget = false;
                    }
                }

                if (applyContinually || isNewTarget)
                {
                    ApplyActions(target);
                }
            }
        }

        private void ManageHits(RaycastHit[] targets)
        {
            if (targets == null)
            {
                if (excludeFromHit.Count > 0) excludeFromHit.Clear();
                return;
            }

            bool isNewTarget;
            List<BasicStats> updatedExclusion = new List<BasicStats>();

            for (int i = 0; i < targets.Length; i++)
            {
                foreach (BasicStats target in targets[i].transform.gameObject.GetComponents<BasicStats>())
                {
                    if (updatedExclusion.Contains(target))
                    {
                        isNewTarget = false;
                    }
                    else
                    {
                        updatedExclusion.Add(target);

                        if (!excludeFromHit.Contains(target))
                        {
                            isNewTarget = true;
                        }
                        else
                        {
                            isNewTarget = false;
                        }
                    }

                    if (applyContinually || isNewTarget)
                    {
                        ApplyActions(target);
                    }
                }
            }

            excludeFromHit = updatedExclusion;
        }

        #endregion

        #region Private Methods

        private void ApplyActions(BasicStats target)
        {
            PlayerCharacterStats pc = target.gameObject.GetComponent<PlayerCharacterStats>();

            if (actions.HasFlag(InteractableActions.SetBackground))
            {
                pc?.SetBackground(backgroundId);
            }

            if (actions.HasFlag(InteractableActions.RemoveBackground))
            {
                pc?.SetBackground(string.Empty);
            }

            if (actions.HasFlag(InteractableActions.AddClass))
            {
                pc?.AddClass(addClassId);
            }

            if (actions.HasFlag(InteractableActions.RemoveClass))
            {
                pc?.RemoveClass(pc.database.GetClass(removeClassId));
            }

            if (actions.HasFlag(InteractableActions.AddStatEffect))
            {
                pc?.AddEffect(addStatusEffectId);
            }

            if (actions.HasFlag(InteractableActions.RemoveStatEffect))
            {
                pc?.RemoveEffect(removeStatusEffectId);
            }

            if (actions.HasFlag(InteractableActions.AddStatModifiers))
            {
                if (target != null)
                {
                    foreach (GDTKStatModifier mod in statModifiers)
                    {
                        mod.Deactivate();
                        pc.AddStatModifier(mod);
                    }
                }
            }

            if (actions.HasFlag(InteractableActions.AddStatusCondition))
            {
                pc?.AddStatusCondition(addStatusConditionId);
            }

            if (actions.HasFlag(InteractableActions.RemoveStatusCondition))
            {
                pc?.RemoveStatusCondition(removeStatusConditionId);
            }

            if (actions.HasFlag(InteractableActions.SaveData))
            {
                pc?.DataSave(pc?.saveFilename);
            }

            if (actions.HasFlag(InteractableActions.LoadData))
            {
                pc?.DataLoad(pc?.saveFilename);
            }

            if (actions.HasFlag(InteractableActions.SetRace))
            {
                pc?.SetRace(raceId);
            }

            if (actions.HasFlag(InteractableActions.RemoveRace))
            {
                pc?.SetRace(string.Empty);
            }

            if (actions.HasFlag(InteractableActions.SpawnObject))
            {
                Instantiate(prefab, parent);
            }

            if(actions.HasFlag(InteractableActions.ActivateObject))
            {
                activateTarget.gameObject.SetActive(true);
            }

            if (actions.HasFlag(InteractableActions.RaiseEvent))
            {
                pc?.RaiseEvent(raiseEventId);
            }

        }

        #endregion

    }
}
#endif