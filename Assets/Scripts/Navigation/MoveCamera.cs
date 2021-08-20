/*
 * Code used from https://gist.github.com/oluf/6951522e4be794178a82cc76be9ee93f
 * Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.  
 * Converted to C# 27-02-13 - no credit wanted.
 * Simple flycam I made, since I couldn't find any others made public.  
 * Made simple to use (drag and drop, done) for regular keyboard layout  
 * WASD : basic movement
 */

using UnityEngine;
using UnityEngine.EventSystems;


public class MoveCamera : MonoBehaviour
{
    public float mainSpeed = 80.0f; //regular speed
    public float shiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running
    public float maxShift = 1000.0f; //Maximum speed when holdin gshift
    public float camSens = 0.25f; //How sensitive it with mouse
    public Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float totalRun = 1.0f;

    public GameObject authenticationWindow;


    private void Start()
    {
        lastMouse = Input.mousePosition;
    }


    void Update()
    {
        // Make sure camera doesn't move when typing and having input window open
        if (authenticationWindow.activeInHierarchy) return;

        Move();

        if (Input.GetMouseButton(0))
        {
            RotateCamera();
        }
            
        lastMouse = Input.mousePosition;
    }


    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            p_Velocity += new Vector3(0, -1, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            p_Velocity += new Vector3(0, 1, 0);
        }
        return p_Velocity;
    }


    public void RotateCamera()
    {
        // Avoid click/drag through UI windows
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        lastMouse = Input.mousePosition - lastMouse;
        lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
        lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
        transform.eulerAngles = lastMouse;
        lastMouse = Input.mousePosition;
        //Mouse  camera angle done.  
    }


    public void Move()
    {
        //Keyboard commands
        Vector3 p = GetBaseInput();
        totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
        p = p * mainSpeed * Time.deltaTime;
        transform.Translate(p);
    }

}
