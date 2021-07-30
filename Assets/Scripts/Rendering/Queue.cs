using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MQ;


[RequireComponent(typeof(NameRenderer))]
public class Queue : MonoBehaviour
{

    public Vector3 position;
    public MQ.Queue queue;
    public GameObject messagePrefab;
    public List<GameObject> messages;

    public QueueManager parent;
    public GameObject instantiatedQueue;
    public GameObject queuePrefab;
    public static GameObject queueInFocus;
    public static GameObject prefabInFocus;

    // TODO: REMOVE THIS LATER DEMO
    public static QueueDetailsController QueueDetailWindow;

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
        instantiatedQueue = Instantiate(queuePrefab, new Vector3(0,0,0), Quaternion.Euler(-90f, 0f, 0f)) as GameObject;
        repositionSelf();
        

        if (queue.holdsMessages)
        {
            int currentDepth = queue.currentDepth;
            // Assign Queue MeshRenderer here for rendering the color of Queue Icon
            queuePrefabMeshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
            CreateMessages(currentDepth, queue.maxNumberOfMessages);

        }
        
        // Add mesh Colider
        MeshCollider mc = instantiatedQueue.transform.parent.gameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = instantiatedQueue.GetComponent<MeshFilter>().sharedMesh;


        //var outline = gameObject.AddComponent<Outline>();
        
        //outline.OutlineMode = Outline.Mode.OutlineAll;
        //outline.OutlineColor = Color.yellow;
        //outline.OutlineWidth = 5f;

    }


    // Update is called once per frame
    void Update()
    {
        // The Queue Icon would flicker if the utilization is higher than 60%
        int currentDepth = queue.currentDepth;
        int MaximumDepth = queue.maxNumberOfMessages;
        flickerTime += Time.deltaTime;
        double utilization = (double)currentDepth / (double)MaximumDepth;
        if (utilization > 0.6)
        {
            Material queueIconColor;
            // The color of queue icon would switch every 0.5 seconds
            if (flickerTime % 1 > 0.618f) // Golden ratio :)
            {
                // Here the icon use BlockWhite material to distinguish with the QueueWhite material
                queueIconColor = Resources.Load("Materials/QueueWhite2") as Material;
            }
            else
            {
                queueIconColor = Resources.Load("Materials/QueueRed") as Material;
            }
            Material[] newQueueMaterials = new Material[queuePrefabMeshRenderer.materials.Length];
            for (int i = 0; i < queuePrefabMeshRenderer.materials.Length; i++)
            {
                Material material = queuePrefabMeshRenderer.materials[i];
                // Here there are three possible materials for the Queue Icon, use them to find and change its material
                if (material.name.Contains("QueueBlue") || material.name.Contains("QueueRed") || material.name.Contains("QueueWhite2"))
                {
                    newQueueMaterials[i] = queueIconColor;
                }
                else
                {
                    newQueueMaterials[i] = material;
                }

            }
            // Upate the Queue with new materials
            queuePrefabMeshRenderer.materials = newQueueMaterials;
        }

    }

    /*
    A queue is selected
    */
    void OnMouseUp()
    {
        // If user clicks on UI objects, Return: Avoid Click through
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Click on twice = Deactivate focus TODO: WHAT DOES THIS DO???
        //if (queueInFocus == instantiatedQueue)
        //{
        //    queueInFocus.transform.localScale = prefabInFocus.transform.localScale;
        //    queueInFocus = null;
        //    return;
        //}
        //// If clicked on once, it's now the new in focus. Reset the previous focus
        //if (queueInFocus)
        //{
        //    queueInFocus.transform.localScale = prefabInFocus.transform.localScale;
        //}

        queueInFocus = instantiatedQueue;
        prefabInFocus = instantiatedQueue;
        instantiatedQueue.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
        /*Do whatever here as per your need*/
        GameObject mainCamera = GameObject.Find("Main Camera");
        mainCamera.transform.rotation = Quaternion.identity;
        mainCamera.transform.rotation = Quaternion.AngleAxis(70, new Vector3(1, 0, 0));
        Vector3 targetPosition = this.transform.position;
        targetPosition.y += 18f;
        targetPosition.x += 10f;
        targetPosition.z -= 5f;
        mainCamera.transform.position =  targetPosition;
        // Camera.main.transform.LookAt(this.position);
        // Camera.main.transform.position = new Vector3(2.5f, 15f, -13f) + this.position;
        Debug.Log("Moving Camera to Queue" + this.name);

        // A Queue is selected -> Show Info Panel
        List<string> temp = new List<string>() { this.parent.name, this.name };
        Debug.Log(this.parent.name + this.name);
        QueueDetailWindow.GetQueueBasicInfo(temp);


        var outline = gameObject.GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 5f;
            outline.enabled = false;
        }

        
        outline.enabled = !outline.enabled;

        CreateMessagePaths();

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
                Debug.Log("Remote queue reached");
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
            } else if (waypointQueue.queue is MQ.AliasQueue)
            {
                Debug.Log("alias queue reached");
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