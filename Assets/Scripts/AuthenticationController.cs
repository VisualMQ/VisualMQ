using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticationController : MonoBehaviour
{
    public InputField userName, apiKey;
    public Button submit, cancel;
    public Text userUpLabel, userDownLabel;
    public Text apiUpLabel, apiDownLabel;

    //private GameObject buttonsCluster, usernameCluster, apiCluster;
    
    // Start is called before the first frame update
    void Start()
    {   
        // Initialisation
        userName = GetComponent<InputField>();
        apiKey = GetComponent<InputField>();
        submit = GetComponent<Button>();
        cancel = GetComponent<Button>();

        apiDownLabel.text = "You can find your API key at...";
        Debug.Log("Initialising the authentication field...");

        // Listen to button activity
        submit.onClick.AddListener(ConfirmButtonClicked);
        cancel.onClick.AddListener(CancelButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update Authentication");
    }

    void ConfirmButtonClicked()
    {
        //Output this to console when the Button3 is clicked
        Debug.Log("Comfirm Button clicked ");
    }

    void CancelButtonClicked()
    {
        Debug.Log("Cancel Button clicked");
    }
}
