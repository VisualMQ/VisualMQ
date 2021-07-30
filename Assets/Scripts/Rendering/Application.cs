using UnityEngine;
using System.Collections;


[RequireComponent(typeof(NameRenderer))]
public class Application : MonoBehaviour
{

    public MQ.Application application;
    public Vector3 position;

    public GameObject applicationPrefab;


    void Awake()
    {
        applicationPrefab = Resources.Load("Prefabs/Application") as GameObject;
    }

    // Use this for initialization
    void Start()
    {
        GameObject instantiatedConn = Instantiate(applicationPrefab);
        instantiatedConn.transform.parent = this.transform;
        instantiatedConn.transform.localPosition = Vector3.zero;
    }

    // Detect Click on Application
    void Update()
    {
        
    }
}
