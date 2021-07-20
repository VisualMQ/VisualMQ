using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using MQ;



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
    public static QueueDetailsController QueueDetailWindow;

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

    /*
    A queue is selected
    */
    void OnMouseUp()
    {
        // If user clicks on UI objects, Return
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

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

        // A Queue is selected -> Show Info Panel
        List<string> temp = new List<string>() { this.parent.name, this.name };
        Debug.Log(this.parent.name + this.name);
        QueueDetailWindow.GetQueueBasicInfo(temp);

        CreateMessagePaths();

    }

    void CreateMessagePaths()
    {
        State state = GameObject.Find("State").GetComponent(typeof(State)) as State;
        List<string> testDependency = state.dependencyGraph.graph[this.name];
        if (testDependency.Count == 0)
        {
            return; //no dependency path
        }

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
        MeshRenderer queuePrefabMeshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
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