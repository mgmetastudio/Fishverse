using UnityEngine;

public class CameraRaycaster : MonoBehaviour
{
    public RaycastData data_water;
    public RaycastData data_interactions;

    [HideInInspector]
    public RaycastHit hit;
    [HideInInspector]
    public bool isHit;

    public void Cast_Water()
    {
        isHit = Physics.BoxCast(transform.position, Vector3.one * data_water.box_size, transform.forward, out hit, transform.rotation, data_water.max_distance, data_water.raycast_mask);

        if (isHit)
        {
            if (hit.distance < data_water.min_trace_distance)
            {
                isHit = false;
                data_water.is_hitting = false;
            }
            else
            {
                data_water.is_hitting = hit.collider.CompareTag(data_water.object_tag);
            }
        }
        else
        {
            data_water.is_hitting = false;
        }
    }

    public void Cast_Interactions()
    {
        isHit = Physics.BoxCast(transform.position, Vector3.one * data_interactions.box_size, transform.forward, out hit, transform.rotation, data_interactions.max_distance, data_interactions.raycast_mask);

        if (isHit)
        {
            if (hit.distance < data_interactions.min_trace_distance)
            {
                isHit = false;
                data_interactions.is_hitting = false;
            }
            else
            {
                data_interactions.is_hitting = hit.collider.CompareTag(data_interactions.object_tag);
            }
        }
        else
        {
            data_interactions.is_hitting = false;
        }
    }

    void OnDrawGizmos()
    {

        if (data_water.draw_debug_info)
        {
            if (isHit)
            {
                if (data_water.is_hitting)
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.yellow;

                Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
                Gizmos.DrawWireCube(transform.position + transform.forward * hit.distance, Vector3.one * 0.5f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, transform.forward * data_water.box_size);
            }


            if (data_interactions.draw_debug_info)
            {
                if (isHit)
                {
                    if (data_interactions.is_hitting)
                        Gizmos.color = Color.blue;
                    else
                        Gizmos.color = Color.cyan;

                    Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
                    Gizmos.DrawWireCube(transform.position + transform.forward * hit.distance, Vector3.one * 0.2f);
                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(transform.position, transform.forward * data_interactions.box_size);
                }
            }
        }
    }

        public Vector3 GetLastHitPosition()
    {
        return (transform.position + transform.forward * hit.distance);
    }
}

[System.Serializable]
public class RaycastData
{
    public bool draw_debug_info;
    public LayerMask raycast_mask;
    public float min_trace_distance = 1f;
    public float max_distance = 20f;
    public float box_size = 0.4f;
    public bool is_hitting;
    public string object_tag;
}