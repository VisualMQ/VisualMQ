using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UserNameController : MonoBehaviour
{
    public InputField userName;
    public Text warningText;
    string currentUserName = "yuexu";

    // Start is called before the first frame update
    void Start()
    {

    }
    
    /*
    // Update is called once per frame
    void Update()
    {
        //currentUserName = userName.text;
        //Debug.Log(currentUserName);
    }
    */

    // return current username
    public string GetCurrentUserName()
    {
        currentUserName = userName.text;
        Debug.Log(currentUserName);
        return currentUserName;
    }
}

