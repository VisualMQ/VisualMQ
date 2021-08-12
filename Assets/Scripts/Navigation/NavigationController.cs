using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationController : MonoBehaviour
{
    // Windows Gameobject
    public GameObject Authentication;
    
    // Buttons
    private Button expandQMSelector, authenticateNewQM, buttonExit, buttonHelp;

    // Left Panel and Container
    public GameObject leftPanel;
    private Transform checkboxItem;

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

        // Button Listener
        expandQMSelector.onClick.AddListener(LeftPanelButtonClicked);

        buttonExit.onClick.AddListener(ExitButtonClicked);
        buttonHelp.onClick.AddListener(HelpButtonClicked);

        authenticateNewQM.onClick.AddListener(ConnectButtonClicked);

        // Locate the Game Object: Left Panel
        checkboxItem = leftPanel.transform.Find("QueueManagerRowItem");
        checkboxItem.gameObject.SetActive(false);
    }


    private void Start()
    {
        // Default: Hide Auth, Filter, Left Panel Windows
        Authentication.SetActive(false); 
        leftPanel.SetActive(false);
    }


    // Click to open the left panel; Click to hide the left panel
    void LeftPanelButtonClicked()
    {
        if (leftPanel.activeSelf == true)
        {
            leftPanel.SetActive(false);
        }
        else
        {
            leftPanel.SetActive(true);
            GenerateCheckBox();
        }
    }

    // The exit button clicked: Delete all QM objects under "State"
    private void ExitButtonClicked()
    {
        UnityEngine.Application.Quit();

        /* Destroy the QM Object
        GameObject stateGameObject = GameObject.Find("State");

        foreach (Transform child in stateGameObject.transform)
        {
            Destroy(child.gameObject);
        }
        */ 
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


    // Load QM Selector
    void GenerateCheckBox()
    {
        // Destroy Previous Object
        DestroyQMSelector();

        // Get Current Number of Check Box
        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
        
        List<string> mqlist = stateComponent.RegisteredQMNameList();

        int size = mqlist.Count;

        float rowHeight = 43.1275f;
        float startX = 176.3f;
        float startY = -41.55f;
        

        for (int i = 0; i < size; i ++)
        {
            Transform item = Instantiate(checkboxItem, leftPanel.transform);
            RectTransform recTransform = item.GetComponent<RectTransform>();
            recTransform.anchoredPosition = new Vector2(startX, -rowHeight * i + startY);
            item.gameObject.SetActive(true);
            
            
            item.Find("TextQueueManager").GetComponent<Text>().text = mqlist[i];

            // connect the toggle to the corresponding QM
            Toggle toggle = item.GetComponent<Toggle>();
            GameObject qm = GameObject.Find(mqlist[i]);
            toggle.onValueChanged.AddListener(delegate{
                showSelector(toggle,qm);
            });
        }

    }


    // for the toggle to control the appear of the QM
    void showSelector(Toggle toggle, GameObject qm)
    {
        
        if(toggle.isOn)
        {
            qm.SetActive(true);
            Debug.Log("--- toggle is selected ---");
        }
        else
        {
            qm.SetActive(false);
            Debug.Log("--- toggle is not selected ---");
        }
    }

    void DestroyQMSelector() {
        foreach (Transform child in leftPanel.transform) 
        {
            if(child.gameObject.name == "QueueManagerRowItem(Clone)")
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }


    void Close()
    {
        leftPanel.SetActive(false);
    }

}
