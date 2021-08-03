using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MQ;

[RequireComponent(typeof(NameRenderer))]
[RequireComponent(typeof(MouseListener))]
public class Channel : MonoBehaviour
{

    public MQ.Channel channel;

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

        GameObject instantiatedChannel = Instantiate(channelPrefab) as GameObject;
        instantiatedChannel.transform.parent = gameObject.transform;
        instantiatedChannel.transform.localPosition = Vector3.zero;

        // This is needed in order to rotate the model appropriately
        // Unfortunately, I couldn't find a way how to import it from Blender
        // so that this wouldn't be needed
        gameObject.transform.rotation = Quaternion.Euler(-90f, 180f, 0f);

        // Add mesh Colider
        MeshCollider mc = gameObject.AddComponent<MeshCollider>();

        mc.sharedMesh = instantiatedChannel.GetComponent<MeshFilter>().sharedMesh;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

