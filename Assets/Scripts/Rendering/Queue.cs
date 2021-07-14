using UnityEngine;
using System.Collections;
using MQ;
using System.Collections.Generic;
using System;

public class Queue : MonoBehaviour
{

    public Vector3 position;
    public MQ.Queue queue;
    public GameObject messagePrefab;
    public List<GameObject> messages;


    public TextMesh textMesh;
    public QueueManager parent;
    public GameObject instantiatedQueue;
    public GameObject queuePrefab;
    public static GameObject queueInFocus;
    public static GameObject prefabInFocus;

    // TODO: REMOVE THIS LATER DEMO
    public static QMDetailsController tempWindow;

    // Used for positioning
    public int rank;

    void newQueueAdded(int rank)
    {
        if (rank < this.rank)
        {
            // If a new queue with a lower rank (position) added. 
            // Increase our own rank
            this.rank++;
            repositionSelf();

        }
    }

    void newQueueDeleted(int rank)
    {
 
        if(rank < this.rank)
        {
            // If a new queue with a lower rank (position) added. 
            // Increase our own rank
            Debug.Log("Decreased my rank");
            this.rank--;
            repositionSelf();

        }
    }

    public void repositionSelf()
    {
        this.position = QueueManager.ComputePosition(this.queue.GetTypeName(),this.rank);
        this.instantiatedQueue.transform.parent = this.transform;
        this.instantiatedQueue.transform.parent.position = this.position;
    }


    void Start()
    {
       
        string prefabName;
        if (queue is MQ.RemoteQueue)
        {
            prefabName = "Prefabs/RemoteQueue";
        }
        else if (queue is MQ.LocalQueue)
        {
            prefabName = "Prefabs/LocalQueue";
        }
        else if (queue is MQ.TransmissionQueue)
        {
            prefabName = "Prefabs/TransmissionQueue";
        }
        else if (queue is MQ.AliasQueue)
        {
            prefabName = "Prefabs/AliasQueue";
        }
        else
        {
            prefabName = "Prefabs/LocalQueue"; //TODO: undefined queue
        }
        queuePrefab = Resources.Load(prefabName) as GameObject;
        instantiatedQueue = Instantiate(queuePrefab, new Vector3(0,0,0), Quaternion.identity) as GameObject;

        // TODO: This is for dynamic positions.
        repositionSelf();
        

        if (queue.holdsMessages)
        {
            int currentDepth = queue.currentDepth;
            CreateMessages(currentDepth, queue.maxNumberOfMessages);

        }
        
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
        // TODO: SHOWQING QM DETAILS FIX DEMO TEST
        tempWindow.Clicked();
       


        // Click on twice = Deactivate focus
        if (queueInFocus == instantiatedQueue)
        {
            queueInFocus.transform.localScale = prefabInFocus.transform.localScale;
            queueInFocus = null;
            return;
        }
        // If clicked on once, it's now the new in focus. Reset the previous focus
        if (queueInFocus)
        {
            queueInFocus.transform.localScale = prefabInFocus.transform.localScale;
        }

        queueInFocus = instantiatedQueue;
        prefabInFocus = instantiatedQueue;
        instantiatedQueue.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
        /*Do whatever here as per your need*/
        Camera.main.transform.LookAt(this.position);
        Camera.main.transform.position = new Vector3(2.5f, 15f, -13f) + this.position;
        Debug.Log("Moving Camera to Queue" + this.name);
    }

    void OnMouseEnter()
    {
        instantiatedQueue.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);   
    }
    void OnMouseExit()
    {
        instantiatedQueue.transform.localScale = this.queuePrefab.transform.localScale;
    }


    void Awake()
    {
        messagePrefab = Resources.Load("Prefabs/Message") as GameObject;
        messages = new List<GameObject>();
    }
    
    void CreateMessages(int currentDepth, int MaximumDepth)
    {
        if (currentDepth == 0) return;

        double utilization = (double)currentDepth / (double)MaximumDepth;
        string messageColor;
        if (utilization > 0.6) //determine what color to render the message prefabs
        {
            messageColor = "Materials/QueueRed";
        }
        else if (utilization < 0.4)
        {
            messageColor = "Materials/QueueBlue";
        }
        else
        {
            messageColor = "Materials/QueueYellow";
        }

        int messagePrefabNum = (int)Math.Ceiling(utilization * 20);
        for (int i = 0; i < messagePrefabNum; i++)
        {
            Vector3 messagePosition = position;
            messagePosition.y = position.y + (i + 1) * 0.2f;
            GameObject instantiatedMessage = Instantiate(messagePrefab, messagePosition, Quaternion.identity) as GameObject;
            instantiatedMessage.transform.parent = this.transform;
            instantiatedMessage.GetComponent<MeshRenderer>().material = Resources.Load(messageColor) as Material;

            messages.Add(instantiatedMessage);
        }
    }

    public void UpdateMessages(int newDepth)
    {
        // First: destroy all old messages
        for (int i = 0; i < messages.Count; i++)
        {
            GameObject.DestroyImmediate(messages[i]);
        }

        // Second: Update to new messages
        if (queue.holdsMessages)
        {
            queue.currentDepth = newDepth;
            CreateMessages(newDepth, queue.maxNumberOfMessages);
        }
    }
}