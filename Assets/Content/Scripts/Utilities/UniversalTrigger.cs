using UnityEngine;
using UnityEngine.Events;

public class UniversalTrigger : MonoBehaviour
{
    public bool print_info = false;
    public UnityEvent on_enter;
    public UnityEvent on_exit;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(print_info)
                Debug.Log("Enter: " + other.name);

            on_enter.Invoke();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (print_info)
                Debug.Log("Exit: " + other.name);
            on_exit.Invoke();
        }
    }
}
