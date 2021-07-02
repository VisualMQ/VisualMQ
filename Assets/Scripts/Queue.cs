using UnityEngine;
using System.Collections;
using MQ;

public class Queue : MonoBehaviour
{

    public Vector3 position;
    public MQ.Queue queue;

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
        else
        {
            prefabName = "LocalQueue";
        }
        GameObject queuePrefab = Resources.Load(prefabName) as GameObject;
        GameObject instantiatedQueue = Instantiate(queuePrefab, position, Quaternion.identity) as GameObject;
        instantiatedQueue.transform.parent = this.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
