using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //VARIABLES
    [SerializeField] private float mouseSensitivity;

    //REFERENCES
    private Transform parent;

    private void Start(){
        parent = transform.parent;
        // Cursor.lockState = CursorLockMode.Locked;   //So we dont see our mouse on the screen and the mouse is always centered. 
    }

    private void Update(){
        Rotate();
    }

    private void Rotate(){
        //Input from our mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        parent.Rotate(Vector3.up, mouseX);
    }
}
