using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MTAssets.EasyMinimapSystem
{
    public class GameController : MonoBehaviour
    {
        //Private cache
        private float beforeFullscreenGlobalSizeMultiplier = -1;

        //Private cache to minimap renderer events
        private int clicksToCreateMarkInMinimap = 0;
        private Vector3 lastWorldPosClickInMinimap = Vector3.zero;
        private Dictionary<MinimapItem, Vector3> allMinimapItemsAndOriginalSizes = new Dictionary<MinimapItem, Vector3>();
        public WorldMapZoom WorldMapZoom;
        //Public variables
        public GameObject fullScreenMapObj;
        public PlayerScript player;
        public MinimapCamera playerMinimapCamera;
        public MinimapItem marker;
        public MinimapItem cursor;
        public MinimapItem playerFieldOfView;
        Dictionary<int, int> Limit0 = new Dictionary<int, int>
        {
            {0, -120},
            {1, 120},
            {2, -120},
            {3, 120},
            {4, 0},
            {5, 250}
        };

        Dictionary<int, int> Limit1 = new Dictionary<int, int>
        {
            {0, -105},
            {1, 105},
            {2, -120},
            {3, 120},
            {4, 10},
            {5, 240}
        };
        Dictionary<int, int> Limit2 = new Dictionary<int, int>
        {
            {0, -30},
            {1, 30},
            {2, -180},
            {3, 200},
            {4, 100},
            {5, 160}
        };
        Dictionary<int, int> Limit3 = new Dictionary<int, int>
        {
            {0, -5},
            {1, 20},
            {2, -140},
            {3, 160},
            {4, 115},
            {5, 142}
        };
        public float minXLimit = -500f;  // Set your own values
        public float maxXLimit = 500f;   // Set your own values
        public float minYLimit = -400f;  // Set your own values
        public float maxYLimit = 400f;   // Set your own values
        public float minZLimit = -230f;  // Set your own values
        public float maxZLimit = 500f;   // Set your own values



        //On update

        void Update()
        {
            //On press M
            if (Input.GetKeyDown(KeyCode.M) == true && fullScreenMapObj.activeSelf == false)
                OpenFullscreenMap();
            if (Input.GetKeyDown(KeyCode.Escape) == true)
                if (fullScreenMapObj.activeSelf == true)
                    CloseFullscreenMap();
        }

        //Button methods

        public void OpenFullscreenMap()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
#endif
            fullScreenMapObj.SetActive(true);
            playerMinimapCamera.gameObject.SetActive(true);
            if(playerFieldOfView == null) { return; }
            else
            {
             playerFieldOfView.enabled = false;
            }
            beforeFullscreenGlobalSizeMultiplier = MinimapDataGlobal.GetMinimapItemsSizeGlobalMultiplier();
            MinimapDataGlobal.SetMinimapItemsSizeGlobalMultiplier(1.5f);
            //player.canHideCursor = false;
            //if (cursor != null)
                //cursor.gameObject.SetActive(true);
        }

        public void CloseFullscreenMap()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
#endif
            fullScreenMapObj.SetActive(false);
            playerMinimapCamera.gameObject.SetActive(false);
            if (playerFieldOfView != null)
            {
                playerFieldOfView.enabled = true;
            }
            MinimapDataGlobal.SetMinimapItemsSizeGlobalMultiplier(beforeFullscreenGlobalSizeMultiplier);
            //player.canHideCursor = true;
            //if (cursor != null)
                //cursor.gameObject.SetActive(false);
        }

        public void OnClickInMinimapRendererArea(Vector3 clickWorldPos, MinimapItem clickedMinimapItem)
        {
            //If is the first click on map, start the routine of double click to mark
            if (clicksToCreateMarkInMinimap == 0)
                StartCoroutine(OnClickInMinimapRendererArea_DoubleClickRoutine());
            //Increase the counter of clicks
            clicksToCreateMarkInMinimap += 1;
            //Store the last click data
            lastWorldPosClickInMinimap = clickWorldPos;
            //Show the Minimap Item Clicked
            if (clickedMinimapItem != null)
                Debug.Log("You clicked on Minimap Item \"" + clickedMinimapItem.gameObject.name + "\".");
        }

        IEnumerator OnClickInMinimapRendererArea_DoubleClickRoutine()
        {
            int milisecondsPassed = 0;

            while (enabled)
            {
                if (milisecondsPassed >= 25) //<-- if is passed 25ms, reset the counter of clicks and break the loop
                {
                    clicksToCreateMarkInMinimap = 0;
                    break;
                }

                if (clicksToCreateMarkInMinimap >= 2)
                {
                    if (marker == null) break;
                    marker.gameObject.SetActive(true);
                    marker.transform.position = lastWorldPosClickInMinimap;
                    clicksToCreateMarkInMinimap = 0;
                    break;
                }

                yield return new WaitForSecondsRealtime(0.001f); //<-- 0.001 is 1ms
                milisecondsPassed += 1;
            }
        }

        public void OnDragInMinimapRendererArea(Vector3 onStartThisDragWorldPos, Vector3 onDraggingWorldPos)
        {
            //Use the position of drag start and current position of drag to move the Minimap Camera of fullscreen minimap
            Vector3 deltaPositionToMoveMap = (onDraggingWorldPos - onStartThisDragWorldPos) * -1.0f;
            playerMinimapCamera.transform.position += (deltaPositionToMoveMap * 10.0f * Time.deltaTime);
        }

        public void OnDragInMinimapRendererArea_(Vector3 onStartThisDragWorldPos, Vector3 onDraggingWorldPos)
        {
            if (WorldMapZoom != null && WorldMapZoom.isHandlePinchZoom)
            {
                // Pinch zoom is being handled, don't move the camera
                return;
            }
            if (playerMinimapCamera.fieldOfView < WorldMapZoom.defaultFieldOfView)
            {
                // Calculate the delta position to move the Minimap Camera
                Vector3 deltaPositionToMoveMap = (onDraggingWorldPos - onStartThisDragWorldPos) * -1.0f;

                // Calculate the new position


                if (WorldMapZoom.zoomSlider.value >= 0.70f)
                {
                    Vector3 newPosition = playerMinimapCamera.transform.position + (deltaPositionToMoveMap * 10.0f * Time.deltaTime);
                    // Clamp the new position within the canvas boundaries
                    newPosition.x = Mathf.Clamp(newPosition.x, Limit0[0], Limit0[1]);
                    newPosition.y = Mathf.Clamp(newPosition.y, Limit0[2], Limit0[3]);
                    newPosition.z = Mathf.Clamp(newPosition.z, Limit0[4], Limit0[5]);
                    playerMinimapCamera.transform.position = newPosition;
                }
                else
                if (WorldMapZoom.zoomSlider.value >= 0.55f && WorldMapZoom.zoomSlider.value < 0.70f)
                {
                    Vector3 newPosition = playerMinimapCamera.transform.position + (deltaPositionToMoveMap * 10.0f * Time.deltaTime);
                    newPosition.x = Mathf.Clamp(newPosition.x, Limit1[0], Limit1[1]);
                    newPosition.y = Mathf.Clamp(newPosition.y, Limit1[2], Limit1[3]);
                    newPosition.z = Mathf.Clamp(newPosition.z, Limit1[4], Limit1[5]);
                    playerMinimapCamera.transform.position = newPosition;
                }
                else
                if (WorldMapZoom.zoomSlider.value >= 0.16f && WorldMapZoom.zoomSlider.value < 0.55f)
                {
                    Vector3 newPosition = playerMinimapCamera.transform.position + (deltaPositionToMoveMap * 10.0f * Time.deltaTime);
                    newPosition.x = Mathf.Clamp(newPosition.x, Limit2[0], Limit2[1]);
                    newPosition.y = Mathf.Clamp(newPosition.y, Limit2[2], Limit2[3]);
                    newPosition.z = Mathf.Clamp(newPosition.z, Limit2[4], Limit2[5]);
                    playerMinimapCamera.transform.position = newPosition;
                }
                else
                if (WorldMapZoom.zoomSlider.value >= 0.025f && WorldMapZoom.zoomSlider.value < 0.16f)
                {
                    Vector3 newPosition = playerMinimapCamera.transform.position + (deltaPositionToMoveMap * 10.0f * Time.deltaTime);
                    newPosition.x = Mathf.Clamp(newPosition.x, Limit3[0], Limit3[1]);
                    newPosition.y = Mathf.Clamp(newPosition.y, Limit3[2], Limit3[3]);
                    newPosition.z = Mathf.Clamp(newPosition.z, Limit3[4], Limit3[5]);
                    playerMinimapCamera.transform.position = newPosition;
                }

                // Set the new position

            }
        }
  
        public void OnOverInMinimapRendererArea(bool isOverMinimapRendererArea, Vector3 mouseWorldPos, MinimapItem overMinimapItem)
        {
            if (cursor == null) return;
            //Hide the cursor
            if (isOverMinimapRendererArea == false)
                cursor.gameObject.SetActive(false);
            //Show the cursor and run logic of on mouse over
            if (isOverMinimapRendererArea == true )
            {
                //Show cursor
                cursor.gameObject.SetActive(true); //<- "Raycast Target" of this Minimap Item, is off

                //Move the cursor
                cursor.gameObject.transform.position = mouseWorldPos;

                //Reset all original sizes
                foreach (var key in allMinimapItemsAndOriginalSizes)
                {
                    if (overMinimapItem != null && key.Key == overMinimapItem)
                        continue;

                    key.Key.sizeOnMinimap = key.Value;
                }

                //Get all minimap items
                MinimapItem[] allMinimapItems = cursor.GetListOfAllMinimapItemsInThisScene();
                //Fill the dictionary of all minimap items
                for (int i = 0; i < allMinimapItems.Length; i++)
                {
                    //Get the minimap item
                    MinimapItem item = allMinimapItems[i];
                    //If is null, skip
                    if (item == null)
                        continue;
                    //Fill the dictionary
                    if (allMinimapItemsAndOriginalSizes.ContainsKey(item) == false)
                        allMinimapItemsAndOriginalSizes.Add(item, item.sizeOnMinimap);
                }

                //Increase size of the selected item (avoid increase size of same minimap item various times)
                if (overMinimapItem != null && overMinimapItem.sizeOnMinimap != (allMinimapItemsAndOriginalSizes[overMinimapItem] * 3.0f))
                    overMinimapItem.sizeOnMinimap = overMinimapItem.sizeOnMinimap * 3.0f;
            }
        }
    }
}