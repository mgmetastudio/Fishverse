using UnityEngine;
using UnityEngine.UI;
 
public class FpsCounter : MonoBehaviour
{
    [SerializeField] private Text _fpsText;
    [SerializeField] private float _hudRefreshRate = 1f;
    public AppSettings_Data app_settings;
    private float _timer;

    private void Start()
    {
        if (app_settings)
        {
            if (app_settings.show_fps_counter == false)
            {
                Destroy(_fpsText.gameObject);
                Destroy(this);
            }
        }
    }


    private void Update()
    {
        if (Time.unscaledTime > _timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            _fpsText.text = "FPS: " + fps;
            _timer = Time.unscaledTime + _hudRefreshRate;
        }
    }
}