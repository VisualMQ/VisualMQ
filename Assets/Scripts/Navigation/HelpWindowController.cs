using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpWindowController : MonoBehaviour
{
    public GameObject HelpWindow;
    private Button closeButton;
    private Text HelpBodyContent;

    private void Awake() 
    {
        // Link to Game Objects
        closeButton = transform.Find("ButtonHelpClose").GetComponent<Button>();
        HelpBodyContent = transform.Find("TextHelp").GetComponent<Text>();
        closeButton.onClick.AddListener(CloseHelpWindow);

        // Change Help Content Here
        HelpBodyContent.text = "Hello";
    }

    private void CloseHelpWindow()
    {
        HelpWindow.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

}
