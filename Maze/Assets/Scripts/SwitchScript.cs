using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    public GameObject MazeCamera;
    public GameObject FirstPerson;

    public bool Screen = true;

    // Start is called before the first frame update
    void Start()
    {
        Screen1();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.O)){
            Screen1();
        }

        if (Input.GetKey(KeyCode.P)){
            Screen2();
        }
   }

   public void Screen1(){
        //MazeCamera
        //Debug.Log("Clicked on O");
        MazeCamera.SetActive(true); //False to hide it, true to show. 
        FirstPerson.SetActive(false);
        Cursor.lockState = CursorLockMode.None;   //So we dont see our mouse on the screen and the mouse is always centered.

   }

   public void Screen2(){
        //Controller
        //Debug.Log("Clicked on P");
        MazeCamera.SetActive(false); //False to hide it, true to show. 
        FirstPerson.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;   //So we dont see our mouse on the screen and the mouse is always centered.
   }
}
