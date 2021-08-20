using UnityEngine;
using UnityEngine.UI;


public class ChannelDetailsController : MonoBehaviour
{
    private Text textName, textType, textConnName;
    private MQ.Channel currentChannel;
    private State stateComponent;


    private void Awake()
    {
        textName = transform.Find("Details/Name/TextName").GetComponent<Text>();
        textType = transform.Find("Details/Type/TextType").GetComponent<Text>();
        textConnName = transform.Find("Details/ConName/TextConName").GetComponent<Text>();

        GameObject stateGameObject = GameObject.Find("State");
        stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
    }


    public void GetChannelDetails(string qmgrName, string channelName)
    {
        currentChannel= stateComponent.GetChannelDetails(qmgrName, channelName);
        
        textName.text = currentChannel.channelName;
        textType.text = currentChannel.channelType;

        // If current channel is sender channel ->  show connection name
        if (currentChannel is MQ.SenderChannel senderChannel) {
            textConnName.text = senderChannel.connectionName;
        } else {
            textConnName.text = "N/A";
        }
    }
}
