using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QMDetailsRightViewController : MonoBehaviour
{
    private Transform QueueRowItem;
    private Transform container;


    // Start is called before the first frame update
    void Start()
    {
        container = transform.Find("QueueRowContainer");
        QueueRowItem = container.Find("QueueRowItem");
        
        QueueRowItem.gameObject.SetActive(false);
        float rowHeight = 45f;
        float startY = 0f;

        for (int i = 0; i < 10; i++)
        {
            Transform item = Instantiate(QueueRowItem, container);
            RectTransform recTransform = item.GetComponent<RectTransform>();
            recTransform.anchoredPosition = new Vector2(-3, -rowHeight * i+50);
            item.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
