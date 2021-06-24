using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationController : MonoBehaviour
{
    public GameObject successNotification;
    public GameObject errorNotification;
    public GameObject infoNotification;
    public GameObject warningNotification;

    public Button closeSuccessNotification;
    public Button closeErrorNotification;
    public Button closeinfoNotification;
    public Button closewarningNotification;

    // Start is called before the first frame update
    void Start()
    {
        // Init to False
        successNotification.SetActive(false);
        errorNotification.SetActive(false);
        infoNotification.SetActive(false);
        warningNotification.SetActive(false);
        
        closeSuccessNotification.onClick.AddListener(closeWindowSuccess);
        closeErrorNotification.onClick.AddListener(closeWindowError);

        //closeinfoNotification.onClick.AddListener(closeWindow);
        //closewarningNotification.onClick.AddListener(closeWindow);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    * Notification Window Generation
    */
    void successMessageWindowGenerator(string info)
    {
        
    }

    /*
    * Notification Close Button
    */
    void closeWindowSuccess()
    {
        Debug.Log("Success Window closed.");
        successNotification.SetActive(false);
    }

    void closeWindowError()
    {
        Debug.Log("Error Window CLosed");
        errorNotification.SetActive(false);
    }
    
}
