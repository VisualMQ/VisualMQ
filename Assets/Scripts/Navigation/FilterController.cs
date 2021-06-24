using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterController : MonoBehaviour
{
    public GameObject FilterWindow;
    // UI Objects
    public Dropdown qmDropdown;
    public Dropdown queueNameDropdown;
    public Dropdown queueTypeDropdown;

    public Button confirmFilter;
    public Button cancelFilter;

    // Variables
    int qmList_index;
    int queueName_index;
    int queueType_index;
    List<string> qmList = new List<string> { "QM1", "QM2", "QM3"}; // test: add QM names to dropdown
    List<string> queueNameList = new List<string> {"Helo"};
    List<string> queueTypeList = new List<string> {"hi"};

    // Start is called before the first frame update
    void Start()
    {
        // Window
        FilterWindow.SetActive(false);

        // Listen to button activity
        confirmFilter.onClick.AddListener(ConfirmButtonClicked);
        cancelFilter.onClick.AddListener(CancelButtonClicked);
        
        // Dropdown Listener
        qmDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChangedQM(qmDropdown);
        });
        queueNameDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChangedQueueName(queueNameDropdown);
        });
        queueTypeDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChangedQueueType(queueTypeDropdown);
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    *  Detect Dropdown Value Change
    *
    */
    void DropdownValueChangedQM(Dropdown dropdown)
    {
        Debug.Log("Queue Manager Dropdown Value is changed");
    }

    void GetAllQueueManagerName()
    {

    }

    void DropdownValueChangedQueueName(Dropdown dropdown)
    {
        Debug.Log("Queue Name Dropdown Selected");
    }

    void DropdownValueChangedQueueType(Dropdown dropdown)
    {
        Debug.Log("Queue Type Dropdown Selected");
    }

    // Confirm Button Clicked
    void ConfirmButtonClicked()
    {   
        Debug.Log("Confirm Button Clicked");
    }

    // Cancel Button Clicked
    void CancelButtonClicked()
    {
        Debug.Log("Cancel Button Clicked");
        FilterWindow.SetActive(false);
    }
}
