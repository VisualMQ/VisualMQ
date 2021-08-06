using UnityEngine;
using System.Collections.Generic;

public class HighlightRenderer : MonoBehaviour
{
    Color directColor = new Color32(56, 95, 231, 200);
    Color indirectColor = new Color32(56, 160, 231, 200);
    Color selectedColor = new Color32(56, 95, 231, 255);

    private Outline outline;

    public void HighlightSelf(string objectName)
    {
        if (!gameObject.name.Equals(objectName))
        {
            return; // Not a clicked object
        }

        // the clicked object to be highlighted
        //var outline = GameObject.Find(gameObject.name + ".Prefab").GetComponent<Outline>();
        
        if (outline == null)
        {
            outline = GameObject.Find(gameObject.name + ".Prefab").AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineVisible;
        }
        outline.OutlineColor = selectedColor;
        outline.OutlineWidth = 8f;
        outline.enabled = true;
        
    }

    public void HighlightDirect(List<string> dependency)
    {
        if (!dependency.Contains(gameObject.name))
        {
            return; // Not a direct dependency object
        }

        // a directly dependent object
        //Outline outline = GetComponentInChildren<Outline>();
        if (outline == null)
        {
            outline = GameObject.Find(gameObject.name + ".Prefab").AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineVisible;
        }
        outline.OutlineColor = directColor;
        outline.OutlineWidth = 4f;
        outline.enabled = true;

        ChangeApplicationOutline(outline);
    }

    public void HighlightIndirect(List<string> dependency)
    {
        if (!dependency.Contains(gameObject.name))
        {
            return; // Not a direct dependency object
        }

        // an indirectly dependent object
        //Outline outline = GetComponentInChildren<Outline>();
        if (outline == null)
        {
            outline = GameObject.Find(gameObject.name + ".Prefab").AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineVisible;
        }
        outline.OutlineColor = indirectColor;
        outline.OutlineWidth = 4f;
        outline.enabled = true;

        ChangeApplicationOutline(outline);
    }

    public void DisableHighlight()
    {
        //Outline outline = GetComponentInChildren<Outline>();
        if (outline == null)
        {
            return; // no highlight yet
        }
        outline.enabled = false;
    }

    public void ChangeApplicationOutline(Outline outline)
    {
        if (TryGetComponent(out Application app))
        {
            outline.OutlineWidth = 8f;
        }
    }

}
