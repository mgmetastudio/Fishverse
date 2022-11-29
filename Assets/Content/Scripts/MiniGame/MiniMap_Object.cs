using System;
using UnityEngine;

public class MiniMap_Object : MonoBehaviour
{
    public Transform minimap_root;
    public GameObject minimap_point_prefab;
    public Transform player;
    public float distance;
    public RectTransform ui_dot;
    public float minimap_size;
    public float max_distance = 30;
    private Vector3 position_relative;

    private void Awake()
    {
        GameObject g = Instantiate(minimap_point_prefab);
        g.transform.SetParent(minimap_root);
        ui_dot = g.GetComponent<RectTransform>();
    }

    public void UpdatePosition()
    {
        distance = Vector3.Distance(transform.position, player.position);

        if (distance < max_distance * 1.5f)
        {
            position_relative = player.InverseTransformPoint(transform.position).normalized;
            float distance_lerp = Mathf.InverseLerp(0, max_distance, distance);
            float pos_x = position_relative.x * minimap_size;
            float pos_y = position_relative.z * minimap_size;
            ui_dot.anchoredPosition = new Vector2(pos_x, pos_y) * distance_lerp;

            Vector3 pos_3d = ui_dot.anchoredPosition3D;
            pos_3d.z = 0;

            ui_dot.anchoredPosition3D = pos_3d;
            ui_dot.localRotation = Quaternion.identity;
            ui_dot.localScale = Vector3.one;
        }
    }

    private void OnDestroy()
    {
        if(ui_dot)
            Destroy(ui_dot.gameObject);
    }
}