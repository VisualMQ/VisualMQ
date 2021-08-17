using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationController : MonoBehaviour
{
    // Windows Gameobject
    public GameObject Authentication;
    
    // Buttons
    private Button expandQMSelector, authenticateNewQM, buttonExit, buttonHelp, buttonReset;

    // Camera position
    // private Camera mainCamera;

    // Left Panel and Container
    private GameObject queueManagersList;
    private GameObject queueManagerRowItem;

    // QM Selector
    private Dictionary<string, bool> QMVisibility = new Dictionary<string, bool>();


    // Locate the Objects
    private void Awake() 
    {
        // Locate the Game Obejct: Button
        expandQMSelector = transform.Find("ButtonView").GetComponent<Button>();
        authenticateNewQM = transform.Find("ButtonConnect").GetComponent<Button>();
        buttonExit = transform.Find("ButtonExit").GetComponent<Button>();
        buttonHelp = transform.Find("ButtonHelp").GetComponent<Button>();
        buttonReset = transform.Find("ButtonReset").GetComponent<Button>();

        // Button Listener
        expandQMSelector.onClick.AddListener(LeftPanelButtonClicked);
        authenticateNewQM.onClick.AddListener(ConnectButtonClicked);
        buttonExit.onClick.AddListener(ExitButtonClicked);
        buttonHelp.onClick.AddListener(HelpButtonClicked);
        buttonReset.onClick.AddListener(ResetButtonClicked);
        
        // Resources for queue manager list dropdown
        queueManagersList = transform.Find("ViewQueueManagers").gameObject;
        queueManagerRowItem = Resources.Load("Prefabs/QueueManagerRowItem") as GameObject;
    }


    private void Start()
    {
        // Default: Hide Auth, Filter, Left Panel Windows
        Authentication.SetActive(false); 
        queueManagersList.SetActive(false);
        buttonReset.gameObject.SetActive(false);
    }


    // Click to open the left panel; Click to hide the left panel
    private void LeftPanelButtonClicked()
    {
        if (queueManagersList.activeSelf == true)
        {
            queueManagersList.SetActive(false);
            buttonReset.gameObject.SetActive(false);
        }
        else
        {
            queueManagersList.SetActive(true);
            buttonReset.gameObject.SetActive(true);
            GenerateCheckBox();
        }
    }


    // The exit button clicked: Delete all QM objects under "State"
    private void ExitButtonClicked()
    {
        UnityEngine.Application.Quit();
    }


    // Go to Github Page
    private void HelpButtonClicked()
    {
        UnityEngine.Application.OpenURL("https://github.com/VisualMQ/VisualMQ");
    }


    // Open up authentication window
    private void ConnectButtonClicked()
    {
        Authentication.SetActive(true);
    }


    // Reset the camera
    private void ResetButtonClicked()
    {
        Debug.Log("RESET Camera position!");
        Camera.main.transform.position = new Vector3(0f, 0f, 0f);
        Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);
    }


    // Load QM Selector
    private void GenerateCheckBox()
    {
        // Destroy Previous Object
        DestroyQMSelector();

        // Get Current Number of Check Box
        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
        
        List<string> mqlist = stateComponent.RegisteredQMNameList();        

        foreach (string qmgrName in mqlist)
        {
            GameObject item = Instantiate(queueManagerRowItem, queueManagersList.transform.Find("QueueManagersList"));            
            
            item.transform.Find("TextQueueManager").GetComponent<Text>().text = qmgrName;

            // connect the toggle to the corresponding QM
            Toggle toggle = item.GetComponent<Toggle>();
            GameObject qm = GameObject.Find(qmgrName);
            toggle.onValueChanged.AddListener(delegate{
                ShowSelector(toggle, qm);
            });
        }

    }


    // for the toggle to control the appear of the QM
    private void ShowSelector(Toggle toggle, GameObject qm)
    {
        if(toggle.isOn)
        {
            qm.SetActive(true);
        }
        else
        {
            qm.SetActive(false);
        }
    }


    // Destroy previous QM selector
    private void DestroyQMSelector() {
        foreach (Transform child in queueManagersList.transform.Find("QueueManagersList")) 
        {
            if(child.gameObject.name == "QueueManagerRowItem(Clone)")
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }


    void Close()
    {
        queueManagersList.SetActive(false);
        buttonReset.gameObject.SetActive(false);
    }

}
