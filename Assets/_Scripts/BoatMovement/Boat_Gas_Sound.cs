using UnityEngine;

public class Boat_Gas_Sound : MonoBehaviour
{
    public Object_Speed target;
    public AudioSource source;
    public float transition_speed = 5f;
    public float min_speed = 0;
    public float max_speed = 10;
    public float min_pitch = 0.8f;
    public float max_pitch = 1.1f;
    public float min_volume = 0f;
    public float max_volume = 0.04f;

    void Update()
    {
        float target_pitch = target.current_speed.Remap(min_speed, max_speed, min_pitch, max_pitch);
        source.pitch = Mathf.Lerp(source.pitch, target_pitch, Time.deltaTime * transition_speed);

        float target_volume = target.current_speed.Remap(min_speed, max_speed, min_volume, max_volume);
        source.volume = Mathf.Lerp(source.volume, target_volume, Time.deltaTime * transition_speed);
    }
}
