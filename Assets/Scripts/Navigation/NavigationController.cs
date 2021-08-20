using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NavigationController : MonoBehaviour
{
    // Windows Gameobject
    public GameObject authenticationWindow;
    
    // Buttons
    private Button expandQMSelector, authenticateNewQM, buttonExit, buttonHelp, buttonReset;

    // Left Panel and Container
    private GameObject viewDropdown;
    private GameObject queueManagersList;
    private GameObject queueManagerRowItem;


    // Locate the Objects
    private void Awake() 
    {
        // Locate the Game Obejct: Button
        expandQMSelector = transform.Find("ButtonView").GetComponent<Button>();
        authenticateNewQM = transform.Find("ButtonConnect").GetComponent<Button>();
        buttonExit = transform.Find("ButtonExit").GetComponent<Button>();
        buttonHelp = transform.Find("ButtonHelp").GetComponent<Button>();
        buttonReset = transform.Find("ViewDropdown/ButtonReset").GetComponent<Button>();

        // Button Listener
        expandQMSelector.onClick.AddListener(ViewButtonClicked);
        authenticateNewQM.onClick.AddListener(ConnectButtonClicked);
        buttonExit.onClick.AddListener(ExitButtonClicked);
        buttonHelp.onClick.AddListener(HelpButtonClicked);
        buttonReset.onClick.AddListener(ResetButtonClicked);

        // Resources for queue manager list dropdown
        viewDropdown = transform.Find("ViewDropdown").gameObject;
        queueManagersList = transform.Find("ViewDropdown/QueueManagersList").gameObject;
        queueManagerRowItem = Resources.Load("Prefabs/QueueManagerRowItem") as GameObject;
    }


    private void Start()
    {
        authenticationWindow.SetActive(false); 
        viewDropdown.SetActive(false);
    }


    // Click to open the left panel; Click to hide the left panel
    private void ViewButtonClicked()
    {
        if (viewDropdown.activeSelf == true)
        {
            viewDropdown.SetActive(false);
        }
        else
        {
            viewDropdown.SetActive(true);
            GenerateQmgrCheckboxes();
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
        authenticationWindow.SetActive(true);
    }


    // Reset the camera
    private void ResetButtonClicked()
    {
        Camera.main.transform.position = new Vector3(0f, 0f, -20f);
        Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);
    }


    // Load QM Selector
    private void GenerateQmgrCheckboxes()
    {
        // Destroy Previous Object
        DestroyQmgrCheckboxes();

        // Get Current Number of Check Box
        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
        
        List<string> qmgrList = stateComponent.GetRegisteredQueueManagers();        

        foreach (string qmgrName in qmgrList)
        {
            GameObject item = Instantiate(queueManagerRowItem, queueManagersList.transform.Find("QueueManagersList"));            
            
            item.transform.Find("TextQueueManager").GetComponent<Text>().text = qmgrName;

            GameObject qmgr = null;
            foreach (MQ.Client client in stateComponent.qmgrs.Keys)
            {
                if (client.GetQueueManagerName() == qmgrName)
                {
                    qmgr = stateComponent.qmgrs[client];
                }
            }

            if (qmgr == null)
            {
                continue; // Queue manager not rendered yet
            }

            // Connect the toggle to the corresponding queue manager
            Toggle toggle = item.GetComponent<Toggle>();

            if (!qmgr.activeInHierarchy)
            {
                toggle.isOn = !toggle.isOn; // If QM is not displayed, disable the toggle as well
            }

            toggle.onValueChanged.AddListener(delegate{
                ShowSelector(toggle, qmgr);
            });
        }
    }


    // For the toggle to control the display of the queue manager
    private void ShowSelector(Toggle toggle, GameObject qmgr)
    {
        if(toggle.isOn)
        {
            qmgr.SetActive(true);
        }
        else
        {
            qmgr.SetActive(false);
        }
    }


    // Destroy previous QM selectors
    private void DestroyQmgrCheckboxes() {
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
        viewDropdown.SetActive(false);
    }

}
