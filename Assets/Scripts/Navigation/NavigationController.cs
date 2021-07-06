using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationController : MonoBehaviour
{
    
    public Button connectNewQMButton;
    public Button addFilterButton;
    public Button expandPanelButton;

    public GameObject Authentication;

    // Left Panel: QM Check Selector
    public GameObject leftPanel;
    public GameObject checkboxItem;
    private Dictionary<string, bool> QMVisibility = new Dictionary<string, bool>();

    private int checkBoxNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
        //connectNewQM.SetActive(false);
        //addFilter.SetActive(false);

        Authentication.SetActive(false);  // Default: Hide Auth Window
        leftPanel.SetActive(false);

        // Button Listener
        expandPanelButton.onClick.AddListener(leftPanelButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
    }

    /*
    * Click to open the left panel
    * Click to hide the left panel
    */
    void leftPanelButtonClicked()
    {
        if (leftPanel.activeSelf == true)
        {
            leftPanel.SetActive(false);
            Debug.Log("Left Panel: Close");
        }
        else
        {
            leftPanel.SetActive(true);
            Debug.Log("Left Panel: Expand");
            GenerateCheckBox();
        }
    }

    void GenerateCheckBox()
    {
        // Get Current Number of Check Box
        Debug.Log("NOTICE: Generating checkbox");
        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
        //checkBoxNumber = stateComponent.GetNumberMQ();
        
        List<string> mqlist = stateComponent.RegisteredQMNameList();
        Debug.Log(string.Join(",", mqlist));

        // Default to true, all mq is visiable
        foreach (string mq in mqlist)
        {
            QMVisibility.Add(mq, true);
        }

    }

}
