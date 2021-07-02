using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MQ;
public static class CreateVisual 
{   
    public static Dictionary<string, QMRender> renderedQMs = new Dictionary<string, QMRender>();
    public static State internalState;
    private static Dictionary<string, int> numberQueues = new Dictionary<string, int>();
    private static int numberManagers = 0;
    private static readonly PrimitiveType QMObject = PrimitiveType.Cube;
    private static readonly PrimitiveType QueueObject = PrimitiveType.Cylinder;
    static int QMXVALUE = 15;

    public class QMRender{
        public GameObject renderedObject;
        public QueueManager QMInfo;
        public Dictionary<string, QueueRender> renderedQueues;

        public QMRender(QueueManager QMinfo, GameObject renderedObject){
            this.renderedObject = renderedObject;
            this.QMInfo = QMinfo;
            this.renderedQueues = new Dictionary<string, QueueRender>();

        }
        

    }
    public class QueueRender{

        public QMRender parentQM;
        public MQ.Queue queueInfo;
        public GameObject renderedObject;
        public int positionRank = 1; // TODO later

        public QueueRender(QMRender parentQM, MQ.Queue queueInfo, GameObject renderedObject){
            this.parentQM = parentQM;
            this.queueInfo = queueInfo;
            this.renderedObject = renderedObject;
        }
    }

    
    public static void VisualizeQM(QueueManager manager)
    {   
        GameObject managerObj = GameObject.CreatePrimitive(QMObject);
        managerObj.transform.position = new Vector3(numberManagers * QMXVALUE, 0, 0); // Side by side
        managerObj.transform.localScale = new Vector3(10, 0.2f, 20);

        /* Now we add the render */
        QMRender qmrender = new QMRender(manager, managerObj);
        renderedQMs[manager.QMInfo.name] = qmrender; // We assume it is unique

        numberManagers++;
    }
    // Delete function here
    
    

    public static void VisualizeQueue(QueueManager manager, MQ.Queue queueInfo)
    {
        QMRender parentQM = renderedQMs[manager.QMInfo.name];

        GameObject queueObj = GameObject.CreatePrimitive(QueueObject);
        queueObj.transform.parent = parentQM.renderedObject.transform;
        queueObj.transform.localPosition = new Vector3(0.0f, 5.33f, (-0.45f) + 0.1f * (numberQueues.ContainsKey(manager.QMInfo.name) ? numberQueues[manager.QMInfo.name] : 0));
        queueObj.transform.localScale = new Vector3(0.1f, 5.0f, 0.05f);
        /* Now add the render */
        // THIS SHOULD NEVER GO WRONG ENSURE THIS:
        QueueRender queuerender = new QueueRender(parentQM, queueInfo, queueObj);
        

        Increment(numberQueues, manager.QMInfo.name);
    }

    public static void generate_render(){
        /* ACCESS INTERNAL STATE HERE */

        foreach (QueueManager manager in internalState.QMs)
        {
            if(!renderedQMs.ContainsKey(manager.QMInfo.name)){
                VisualizeQM(manager);
            }
            QMRender qmrender = renderedQMs[manager.QMInfo.name];
            
            foreach (MQ.Queue queue in manager.Queues)
            {
                if(!qmrender.renderedQueues.ContainsKey(queue.name)){
                    VisualizeQueue(manager,queue);
                }
            }
            // 1 2,3,4, 
        }
    }


    

    // Helper function   
    public static void Increment<T>(this Dictionary<T, int> dictionary, T key)
    {
        int count = 0;
        dictionary.TryGetValue(key, out count);
        dictionary[key] = count + 1;
    }
}

