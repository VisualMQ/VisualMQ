using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QMClickedController : MonoBehaviour, IPointerClickHandler
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            /*
            Vector3 mousePos = Input.mousePosition;
            Debug.Log("SSSSSS" + mousePos);
            Debug.Log("DDDDD" + this.transform.position + "---" + this.transform.parent.name);
            */
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    // CurrentClickedGameObject(raycastHit.transform.gameObject);
                    Debug.Log("hi");
                }
            }
            
        }


        
    }
    
}          
