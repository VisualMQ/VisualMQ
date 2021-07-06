using UnityEngine;
using System.Collections;
using MQ;

public class Queue : MonoBehaviour
{

    public Vector3 position;
    public MQ.Queue queue;
    public Transform textMeshTransform;

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
            prefabName = "LocalQueue";
        }
        GameObject queuePrefab = Resources.Load(prefabName) as GameObject;
        GameObject instantiatedQueue = Instantiate(queuePrefab, position, Quaternion.identity) as GameObject;
        instantiatedQueue.transform.parent = this.transform;

        // TODO: Move this to prefab
        GameObject textObj = new GameObject();
        textObj.transform.parent = instantiatedQueue.transform;

        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = queue.queueName;
        textMesh.characterSize = 0.4f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.transform.position = new Vector3(instantiatedQueue.transform.position.x, instantiatedQueue.transform.position.y + 5, instantiatedQueue.transform.position.z);
        textMeshTransform = textMesh.transform;
    }


    // Update is called once per frame
    void Update()
    {
        textMeshTransform.rotation = Quaternion.LookRotation(textMeshTransform.position - Camera.main.transform.position);
    }
}
