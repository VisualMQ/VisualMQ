using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationDetailsController : MonoBehaviour
{

    private Text textName, textType, textAppTag, textAppDesc;

    State stateComponent;


    private void Awake()
    {
        textName = transform.Find("Details/Name/TextName").GetComponent<Text>();
        textType = transform.Find("Details/Type/TextType").GetComponent<Text>();
        textAppTag = transform.Find("Details/Description/TextDescription").GetComponent<Text>();
        textAppDesc = transform.Find("Details/Tag/TextTag").GetComponent<Text>();

        // State component
        GameObject stateGameObject = GameObject.Find("State");
        stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
    }


    public void GetApplicationDetails(string qmgrName, string applicationName)
    {
        MQ.Application application = stateComponent.GetApplicationDetails(qmgrName, applicationName);

        textName.text = application.conn;
        textType.text = application.appltype;
        textAppTag.text = application.appltag;
        textAppDesc.text = application.appldesc;
    }
}