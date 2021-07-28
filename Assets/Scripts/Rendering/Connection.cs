using UnityEngine;
using System.Collections;


[RequireComponent(typeof(NameRenderer))]
public class Connection : MonoBehaviour
{

    public MQ.Connection connection;
    public Vector3 position;

    public GameObject connectionPrefab;


    void Awake()
    {
        connectionPrefab = Resources.Load("Prefabs/Connection") as GameObject;
    }

    // Use this for initialization
    void Start()
    {
        GameObject instantiatedConn = Instantiate(connectionPrefab);
        instantiatedConn.transform.parent = this.transform;
        instantiatedConn.transform.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
