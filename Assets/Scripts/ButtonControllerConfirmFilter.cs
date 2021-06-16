using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControllerConfirmFilter : MonoBehaviour
{
    public Button ButtonConfirm;

    // Start is called before the first frame update
    void Start()
    {

        // Add Listener to the button
        ButtonConfirm.onClick.AddListener(ButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
        WHEN the button (to submit current filter)
        THEN Check what is the current Filter(s) Options
    */
    void ButtonClicked()
    {
        Debug.Log("You have clicked the button!");

    }

    void CheckSelectedFilters()
    {

    }
}
