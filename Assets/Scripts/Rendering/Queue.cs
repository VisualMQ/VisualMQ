using UnityEngine;
using System.Collections;
using MQ;

public class Queue : MonoBehaviour
{

    public Vector3 position;
    public MQ.Queue queue;
    public GameObject messagePrefab;

    void Awake()
    {
        messagePrefab = Resources.Load("Message") as GameObject;
    }

    // Use this for initialization
    void Start()
    {
        string prefabName;
        if (queue is MQ.RemoteQueue) 
        {
            prefabName = "RemoteQueue";
        }
        else if (queue is MQ.LocalQueue)
        {
            prefabName = "LocalQueue";
        }
        else if (queue is MQ.TransmissionQueue)
        {
            prefabName = "TransmissionQueue";
        }
        else if (queue is MQ.AliasQueue)
        {
            prefabName = "AliasQueue";
        }
        else
        {
            prefabName = "LocalQueue"; //TODO: undefined queue
        }
        GameObject queuePrefab = Resources.Load(prefabName) as GameObject;
        GameObject instantiatedQueue = Instantiate(queuePrefab, position, Quaternion.identity) as GameObject;
        instantiatedQueue.transform.parent = this.transform;

        if (queue is MQ.LocalQueue)
        {
            for (int i = 0; i < ((MQ.LocalQueue)queue).currentDepth; i++)
            {
                Vector3 messagePosition = position;
                messagePosition.y = position.y + (i+1) * 0.2f;
                GameObject instantiatedMessage = Instantiate(messagePrefab, messagePosition, Quaternion.identity) as GameObject;
                instantiatedMessage.transform.parent = this.transform;

                instantiatedMessage.GetComponent<Renderer>().material = Resources.Load("QueueBlue") as Material;
            }
        }
        else if (queue is MQ.TransmissionQueue)
        {
            for (int i = 0; i < ((MQ.TransmissionQueue)queue).currentDepth; i++)
            {
                Vector3 messagePosition = position;
                messagePosition.y = position.y + +0.5f + i * 0.2f;
                GameObject instantiatedMessage = Instantiate(messagePrefab, messagePosition, Quaternion.identity) as GameObject;
                instantiatedMessage.transform.parent = this.transform;

                instantiatedMessage.GetComponent<MeshRenderer>().material = Resources.Load("QueueBlue") as Material;
            }
        }
    }
    

    // Update is called once per frame
    void Update()
    {

    }
}
