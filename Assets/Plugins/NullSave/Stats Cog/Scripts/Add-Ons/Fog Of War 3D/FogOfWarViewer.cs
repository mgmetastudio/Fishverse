using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [RequireComponent(typeof(StatsCog))]
    public class FogOfWarViewer : MonoBehaviour
    {

        #region Variables

        public LayerMask collisionMask;
        public LayerMask existanceMask;

        public string visionRangeStat = "VisionRange";

        private Vector3 lastPos;
        private Quaternion lastRot;

        private List<FogOfWarTarget> views = new List<FogOfWarTarget>();

        private StatsCog statsCog;
        private StatValue statVision;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            statsCog = GetComponentInChildren<StatsCog>();
            statVision = statsCog.FindStat(visionRangeStat);
            if(statVision != null)
            {
                statVision.onValueChanged.AddListener((float v1, float v2) => Look());
            }
        }

        private void Start()
        {
            lastPos = transform.position;
            lastRot = transform.rotation;
            Look();
        }

        private void Update()
        {
            if (transform.position != lastPos || transform.rotation != lastRot)
            {
                lastPos = transform.position;
                lastRot = transform.rotation;
                Look();
            }
        }

        #endregion

        #region Public Methods

        public void Look()
        {
            float radius = statVision.CurrentValue / 2;

            Vector3 yOffset = new Vector3(0, 0.5f, 0);
            Vector3 castFrom = transform.position + yOffset;

            // Remove existing views
            foreach (FogOfWarTarget view in views)
            {
                view.RemoveViewer(this);
            }
            views.Clear();

            FogOfWarTarget item;
            RaycastHit subhit;
            RaycastHit[] hits = Physics.SphereCastAll(castFrom, radius, transform.up, radius, existanceMask);
            float distance;
            for (int i = 0; i < hits.Length; i++)
            {
                item = hits[i].transform.gameObject.GetComponentInChildren<FogOfWarTarget>();
                if(item == null && hits[i].transform.parent != null)
                {
                    item = hits[i].transform.parent.gameObject.GetComponentInChildren<FogOfWarTarget>();
                }
                if (item != null)
                {
                    distance = Vector3.Distance(castFrom, hits[i].transform.position);

                    // Check for blocker
                    if (Physics.Raycast(castFrom, (hits[i].transform.position + yOffset) - castFrom, out subhit, distance - 0.05f, collisionMask))
                    {
                        if (hits[i].transform == subhit.transform || hits[i].transform.parent == subhit.transform || subhit.transform.parent == hits[i].transform)
                        {
                            if (radius - distance <= 0.5f)
                            {
                                item.AddEdgeViewer(this);
                            }
                            else
                            {
                                item.AddViewer(this);
                            }
                            views.Add(item);
                        }
                    }
                    else
                    {
                        if (radius - distance <= 0.5f)
                        {
                            item.AddEdgeViewer(this);
                        }
                        else
                        {
                            item.AddViewer(this);
                        }
                        views.Add(item);
                    }
                }
            }
        }

        public void RemoveFromView(FogOfWarTarget target)
        {
            views.Remove(target);
        }

        #endregion

    }
}