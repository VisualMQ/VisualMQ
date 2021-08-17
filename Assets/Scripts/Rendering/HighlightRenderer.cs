using UnityEngine;
using System.Collections.Generic;


public class HighlightRenderer : MonoBehaviour
{
    private Color directColor = new Color32(56, 95, 231, 200);
    private Color indirectColor = new Color32(56, 160, 231, 200);
    private Color selectedColor = new Color32(56, 95, 231, 255);
    private Outline outline;


    public void HighlightSelf(string objectName)
    {
        if (!gameObject.name.Equals(objectName))
        {
            return; // Not a clicked object
        }

        // A selected object to be highlighted      
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

        // A directly dependent object
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

        // An indirectly dependent object
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
        if (outline == null)
        {
            return; // Object has not been highlighted yet
        }
        outline.enabled = false;
    }


    // We added special case of having a thicker outline for applications because
    // the outline was not very visible for them since they are rectangular blocks
    public void ChangeApplicationOutline(Outline outline)
    {
        if (TryGetComponent(out Application app))
        {
            outline.OutlineWidth = 8f;
        }
    }

}
