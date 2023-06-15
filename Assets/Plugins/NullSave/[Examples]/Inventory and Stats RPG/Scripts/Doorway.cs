using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Doorway : MonoBehaviour
{

    #region Variables

    public GameObject blackout;
    public bool visibleAfterTrigger;
    public GameObject[] disableCameras;
    public GameObject enableCamera;

    #endregion

    #region Unity Methods

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        blackout.SetActive(visibleAfterTrigger);
        if (!visibleAfterTrigger)
        {
            foreach(GameObject obj in disableCameras)
            {
                obj.SetActive(false);
            }
            enableCamera.SetActive(true);
        }
    }

    #endregion

}
