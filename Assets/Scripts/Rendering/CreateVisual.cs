using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MQ;
public static class CreateVisual 
{

    private static readonly PrimitiveType QMObject = PrimitiveType.Cube;
    private static readonly PrimitiveType QueueObject = PrimitiveType.Cylinder;

    private static Dictionary<string, int> numberQueues = new Dictionary<string, int>();
    private static int numberManagers = 0;

    // Temporary:
    static int QMXVALUE = 15;
    public static void VisualizeQM(QMInfo manager)
    {   
        GameObject managerObj = GameObject.CreatePrimitive(QMObject);
        managerObj.transform.position = new Vector3(numberManagers * QMXVALUE, 0, 0); // Side by side
        managerObj.transform.localScale = new Vector3(10, 0.2f, 20);
        manager.obj = managerObj;
        // TODO: hardcoded values here change when prefabs created

        numberManagers++;
    }


    public static void VisualizeQueue(QMInfo manager, QueueStorage queue)
    {
        GameObject queueObj = GameObject.CreatePrimitive(QueueObject);
        queueObj.transform.parent = manager.obj.transform;
        queueObj.transform.localPosition = new Vector3(0.0f, 5.33f, (-0.45f) + 0.1f * (numberQueues.ContainsKey(manager.name) ? numberQueues[manager.name] : 0));
        queueObj.transform.localScale = new Vector3(0.1f, 5.0f, 0.05f);
        queue.obj = queueObj;

        Increment(numberQueues, manager.name);
    }

       
    public static void Increment<T>(this Dictionary<T, int> dictionary, T key)
    {
        int count = 0;
        dictionary.TryGetValue(key, out count);
        dictionary[key] = count + 1;
    }
}

