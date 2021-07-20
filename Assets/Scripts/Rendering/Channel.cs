using UnityEngine;
using System.Collections;

public class Channel : MonoBehaviour
{

    public MQ.Channel channel;
    public Vector3 position;
    public QueueManager parent;
    public GameObject instantiatedChannel;
    // Use this for initialization
    void Start()
    {
        string prefabName;
        if (channel is MQ.SenderChannel)
        {
            prefabName = "Prefabs/SenderChannel";
        }
        else if (channel is MQ.ReceiverChannel)
        {
            prefabName = "Prefabs/ReceiverChannel";
        }
        else
        {
            prefabName = "not defined"; //TODO: throw an exception
        }
        GameObject channelPrefab = Resources.Load(prefabName) as GameObject;
        instantiatedChannel = Instantiate(channelPrefab, new Vector3(0, 0, 0), Quaternion.Euler(-90f, 180f, 0f)) as GameObject;
        instantiatedChannel.transform.parent = this.transform;
        instantiatedChannel.transform.parent.position = this.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
