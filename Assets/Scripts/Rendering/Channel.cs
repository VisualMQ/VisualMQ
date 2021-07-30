using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MQ;

[RequireComponent(typeof(NameRenderer))]
public class Channel : MonoBehaviour
{

    public MQ.Channel channel;
    public Vector3 position;
    public QueueManager parent;
    public GameObject instantiatedChannel;
    // Use this for initialization
    void Start()
    {
        string prefabName;
        if (channel is MQ.SenderChannel)
        {
            prefabName = "Prefabs/SenderChannel";
        }
        else if (channel is MQ.ReceiverChannel)
        {
            prefabName = "Prefabs/ReceiverChannel";
        }
        else
        {
            prefabName = "not defined"; //TODO: throw an exception
        }
        GameObject channelPrefab = Resources.Load(prefabName) as GameObject;
        instantiatedChannel = Instantiate(channelPrefab, new Vector3(0, 0, 0), Quaternion.Euler(-90f, 180f, 0f)) as GameObject;
        instantiatedChannel.transform.parent = this.transform;
        instantiatedChannel.transform.parent.position = this.position;


        // Add mesh Colider
        MeshCollider mc = instantiatedChannel.transform.parent.gameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = instantiatedChannel.GetComponent<MeshFilter>().sharedMesh;
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnMouseUp()
    {
        // TODO: This code is copy pasted from Queue code. we should change this.
        Debug.Log("Mouse");
        // If user clicks on UI objects, Return: Avoid Click through
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }




        instantiatedChannel.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
        /*Do whatever here as per your need*/
        GameObject mainCamera = GameObject.Find("Main Camera");
        mainCamera.transform.rotation = Quaternion.identity;
        mainCamera.transform.rotation = Quaternion.AngleAxis(70, new Vector3(1, 0, 0));
        Vector3 targetPosition = this.transform.position;
        targetPosition.y += 18f;
        targetPosition.x += 10f;
        targetPosition.z -= 5f;
        mainCamera.transform.position = targetPosition;
        // Camera.main.transform.LookAt(this.position);
        // Camera.main.transform.position = new Vector3(2.5f, 15f, -13f) + this.position;
        Debug.Log("Moving Camera to Channel" + this.name);




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
    }


    void OnMouseEnter()
    {
        
        instantiatedChannel.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
    }
    void OnMouseExit()
    {
        instantiatedChannel.transform.localScale = new Vector3(1.00f, 1.00f, 1.00f);
    }

}
