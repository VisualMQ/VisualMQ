using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OODController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

// OOD Design

public class QueueManager
{
    public int QMId;
    // Constructor
    public QueueManager(){

    }


}

public class Queue:QueueManager
{
    public int queueId;


}

public class Messages: Queue
{
    public int messagesId;

}
