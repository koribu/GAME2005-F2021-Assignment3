using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Look")]
    public float sensitivity = 10.0f;

    [Header("Movement")]
    public float maxSpeed = 10.0f;

    private float XAxisRotation = 0.0f;
    private float YAxisRotation = 0.0f;
    private Vector2 mouse;
     
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        MouseLook();
        MouseMove();
    }
    private void MouseMove()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        transform.position = Vector3.MoveTowards(transform.position, transform.forward * maxSpeed,y*maxSpeed*Time.deltaTime);

    }
    private void MouseLook()
    {
        // get input from mouse
        mouse.x = Input.GetAxis("Mouse X") * sensitivity;
        mouse.y = Input.GetAxis("Mouse Y") * sensitivity;

        // look up and down
        XAxisRotation -= mouse.y;
        XAxisRotation = Mathf.Clamp(XAxisRotation, -90.0f, 90.0f);
        //left and right
        YAxisRotation += mouse.x;
      //  YAxisRotation = Mathf.Clamp(YAxisRotation, -90.0f, 90.0f);

        transform.localRotation = Quaternion.Euler(XAxisRotation, YAxisRotation, 0.0f);
    }
}
