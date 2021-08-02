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

    public void repositionSelf()
    {
        this.position = this.parent.ComputePosition(this.queue.GetTypeName(),this.rank);
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
        instantiatedQueue.name = this.name + ".Prefab";
        //var outline = instantiatedQueue.AddComponent<Outline>();
        //outline.OutlineMode = Outline.Mode.OutlineVisible;
        //outline.OutlineColor = Color.red;
        //outline.OutlineWidth = 5f;
        //outline.enabled = false;

        repositionSelf();
        

        if (queue.holdsMessages)
        {
            int currentDepth = queue.currentDepth;
            // Assign Queue MeshRenderer here for rendering the color of Queue Icon
            queuePrefabMeshRenderer = GameObject.Find(this.name + ".Prefab").GetComponent<MeshRenderer>();

            Material[] queueMaterials = queuePrefabMeshRenderer.materials;

            CreateMessages(currentDepth, queue.maxNumberOfMessages);

        }
        
        // Add mesh Colider
        MeshCollider mc = gameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = instantiatedQueue.GetComponent<MeshFilter>().sharedMesh;

    }


    // Update is called once per frame
    void Update()
    {
        //// The Queue Icon would flicker if the utilization is higher than 60%
        //int currentDepth = queue.currentDepth;
        //int MaximumDepth = queue.maxNumberOfMessages;
        //flickerTime += Time.deltaTime;
        //double utilization = (double)currentDepth / (double)MaximumDepth;
        //if (utilization > 0.6)
        //{
        //    Material queueIconColor;
        //    // The color of queue icon would switch every 0.5 seconds
        //    if (flickerTime % 1 > 0.618f) // Golden ratio :)
        //    {
        //        // Here the icon use BlockWhite material to distinguish with the QueueWhite material
        //        queueIconColor = Resources.Load("Materials/QueueWhite2") as Material;
        //    }
        //    else
        //    {
        //        queueIconColor = Resources.Load("Materials/QueueRed") as Material;
        //    }
        //    Material[] newQueueMaterials = new Material[queuePrefabMeshRenderer.materials.Length];
        //    for (int i = 0; i < queuePrefabMeshRenderer.materials.Length; i++)
        //    {
        //        Material material = queuePrefabMeshRenderer.materials[i];
        //        // Here there are three possible materials for the Queue Icon, use them to find and change its material
        //        if (material.name.Contains("QueueBlue") || material.name.Contains("QueueRed") || material.name.Contains("QueueWhite2"))
        //        {
        //            newQueueMaterials[i] = queueIconColor;
        //        }
        //        else
        //        {
        //            newQueueMaterials[i] = material;
        //        }

        //    }
        //    // Upate the Queue with new materials
        //    queuePrefabMeshRenderer.materials = newQueueMaterials;
        //}

    }


    void CreateMessagePaths()
    {
        State state = GameObject.Find("State").GetComponent(typeof(State)) as State;
        if (!state.dependencyGraph.graph.ContainsKey(this.name))
        {
            return; //no dependency for this queue
        }
        List<string> testDependency = state.dependencyGraph.graph[this.name];

        int idx = 0;
        while (idx < testDependency.Count)
        {
            Queue waypointQueue = GameObject.Find(testDependency[idx]).GetComponent(typeof(Queue)) as Queue;
            if (waypointQueue.queue is MQ.RemoteQueue)
            {
                GameObject path = new GameObject("Path", typeof(PathCreation.AutoPathGenerator));
                PathCreation.AutoPathGenerator pathGenerator = path.GetComponent(typeof(PathCreation.AutoPathGenerator)) as PathCreation.AutoPathGenerator;
                List<Transform> testTransform = new List<Transform>();
                for (int i = idx; i < idx + 5; i++)
                {
                    GameObject waypointObject = GameObject.Find(testDependency[i]);
                    if (waypointObject == null)
                    {
                        break;
                    }
                    testTransform.Add(waypointObject.transform);
                }
                pathGenerator.waypoints = testTransform.ToArray();

                //create "message" object
                GameObject follower = GameObject.CreatePrimitive(PrimitiveType.Cube);
                follower.AddComponent(typeof(PathCreation.PathFollower));
                PathCreation.PathFollower followGenerator = follower.GetComponent(typeof(PathCreation.PathFollower)) as PathCreation.PathFollower;

                //have this message object follow the path defined in path object
                followGenerator.pathCreator = path.GetComponent(typeof(PathCreation.PathCreator)) as PathCreation.PathCreator;
                followGenerator.endOfPathInstruction = PathCreation.EndOfPathInstruction.Loop;

                //increment idx
                idx += 5;
            }
            else if (waypointQueue.queue is MQ.AliasQueue)
            {
                GameObject path = new GameObject("Path", typeof(PathCreation.AutoPathGenerator));
                PathCreation.AutoPathGenerator pathGenerator = path.GetComponent(typeof(PathCreation.AutoPathGenerator)) as PathCreation.AutoPathGenerator;
                List<Transform> testTransform = new List<Transform>();
                for (int i = idx; i < idx + 2; i++)
                {
                    GameObject waypointObject = GameObject.Find(testDependency[i]);
                    Debug.Log("Current Waypoint Object is: " + waypointObject);
                    if (waypointObject == null)
                    {
                        break;
                    }
                    testTransform.Add(waypointObject.transform);
                }
                pathGenerator.waypoints = testTransform.ToArray();

                //create "message" object
                GameObject follower = GameObject.CreatePrimitive(PrimitiveType.Cube);
                follower.AddComponent(typeof(PathCreation.PathFollower));
                PathCreation.PathFollower followGenerator = follower.GetComponent(typeof(PathCreation.PathFollower)) as PathCreation.PathFollower;

                //have this message object follow the path defined in path object
                followGenerator.pathCreator = path.GetComponent(typeof(PathCreation.PathCreator)) as PathCreation.PathCreator;
                followGenerator.endOfPathInstruction = PathCreation.EndOfPathInstruction.Loop;

                //increment idx
                idx += 2;
            } else
            {
                throw new Exception();
            }
        }
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
            Vector3 messagePosition = position;
            messagePosition.y = position.y + (i + 1) * 0.2f;
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