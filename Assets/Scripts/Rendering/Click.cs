using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Click : MonoBehaviour
{
    private float downClickTime;
    private const float CLICK_DELTA_TIME = 0.5f;
    private Vector3 clickpo;

    public GameObject sidebar;

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
                    string objectName = hit.transform.name;
                    Debug.Log("yiiha hitting " + objectName);

                    if (objectName.Substring(0, 5) == "Block")
                    {
                        string qmgrName = objectName.Substring(6);

                        SidebarController sidebarController = sidebar.GetComponent<SidebarController>();
                        sidebarController.ShowQueueManagerDetails(qmgrName);
                    }
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
