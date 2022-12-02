using System;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public MiniMap_Object[] minimap_objects;
    public float update_delay;
    private float update_timer;
    private void Update()
    {
        update_timer += Time.deltaTime;
        if (update_timer > update_delay)
        {
            update_timer = 0;
            foreach (MiniMap_Object obj in minimap_objects)
            {
                if(obj)
                    obj.UpdatePosition();
            }
        }
    }
}
