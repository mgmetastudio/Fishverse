using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MTAssets.EasyMinimapSystem
{
    public class WorldMapZoom : MonoBehaviour
    {
        private float defaultZoomMultiplier = 0.0f;
        public float defaultFieldOfView = 0.0f;

        public MinimapCamera minimapCamera;
        public Slider zoomSlider;
        public float maxZoomPossible = 75.0f;
        public float minItemsMultiplierPossible = 1.5f;
        public float maxItemsMultiplierPossible = 3f;
        //For buttons + and -
        public float zoomStep = 0.1f;
        private Vector2 touchStartPos;
        private float touchStartSliderValue;
        private float initialPinchDistance;
        public bool isHandlePinchZoom;
        public Vector3 originalPosition;
        private bool isZoomingOut = false;

        void Start()
        {
            //Get default param
            defaultZoomMultiplier = MinimapDataGlobal.GetMinimapItemsSizeGlobalMultiplier();
            defaultFieldOfView = minimapCamera.fieldOfView;
            isHandlePinchZoom = false;
            originalPosition = minimapCamera.transform.position;

        }

        void Update()
        {
            HandlePinchZoom();
           
            //Calculate zoom and apply

            float newMultiplier = Mathf.Lerp(minItemsMultiplierPossible, maxItemsMultiplierPossible, zoomSlider.value);
            MinimapDataGlobal.SetMinimapItemsSizeGlobalMultiplier(newMultiplier);
            float zoomDelta = maxZoomPossible * zoomSlider.value;

            // Check if zoom slider value is decreasing
            if (zoomSlider.value < touchStartSliderValue)
            {
                isZoomingOut = true;
            }
            else
            {
                isZoomingOut = false;
            }

            if (isZoomingOut)
            {
                // Adjust the target position based on zoom value
                Vector3 targetPosition = CalculateTargetPosition();

                // Lerp the position towards the target position
                minimapCamera.transform.position = Vector3.Lerp(minimapCamera.transform.position, targetPosition, 0.5f);
            }

            minimapCamera.fieldOfView = defaultFieldOfView - zoomDelta;
            touchStartSliderValue = zoomSlider.value;


        }
        public void DecreaseZoom()
        {
            zoomSlider.value = Mathf.Clamp01(zoomSlider.value - zoomStep);
        }
        public void IncreaseZoom()
        {
            zoomSlider.value = Mathf.Clamp01(zoomSlider.value + zoomStep);
        }
        void HandlePinchZoom()
        {
            if (Input.touchCount == 2)
            {
                isHandlePinchZoom = true;
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
                {
                    touchStartPos = (touch1.position + touch2.position) / 2;
                    initialPinchDistance = Vector2.Distance(touch1.position, touch2.position);
                }
                else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    Vector2 currentPos = (touch1.position + touch2.position) / 2;
                    float currentPinchDistance = Vector2.Distance(touch1.position, touch2.position);

                    // Calculate pinch delta
                    float pinchDelta = currentPinchDistance - initialPinchDistance;

                    // Adjust zoom slider based on pinch delta
                    zoomSlider.value = Mathf.Clamp01(zoomSlider.value + pinchDelta * 0.0005f);
                }
            }
            else
            {
                isHandlePinchZoom = false;
            }
        }

        Vector3 CalculateTargetPosition()
        {
            // Adjust the target position based on zoom value
            return Vector3.Lerp(originalPosition, new Vector3(16f, 99.7f, 120f),1-zoomSlider.value);
        }
    }
}