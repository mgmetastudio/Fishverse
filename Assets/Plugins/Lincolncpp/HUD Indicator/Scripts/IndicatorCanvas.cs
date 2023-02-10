using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LincolnCpp.HUDIndicator {

    public abstract class IndicatorCanvas {
        protected Indicator indicator { private set; get; }
        protected IndicatorRenderer renderer { private set; get; }
        protected GameObject gameObject;
        protected bool active;

        protected GameObject destanceGameObject;
        protected TMPro.TMP_Text distanceText;

        public virtual void Create(Indicator indicator, IndicatorRenderer renderer) {
            this.indicator = indicator;
            this.renderer = renderer;

            active = true;
		}

        public abstract void LateUpdate();

        public virtual void OnEnable() {
            if(gameObject != null) {
                gameObject.SetActive(true);
			}
            active = true;
        }
        public virtual void OnDisable(){
            if(gameObject != null) {
                gameObject.SetActive(false);
            }
            active = false;
        }

        public virtual void Destroy() {}

        public bool IsVisible() {
            return indicator.Visible && renderer.visible;
        }

        protected void UpdateDistance()
        {
            distanceText.text = (Mathf.Clamp((int)Vector3.Distance(indicator.gameObject.transform.position, renderer.camera.transform.position), 0, 9999)).ToString() + "m";
        }
    }
}