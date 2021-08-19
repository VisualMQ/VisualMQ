using UnityEngine;
using UnityEngine.UI;


public class ChannelDetailsController : MonoBehaviour
{
    private Text textName, textType;
    private MQ.Channel currentChannel;
    private State stateComponent;


    private void Awake()
    {
        textName = transform.Find("Details/Name/TextName").GetComponent<Text>();
        textType = transform.Find("Details/Type/TextType").GetComponent<Text>();

        GameObject stateGameObject = GameObject.Find("State");
        stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
    }


    public void GetChannelDetails(string qmgrName, string channelName)
    {
        currentChannel = stateComponent.GetChannelDetails(qmgrName, channelName);
        
        textName.text = currentChannel.channelName;
        textType.text = currentChannel.channelType;
    }
}
