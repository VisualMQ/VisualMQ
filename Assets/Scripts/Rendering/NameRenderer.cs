using UnityEngine;
using System.Collections;



//This component is used for rendering names (text) above objects
//like queues, channels and applications
public class NameRenderer : MonoBehaviour
{

    public string objectName;
    private TextMesh textMesh;

    // Use this for initialization
    void Start()
    {
        // Generate the initial text to be displayed above Queues.
        GameObject textObj = new GameObject("NameText", typeof(TextMesh));
        textObj.transform.parent = gameObject.transform;
        textObj.transform.localPosition = new Vector3(0, 5, 0);
        textMesh = textObj.GetComponent<TextMesh>();
        textMesh.text = objectName;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
    }

    // Update is called once per frame
    void Update()
    {
        // Magic number (TODO) currently hides if euclidean distance between camera and text.
        if (Vector3.Distance(gameObject.transform.position, Camera.main.transform.position) < 35)
        {
            textMesh.gameObject.SetActive(true);
        }
        else
        {
            textMesh.gameObject.SetActive(false);
        }

        // Change the size log decreasing towards min of 0.2.
        if (textMesh == null)
        {
            return;
        }

        QueueManager qmgr = GetComponentInParent(typeof(QueueManager)) as QueueManager;
        textMesh.characterSize = (0.4f / qmgr.queues.Count) + 0.1f;

        // Obtain the middle Queue component and align all the text according to it.
        var firstIndex = qmgr.renderedQueues.GetEnumerator();
        Queue usedQueue = null;
        float distance = int.MaxValue;
        for (int i = 0; i < qmgr.renderedQueues.Count / 2; i++)
        {
            firstIndex.MoveNext();
            Queue firstQueue = firstIndex.Current.Value.GetComponent(typeof(Queue)) as Queue;

            if (Vector3.Distance(firstQueue.position, Camera.main.transform.position) < distance)
            {
                usedQueue = firstQueue;
                distance = Vector3.Distance(firstQueue.position, Camera.main.transform.position);
            }
        }
        TextMesh usedQueueTextMesh = usedQueue.GetComponentInChildren(typeof(TextMesh)) as TextMesh;
        textMesh.transform.rotation = Quaternion.LookRotation(usedQueueTextMesh.transform.position - Camera.main.transform.position);
    }
}
