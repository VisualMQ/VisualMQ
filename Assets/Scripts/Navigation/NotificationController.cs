using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NotificationController : MonoBehaviour
{
    // The Notification Object
    public GameObject successNotification, errorNotification, infoNotification, warningNotification;
    // The Close Button of the Notification Object
    public Button closeSuccessNotification, closeErrorNotification, closeinfoNotification, closewarningNotification;
    // The Time Text Field
    public Text timeSuccessNotification, timeErrorNotification, timeInfoNotification, timeWarningNotification;
    // The Main Text FIeld
    public Text messageSuccess, messageError, messageInfo, messageWarning;

    string currentDate = (DateTime.Now).ToString();

    // Start is called before the first frame update
    void Start()
    {
        // Init to False
        successNotification.SetActive(false);
        errorNotification.SetActive(false);
        infoNotification.SetActive(false);
        warningNotification.SetActive(false);

        // Listen to close active
        closeSuccessNotification.onClick.AddListener(closeWindowSuccess);
        closeErrorNotification.onClick.AddListener(closeWindowError);
        closeinfoNotification.onClick.AddListener(closeWindowInfo);
        closewarningNotification.onClick.AddListener(closeWindowWarning);
    }

    void CloseNotification()
    {
        successNotification.SetActive(false);
    }

    /*
    * Notification Window Generation for all 4 types of notification window
    */
    public void successMessageWindowGenerator(string info)
    {
        successNotification.SetActive(true);
        timeSuccessNotification.text = (DateTime.Now).ToString();
        messageSuccess.text = info;

    }

    public void errorMessageWindowGenerator(string info)
    {
        errorNotification.SetActive(true);
        timeErrorNotification.text = (DateTime.Now).ToString();
        messageError.text = info;
    }

    public void infoMessageWindowGenerator(string info)
    {
        infoNotification.SetActive(true);
        timeInfoNotification.text = (DateTime.Now).ToString();
        messageInfo.text = info;

    }

    public void warningMessageWindowGenerator(string info)
    {
        warningNotification.SetActive(true);
        timeWarningNotification.text = (DateTime.Now).ToString();
        messageWarning.text = info;
    }

    /*
    * Notification Close Button
    */
    void closeWindowSuccess()
    {
        successNotification.SetActive(false);
    }

    void closeWindowError()
    {
        errorNotification.SetActive(false);
    }

    void closeWindowInfo()
    {
        infoNotification.SetActive(false);
    }

    void closeWindowWarning()
    {
        warningNotification.SetActive(false);
    }
    
}

