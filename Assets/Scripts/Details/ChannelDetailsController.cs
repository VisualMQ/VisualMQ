using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChannelDetailsController : MonoBehaviour
{
    //public GameObject channelDetailsWindow;
    private Button closeButton;

    private Text textName, textType;
    
    private void Awake()
    {
        MouseListener.channelDetailsWindow = this;

        closeButton = transform.Find("ButtonClose").GetComponent<Button>();
        textName = transform.Find("TextGroup/Text1").GetComponent<Text>();
        textType = transform.Find("TextGroup/Text2").GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);

        closeButton.onClick.AddListener(Close);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Close()
    {
        Reset();
        gameObject.SetActive(false);
    }

    void Reset()
    {
        textName.text = "";
        textType.text = "";
    }

    public void GetChannelDetails(string qmgrName, string channelName)
    {

        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
        MQ.Channel channel = stateComponent.GetChannelDetails(qmgrName, channelName);
        

        textName.text = channel.channelName;
        textType.text = channel.channelType;

        gameObject.SetActive(true);
    }
}
