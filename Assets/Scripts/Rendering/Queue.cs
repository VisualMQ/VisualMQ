using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MQ;


[RequireComponent(typeof(NameRenderer))]
[RequireComponent(typeof(HighlightRenderer))]
[RequireComponent(typeof(MouseListener))]
public class Queue : MonoBehaviour
{

    public Vector3 position;
    public MQ.Queue queue;
    public GameObject messagePrefab;
    public List<GameObject> messages;

    public QueueManager parent;
    public GameObject instantiatedQueue;
    public GameObject queuePrefab;   

    // Used for positioning
    public int rank;

    // Use MeshRenderer to change and update the Queue object
    private MeshRenderer queuePrefabMeshRenderer;

    // Use for flickering the Queue Icon
    private float flickerTime;

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

    public void repositionSelf(bool start = false)
    {

        this.position = this.parent.ComputePosition(this.queue.GetTypeName(), this.rank);
        this.instantiatedQueue.transform.parent = this.transform;

        if (start)
        {
            this.instantiatedQueue.transform.parent.position = new Vector3(this.position.x, -10, this.position.z);
            InvokeRepeating("createPositionAnimation", 1.0f, 0.01f);

            foreach (GameObject messageobj in this.messages)
            {
                messageobj.SetActive(false);
            }
        }
        else
        {
            this.instantiatedQueue.transform.parent.position = this.position;
        }

    }

    public void createPositionAnimation()
    {

        Vector3 endPosition = this.parent.ComputePosition(this.queue.GetTypeName(), this.rank);
        this.instantiatedQueue.transform.parent.position =
                Vector3.MoveTowards(this.instantiatedQueue.transform.parent.position,
                endPosition, Time.deltaTime * 22.0f // (Yes, Magic number) 
                );




        if (Vector3.Distance(transform.position, endPosition) < 0.001f)
        {
            foreach (GameObject messageobj in this.messages)
            {
                messageobj.SetActive(true);
            }
            CancelInvoke();
        }





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
        instantiatedQueue.name = this.name + ".Prefab";

        
        if (queue.holdsMessages)
        {
            int currentDepth = queue.currentDepth;
            // Assign Queue MeshRenderer here for rendering the color of Queue Icon
            queuePrefabMeshRenderer = GameObject.Find(this.name + ".Prefab").GetComponent<MeshRenderer>();

            Material[] queueMaterials = queuePrefabMeshRenderer.materials;

            CreateMessages(currentDepth, queue.maxNumberOfMessages);

        }

        repositionSelf(true);

        // Add mesh Colider
        MeshCollider mc = gameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = instantiatedQueue.GetComponent<MeshFilter>().sharedMesh;

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
        Material queueUtilisationColor = Resources.Load(messageColor) as Material;

        for (int i = 0; i < messagePrefabNum; i++)
        {
            Vector3 messagePosition = new Vector3(position.x, 0, position.z);
            messagePosition.y = 0 + (i + 1) * 0.2f;
            GameObject instantiatedMessage = Instantiate(messagePrefab, messagePosition, Quaternion.identity) as GameObject;
            instantiatedMessage.transform.parent = this.transform;
            instantiatedMessage.GetComponent<MeshRenderer>().material = queueUtilisationColor;

            messages.Add(instantiatedMessage);
        }

        // Change color of icon on top of queue to correspond to messages
        // This relies on the fact the Queue prefab is first in hieararchy
        // see GetComponentInChildren method
        Material[] queueMaterials = queuePrefabMeshRenderer.materials;
        Material[] newQueueMaterials = new Material[queueMaterials.Length];
        for (int i = 0; i < queueMaterials.Length; i++)
        {
            Material material = queueMaterials[i];
            // QueueBlue is the default utilisation color, so we know
            // we have to change this material
            if (material.name.Contains("QueueBlue"))
            {
                newQueueMaterials[i] = queueUtilisationColor;
            } else
            {
                newQueueMaterials[i] = material;
            }
        }
        queuePrefabMeshRenderer.materials = newQueueMaterials;
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