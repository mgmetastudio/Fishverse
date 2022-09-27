using UnityEngine;

public class CameraRaycaster : MonoBehaviour
{
    public bool draw_debug_info;
    public LayerMask raycast_mask;
    public float min_trace_distance = 1f;
    public float max_distance = 20f;
    public float box_size = 0.4f;
    public bool is_hitting_water;
    
    [HideInInspector]
    public RaycastHit hit;
    [HideInInspector]
    public bool isHit;

    public void Cast()
    {
        isHit = Physics.BoxCast(transform.position, Vector3.one * box_size, transform.forward, out hit, transform.rotation, max_distance, raycast_mask);

        if (isHit)
        {
            if (hit.distance < min_trace_distance)
            {
                isHit = false;
                is_hitting_water = false;
            }
            else
            {
                is_hitting_water = hit.collider.CompareTag("Water");
            }
        }
        else
        {
            is_hitting_water = false;
        }
    }

    void OnDrawGizmos()
    {
        
        if (draw_debug_info)
        {
            if (isHit)
            {
                if (is_hitting_water)
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.yellow;

                Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
                Gizmos.DrawWireCube(transform.position + transform.forward * hit.distance, Vector3.one * 0.5f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, transform.forward * box_size);
            }
        }
    }

    public Vector3 GetLastHitPosition()
    {
        return (transform.position + transform.forward * hit.distance);
    }
}