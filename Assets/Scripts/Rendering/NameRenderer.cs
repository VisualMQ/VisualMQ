/* 
 * NameRenderer is a component attached to every rendered MQ entities
 * This component is used for rendering names (texts) above objects
 */
using UnityEngine;

public class NameRenderer : MonoBehaviour
{
    public string objectName;
    private TextMesh textMesh;
    private int DISTANCE_VISIBLE = 35; //The max euclidean distance between camera and text for name rendering

    // Use this for initialization
    void Start()
    {
        // Generate the initial text to be displayed above Queues.
        GameObject textObj = new GameObject("NameText", typeof(TextMesh));
        textObj.transform.parent = gameObject.transform;

        // This is quick and dirty hack: Since we have to rotate the Channel game object, so that
        // mesh collider is aligned with mesh, the Channel game object is rotated and we
        // need to position text with respect to different axis. See Channel object
        if (TryGetComponent(out Channel _))
        {
            textObj.transform.localPosition = new Vector3(0, 0, 5);
        } else
        {
            textObj.transform.localPosition = new Vector3(0, 5, 0);
        }
        
        textMesh = textObj.GetComponent<TextMesh>();
        textMesh.text = objectName;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(gameObject.transform.position, Camera.main.transform.position) < DISTANCE_VISIBLE)
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
        textMesh.characterSize = (0.4f / qmgr.queueManager.queues.Count) + 0.1f;

        // Rotate text to face main camera
        textMesh.transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - textMesh.transform.position) * Quaternion.Euler(0, 180, 0); ;
    }
}
