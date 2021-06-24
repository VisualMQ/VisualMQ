using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationController : MonoBehaviour
{
    //public GameObject AuthenticationWindow;
    public Button connectNewQMButton;
    public Button addFilterButton;
    public Button expandPanelButton;

    public GameObject leftPanel;

    // Start is called before the first frame update
    void Start()
    {
        //AuthenticationWindow.SetActive(false);
        //connectNewQM.SetActive(false);
        //addFilter.SetActive(false);

        leftPanel.SetActive(false);
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void leftPanelButtonClicked()
    {

    }
}
