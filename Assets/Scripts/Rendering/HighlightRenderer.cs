using UnityEngine;
using System.Collections.Generic;

public class HighlightRenderer : MonoBehaviour
{
    Color directColor = new Color32(255, 150, 0, 255);
    Color indirectColor = new Color32(150, 255, 0, 255);

    public void HighlightSelf(string objectName)
    {
        if (!gameObject.name.Equals(objectName))
        {
            return; // Not a clicked object
        }
        else
        {
            // the clicked object to be highlighted
            var outline = GameObject.Find(gameObject.name + ".Prefab").GetComponent<Outline>();
            if (outline == null)
            {
                outline = GameObject.Find(gameObject.name + ".Prefab").AddComponent<Outline>();
                outline.OutlineMode = Outline.Mode.OutlineVisible;
                outline.OutlineColor = Color.yellow;
                outline.OutlineWidth = 5f;
                outline.enabled = false;
            }
            outline.OutlineColor = Color.yellow;
            outline.enabled = true;
        }
    }

    public void HighlightDirect(List<string> dependency)
    {
        if (!dependency.Contains(gameObject.name))
        {
            return; // Not a direct dependency object
        }
        else
        {
            // a directly dependent object
            var outline = GameObject.Find(gameObject.name + ".Prefab").GetComponent<Outline>();
            if (outline == null)
            {
                outline = GameObject.Find(gameObject.name + ".Prefab").AddComponent<Outline>();
                outline.OutlineMode = Outline.Mode.OutlineVisible;
                outline.OutlineColor = Color.yellow;
                outline.OutlineWidth = 5f;
                outline.enabled = false;
            }
            outline.OutlineColor = directColor;
            outline.enabled = true;
        }
    }

    public void HighlightIndirect(List<string> dependency)
    {
        if (!dependency.Contains(gameObject.name))
        {
            return; // Not a direct dependency object
        }
        else
        {
            // an indirectly dependent object
            var outline = GameObject.Find(gameObject.name + ".Prefab").GetComponent<Outline>();
            if (outline == null)
            {
                outline = GameObject.Find(gameObject.name + ".Prefab").AddComponent<Outline>();
                outline.OutlineMode = Outline.Mode.OutlineVisible;
                outline.OutlineColor = Color.yellow;
                outline.OutlineWidth = 5f;
                outline.enabled = false;
            }
            outline.OutlineColor = indirectColor;
            outline.enabled = true;
        }
    }

    public void DisableHighlight()
    {
        var outline = GameObject.Find(gameObject.name + ".Prefab").GetComponent<Outline>();
        if (outline == null)
        {
            return; // no highlight yet
        } else
        {
            outline.enabled = false;
        }
    }

}
