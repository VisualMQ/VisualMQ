using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownController : MonoBehaviour
{
    public Dropdown dropdown;
    public int dropdown_index;
    List<string> dropOptions = new List<string> { "QM1", "QM2", "QM3"}; // test: add QM names to dropdown

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Just want to tell you the Dropdown for QM is initialised");

        // Connect to the Component
        dropdown = GetComponent<Dropdown>();

        // Clear previous options
        dropdown.ClearOptions();
        // Add some options
        dropdown.AddOptions(dropOptions);

        // Listener to Dropdown value change
        dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(dropdown);
        });

    }

    /*
    // Update is called once per frame
    void Update()
    {
        //dropdown_value = dropdown.value;
        //Debug.Log(dropdown_value);

    }
    */

    // Listen to Dropdown value change
    void DropdownValueChanged(Dropdown dropdown)
    {
        Debug.Log("Dropdown Value is changed, Current QM: " + dropOptions[dropdown.value]);
    }

}
