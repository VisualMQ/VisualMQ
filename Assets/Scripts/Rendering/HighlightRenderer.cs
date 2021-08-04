﻿using UnityEngine;
using System.Collections.Generic;

public class HighlightRenderer : MonoBehaviour
{

    public void Highlight(List<string> objectDependency)
    {

        var outline = GameObject.Find(gameObject.name + ".Prefab").GetComponent<Outline>();
        if (objectDependency.Count == 0 || (objectDependency.Count == 1 && gameObject.name != objectDependency[objectDependency.Count - 1]))
        {
            if (outline == null)
            {
                return;
            } else
            {
                outline.enabled = false;
                return;
            }

        }
        if (outline == null)
        {
            outline = GameObject.Find(gameObject.name + ".Prefab").AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 5f;
            outline.enabled = false;
        }


        if (gameObject.name == objectDependency[objectDependency.Count - 1])
        {
            outline.OutlineColor = Color.yellow;
            outline.enabled = true;
            return;
        }
        else if (objectDependency.Contains(gameObject.name))
        {
            outline.OutlineColor = Color.cyan;
            outline.enabled = true;
            return;
        }
        else
        {
            outline.enabled = false;
        }
    }
}