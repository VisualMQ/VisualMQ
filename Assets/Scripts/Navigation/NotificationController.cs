using System;
using UnityEngine;
using UnityEngine.UI;


public class NotificationController : MonoBehaviour
{
    // The notification objects
    public GameObject successNotification, errorNotification, infoNotification, warningNotification;
    // The close button of the notification object
    public Button closeSuccessNotification, closeErrorNotification, closeInfoNotification, closeWarningNotification;
    // The time text field
    public Text timeSuccessNotification, timeErrorNotification, timeInfoNotification, timeWarningNotification;
    // The main text field
    public Text messageSuccess, messageError, messageInfo, messageWarning;


    // Start is called before the first frame update
    void Start()
    {
        // Init to False
        successNotification.SetActive(false);
        errorNotification.SetActive(false);
        infoNotification.SetActive(false);
        warningNotification.SetActive(false);

        // Listen to close active
        closeSuccessNotification.onClick.AddListener(CloseWindowSuccess);
        closeErrorNotification.onClick.AddListener(CloseWindowError);
        closeInfoNotification.onClick.AddListener(CloseWindowInfo);
        closeWarningNotification.onClick.AddListener(CloseWindowWarning);
    }


    public void ShowSuccessNotification(string text)
    {
        successNotification.SetActive(true);
        timeSuccessNotification.text = (DateTime.Now).ToString();
        messageSuccess.text = text;
    }


    public void ShowErrorNotification(string text)
    {
        errorNotification.SetActive(true);
        timeErrorNotification.text = (DateTime.Now).ToString();
        messageError.text = text;
    }


    public void ShowInfoNotification(string text)
    {
        infoNotification.SetActive(true);
        timeInfoNotification.text = (DateTime.Now).ToString();
        messageInfo.text = text;
    }


    public void ShowWarningNotification(string text)
    {
        warningNotification.SetActive(true);
        timeWarningNotification.text = (DateTime.Now).ToString();
        messageWarning.text = text;
    }


    void CloseWindowSuccess()
    {
        successNotification.SetActive(false);
    }


    void CloseWindowError()
    {
        errorNotification.SetActive(false);
    }


    void CloseWindowInfo()
    {
        infoNotification.SetActive(false);
    }


    void CloseWindowWarning()
    {
        warningNotification.SetActive(false);
    }
    
}
