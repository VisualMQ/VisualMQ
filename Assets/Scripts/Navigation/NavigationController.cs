using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationController : MonoBehaviour
{
    // Windows Gameobject
    public GameObject Authentication;
    public GameObject FilterWindow;
    
    // Buttons
    private Button expandQMSelector, authenticateNewQM, applyFilter;

    // Left Panel
    public GameObject leftPanel;
    private Transform checkboxItem;
    private Transform leftPanelContainer;

    private Dictionary<string, bool> QMVisibility = new Dictionary<string, bool>();

    private void Awake() 
    {
        // Locate the Game Obejct: Button
        expandQMSelector = transform.Find("ButtonExpandSidePanel").GetComponent<Button>();
        authenticateNewQM = transform.Find("ButtonConnectNewMQ").GetComponent<Button>();
        applyFilter = transform.Find("ButtonAddFilter").GetComponent<Button>();
        // Button Listener
        expandQMSelector.onClick.AddListener(LeftPanelButtonClicked);
        applyFilter.onClick.AddListener(AddFilterButtonClicked);

        // Locate the Game Object: Left Panel
        leftPanelContainer = transform.Find("LeftPanel");
        checkboxItem = leftPanelContainer.Find("QMSelectorRowItem");
        checkboxItem.gameObject.SetActive(false);

    }


    void Start()
    {
        // Default: Hide Auth, Filter, Left Panel Windows
        Authentication.SetActive(false); 
        leftPanel.SetActive(false);
        FilterWindow.SetActive(false);
    }


    /*
    * Click to open the left panel; Click to hide the left panel
    */
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

    /*
        Button Listener: Filter
    */
    void AddFilterButtonClicked()
    {
        FilterWindow.SetActive(true);
    }

    /*
        Load QM Selector
    */
    void GenerateCheckBox()
    {
        // Destroy Previous Object
        DestroyQMSelector();

        // Get Current Number of Check Box
        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
        
        List<string> mqlist = stateComponent.RegisteredQMNameList();

        int size = mqlist.Count;

        float rowHeight = 48f;
        float startY = -24f;

        for (int i = 0; i < size; i ++)
        {
            Transform item = Instantiate(checkboxItem, leftPanelContainer);
            RectTransform recTransform = item.GetComponent<RectTransform>();
            recTransform.anchoredPosition = new Vector2(0, -rowHeight * i + startY);
            item.gameObject.SetActive(true);
            
            item.Find("TextQMName").GetComponent<Text>().text = mqlist[i];
        }

    }

    void DestroyQMSelector() {
        foreach (Transform child in leftPanelContainer) 
        {
            if(child.gameObject.name == "QMSelectorRowItem(Clone)")
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

}
