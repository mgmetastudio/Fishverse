using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishing_Orbit_Camera : MonoBehaviour
{
    public float ScrollSpeed = 15;

    public float ScrollEdge = 0.1f;

    public float PanSpeed = 10;

    public Vector2 zoomRange = new Vector2(-10, 100);

    public float CurrentZoom = 0;

    public float ZoomZpeed = 1;

    public float ZoomRotation = 1;

    public Vector2 zoomAngleRange = new Vector2(20, 70);

    public float rotateSpeed = 10;

    private Vector3 initialPosition;

    private Vector3 initialRotation;

    [Header("Max Z Position")]
    public float Max_Z_Position = 178f;
    [Header("Min Z Position")]
    public float Min_Z_Position = 91.68f;
    [Header("Max X Position")]
    public float Max_X_Position = 238.9f;
    [Header("Min X Position")]
    public float Min_X_Position = 164.7f;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.eulerAngles;
    }


    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, Min_X_Position, Max_X_Position);
        //pos.z = Mathf.Clamp(pos.z, 93, 41);

        transform.position = pos;

        if (transform.position.z > Max_Z_Position)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Max_Z_Position);
        }
        if (transform.position.z < Min_Z_Position)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Min_Z_Position);
        }
        // panning     
        /*if (Input.GetMouseButton(1))
        {
            transform.Translate(Vector3.right * Time.deltaTime * PanSpeed * (Input.mousePosition.x - Screen.width * 0.5f) / (Screen.width * 0.5f), Space.World);
            transform.Translate(Vector3.forward * Time.deltaTime * PanSpeed * (Input.mousePosition.y - Screen.height * 0.5f) / (Screen.height * 0.5f), Space.World);
        }*/

        //else
        //{
        if (Input.GetKey("d"))
        {
            transform.Translate(Vector3.right * Time.deltaTime * PanSpeed, Space.Self);
        }
        else if (Input.GetKey("a"))
        {
            transform.Translate(Vector3.right * Time.deltaTime * -PanSpeed, Space.Self);
        }

        if (Input.GetKey("w"))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * PanSpeed, Space.Self);
        }
        else if (Input.GetKey("s"))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * -PanSpeed, Space.Self);
        }

        if (Input.GetKey("q"))
        {
            transform.Rotate(Vector3.up * Time.deltaTime * -rotateSpeed, Space.World);
        }
        else if (Input.GetKey("e"))
        {
            transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed, Space.World);
        }
        //}

        // zoom in/out
        CurrentZoom -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 1000 * ZoomZpeed;

        CurrentZoom = Mathf.Clamp(CurrentZoom, zoomRange.x, zoomRange.y);

        transform.position = new Vector3(transform.position.x, transform.position.y - (transform.position.y - (initialPosition.y + CurrentZoom)) * 0.1f, transform.position.z);

        float x = transform.eulerAngles.x - (transform.eulerAngles.x - (initialRotation.x + CurrentZoom * ZoomRotation)) * 0.1f;
        x = Mathf.Clamp(x, zoomAngleRange.x, zoomAngleRange.y);

        transform.eulerAngles = new Vector3(x, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
