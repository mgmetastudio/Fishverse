using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class FogOfWar3D : MonoBehaviour
    {

        #region Variables

        public Transform worldContainer;

        private FogOfWarViewer[] lookers;
        private FogOfWarTarget[] sites;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            ResetVision();
        }

        #endregion

        #region Public Methods

        public void RefreshViewers()
        {
            foreach (FogOfWarViewer looker in lookers)
            {
                looker.Look();
            }
        }

        public void RemoveTarget(FogOfWarTarget target)
        {
            List<FogOfWarTarget> targets = new List<FogOfWarTarget>();
            for (int i = 0; i < sites.Length; i++)
            {
                if (sites[i] != target)
                {
                    targets.Add(sites[i]);
                }
            }

            sites = new FogOfWarTarget[targets.Count];
            for (int i = 0; i < sites.Length; i++)
            {
                sites[i] = targets[i];
            }

            foreach(FogOfWarViewer viewer in lookers)
            {
                viewer.RemoveFromView(target);
            }
        }

        public void RemoveViewer(FogOfWarViewer target)
        {
            List<FogOfWarViewer> targets = new List<FogOfWarViewer>();
            for (int i = 0; i < lookers.Length; i++)
            {
                if (lookers[i] != target)
                {
                    targets.Add(lookers[i]);
                }
            }

            lookers = new FogOfWarViewer[targets.Count];
            for (int i = 0; i < lookers.Length; i++)
            {
                lookers[i] = targets[i];
            }
        }

        public void ResetVision()
        {
            lookers = worldContainer.gameObject.GetComponentsInChildren<FogOfWarViewer>();
            sites = worldContainer.gameObject.GetComponentsInChildren<FogOfWarTarget>();

            foreach (FogOfWarTarget site in sites)
            {
                site.Manager = this;
                site.WasSeen = false;
                site.UpdateView();
            }

            RefreshViewers();
        }

        #endregion

    }
}