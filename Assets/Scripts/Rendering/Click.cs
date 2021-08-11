using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Click : MonoBehaviour
{
    private float downClickTime;
    private const float CLICK_DELTA_TIME = 0.5f;
    private Vector3 clickpo;

    // Update is called once per frame
    void Update()
    {
        // Check for LEFT mouse input 

        if (Input.GetMouseButtonDown(0))
        {
            downClickTime = Time.time;
            clickpo = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            if (Time.time - downClickTime <= CLICK_DELTA_TIME)
            {
                Ray ray = Camera.main.ScreenPointToRay(clickpo);
                RaycastHit hit;
                // Casts the ray and get the first game object hit
                if (Physics.Raycast(ray, out hit, 100))
                {
                }
                else
                {
                    // Nothing to highlight so broadcast empty list
                    gameObject.BroadcastMessage("Highlight", new List<string>(), SendMessageOptions.DontRequireReceiver);

                    // Close all sidebars
                    GameObject canvas = GameObject.Find("Canvas");
                    canvas.BroadcastMessage("Close", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}
