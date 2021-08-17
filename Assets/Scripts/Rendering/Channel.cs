using System;
using UnityEngine;


[RequireComponent(typeof(NameRenderer))]
[RequireComponent(typeof(HighlightRenderer))]
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
        else if (channel is MQ.ApplicationChannel)
        {
            prefabName = "Prefabs/ApplicationChannel";
        }
        else
        {
            throw new Exception("Unrecognized type of channel.");
        }
        GameObject channelPrefab = Resources.Load(prefabName) as GameObject;
        GameObject instantiatedChannel = Instantiate(channelPrefab) as GameObject;
        instantiatedChannel.transform.parent = gameObject.transform;
        instantiatedChannel.transform.localPosition = Vector3.zero;

        instantiatedChannel.name = this.name + ".Prefab";
        instantiatedChannel.transform.name = instantiatedChannel.name;

        // This is needed in order to rotate the model appropriately
        // Unfortunately, I couldn't find a way how to import it from Blender
        // so that this wouldn't be needed
        gameObject.transform.rotation = Quaternion.Euler(-90f, 180f, 0f);

        // Add mesh Colider
        MeshCollider mc = gameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = instantiatedChannel.GetComponent<MeshFilter>().sharedMesh;
    }

}
