using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class APIController : MonoBehaviour
{
    public InputField api;
    public Text warningAPI;
    string currentAPIKey = "example key";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetCurrentAPIKey()
    {
        currentAPIKey = api.text;
        return currentAPIKey;
    }
}
