using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QMClicked : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onMouseDown()
    {
        GameObject mainCamera =  GameObject.Find("Main Camera");
        mainCamera.transform.rotation = Quaternion.identity;
        mainCamera.transform.rotation = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
        // mainCamera.transform.rotation = Quaternion.AngleAxis(60, new Vector3(1, 0, 0));
        Vector3 targetPosition = this.transform.position;
        targetPosition.y += 18f;
        // targetPosition.x += 10f;
        // targetPosition.z -= 5f; 
        mainCamera.transform.position =  targetPosition;
        Debug.Log("QM clicked");
    }
}
