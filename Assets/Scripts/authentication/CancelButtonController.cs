using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CancelButtonController : MonoBehaviour
{
    public Button cancelButton;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Authentication: Confirm Button");
        //cancelButton.onClick.AddListener(CancelButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CancelButtonClicked()
    {
        //Debug.Log("Cancel Button clicked");
        // If clicked, close current window
        //...
    }
}
