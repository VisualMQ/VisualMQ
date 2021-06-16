using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownControllerQueue : MonoBehaviour
{
    public Dropdown dropdownQueue;
    public int dropdownQueue_index;
    List<string> dropQueue_Options = new List<string> { "queue 1", "queue 2", "queue 3"}; // test: add QM names to dropdown

    // Start is called before the first frame update
    void Start()
    {
        dropdownQueue = GetComponent<Dropdown>();
        Debug.Log("This is dropdown for queue");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Listen to Dropdown value change
    void DropdownValueChanged(Dropdown dropdownQueue)
    {
        Debug.Log("Dropdown Value is changed, Current Queue: " + dropQueue_Options[dropdownQueue.value]);
    }
}
