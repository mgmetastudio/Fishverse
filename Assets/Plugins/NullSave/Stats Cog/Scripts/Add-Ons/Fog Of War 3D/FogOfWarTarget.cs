using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Stats
{
    public class FogOfWarTarget : MonoBehaviour
    {

        #region Variables

        public bool hideIfNoActiveViewers;

        private List<Material> mats;
        private List<Color> normal, edge, wasSeen;
        private List<FogOfWarViewer> viewers = new List<FogOfWarViewer>();
        private List<FogOfWarViewer> edgeViewers = new List<FogOfWarViewer>();

        public UnityEvent onDestroyed;

        #endregion

        #region Properties

        public bool IsVisible
        {
            get
            {
                return viewers.Count > 0 || edgeViewers.Count > 0;
            }
        }

        public FogOfWar3D Manager { get; set; }

        public bool WasSeen { get; set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            ResetMaterials();
        }

        private void OnDestroy()
        {
            Manager.RemoveTarget(this);
        }

        #endregion

        #region Public Methods

        public void AddViewer(FogOfWarViewer viewer)
        {
            if (viewers.Contains(viewer)) return;

            viewers.Add(viewer);
            UpdateView();
            WasSeen = true;
        }

        public void AddEdgeViewer(FogOfWarViewer viewer)
        {
            if (edgeViewers.Contains(viewer)) return;

            edgeViewers.Add(viewer);
            WasSeen = true;
            UpdateView();
        }

        public void RemoveEdgeViewer(FogOfWarViewer viewer)
        {
            if (edgeViewers.Count == 0) return;

            edgeViewers.Remove(viewer);
            UpdateView();
        }

        public void RemoveViewer(FogOfWarViewer viewer)
        {
            RemoveEdgeViewer(viewer);
            if (viewers.Count == 0) return;

            viewers.Remove(viewer);
            UpdateView();
        }

        public void ResetMaterials()
        {
            mats = new List<Material>();
            normal = new List<Color>();
            edge = new List<Color>();
            wasSeen = new List<Color>();
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                foreach (Material mat in renderer.materials)
                {
                    mats.Add(mat);
                    if (mat.HasProperty("_Color"))
                    {
                        normal.Add(mat.color);
                        edge.Add(new Color(mat.color.r / 2, mat.color.g / 2, mat.color.b / 2));
                        wasSeen.Add(new Color(mat.color.r / 10, mat.color.g / 10, mat.color.b / 10));
                    }
                    else
                    {
                        normal.Add(Color.white);
                        edge.Add(Color.white);
                        wasSeen.Add(Color.white);
                    }
                }
            }
        }

        public void UpdateView()
        {
            if (mats == null)
            {
                ResetMaterials();
            }

            if (viewers.Count == 0)
            {
                if(edgeViewers.Count > 0)
                {
                    SetRenderers(true);
                    SetMaterial(edge);
                }
                else
                {
                    if(hideIfNoActiveViewers || !WasSeen)
                    {
                        SetRenderers(false);
                    }
                    else
                    {
                        SetMaterial(wasSeen);
                    }
                }
            }
            else
            {
                SetRenderers(true);
                SetMaterial(normal);
            }
        }

        #endregion

        #region Private Methods

        public void SetMaterial(List<Color> color)
        {
            for (int i = 0; i < mats.Count; i++)
            {
                if (mats[i].HasProperty("_Color"))
                {
                    mats[i].color = color[i];
                }
            }
        }

        public void SetMaterial(Color color)
        {
            if (mats == null)
            {
                mats = new List<Material>();
                foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
                {
                    foreach (Material mat in renderer.materials)
                    {
                        mats.Add(mat);
                    }
                }
            }
            foreach (Material mat in mats)
            {
                if (mat.HasProperty("_Color"))
                {
                    mat.color = color;
                }
            }
        }

        private void SetRenderers(bool value)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = value;
            }
        }

        #endregion

    }
}