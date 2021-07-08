using UnityEngine;
using System.Collections;
using MQ;

public class Queue : MonoBehaviour
{

    public Vector3 position;
    public MQ.Queue queue;
 
    public TextMesh textMesh;
    public QueueManager parent;

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
        GameObject instantiatedQueue = Instantiate(queuePrefab, new Vector3(0,0,0), Quaternion.identity) as GameObject;
        instantiatedQueue.transform.parent = this.transform;
        instantiatedQueue.transform.parent.position = position;
        // Add mesh Colider
        MeshCollider mc = instantiatedQueue.transform.parent.gameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = instantiatedQueue.GetComponent<MeshFilter>().sharedMesh;


        // TODO: Move this to prefab
        // Generate the initial text to be displayed above Queues.
        GameObject textObj = new GameObject();
        textObj.transform.parent = instantiatedQueue.transform;

        textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = queue.queueName;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.transform.position = new Vector3(instantiatedQueue.transform.position.x, instantiatedQueue.transform.position.y + 5, instantiatedQueue.transform.position.z);
    }


    // Update is called once per frame
    void Update()
    {

        // Magic number (TODO) currently hides if euclidean distance between camera and text.
        if(Vector3.Distance(this.position, Camera.main.transform.position) < 35)
        {
            textMesh.gameObject.SetActive(true);
        }
        else
        {
            textMesh.gameObject.SetActive(false);
        }

        // Change the size log decreasing towards min of 0.2.
        if(textMesh == null)
        {
            return;
        }
        textMesh.characterSize = (0.4f / parent.queues.Count) + 0.1f;

        // Obtain the middle Queue component and align all the text according to it.

        
        var firstIndex = parent.renderedQueues.GetEnumerator();
        Queue usedQueue = null;
        float distance = int.MaxValue;
        for (int i = 0; i < parent.renderedQueues.Count / 2; i++)
        {
            firstIndex.MoveNext();
            Queue firstQueue = firstIndex.Current.Value.GetComponent(typeof(Queue)) as Queue;

            if (Vector3.Distance(firstQueue.position, Camera.main.transform.position) < distance)
            {
                usedQueue = firstQueue;
                distance = Vector3.Distance(firstQueue.position, Camera.main.transform.position);
            }

        }

        

        textMesh.transform.rotation = Quaternion.LookRotation(usedQueue.textMesh.transform.position - Camera.main.transform.position);
        

       // textMesh.transform.rotation = Quaternion.LookRotation(textMesh.transform.position - Camera.main.transform.position);


    }

    void OnMouseUp()
    {
        /*Do whatever here as per your need*/
        Camera.main.transform.LookAt(this.position);
        Camera.main.transform.position = new Vector3(2.5f, 15f, -13f) + this.position;
        Debug.Log("Moving Camera to Queue" + this.name);
    }

}
