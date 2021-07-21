using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewSelector : MonoBehaviour
{
    public Camera mainCamera;
    public Camera topCamera;
    // public Camera frontCamera;
    // public Button buttoMain;
    // public Button buttoTop;
    // public Button buttoFront;

    // Start is called before the first frame update
    void Start()
    {
        topCamera.enabled = false;
        // frontCamera.enabled = false;
        // buttonMain.onClick.AddListener(ShowMainView);
        // buttonTop.onClick.AddListener(ShowOverheadView);
        // buttonFront.onClick.AddListener(ShowFrontView);
    }

    public void ShowMainView()
    {
        mainCamera.enabled = true;
        topCamera.enabled = false;
        // frontCamera.enabled = false;
    }

    public void ShowOverheadView()
    {
        mainCamera.enabled = false;
        topCamera.enabled = true;
        // frontCamera.enabled = false;
    }

    

    // public void ShowFrontView()
    // {
    //     mainCamera.enabled = false;
    //     topCamera.enabled = false;
    //     frontCamera.enabled = true;
    // }
    

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.M)){
            ShowMainView();
        }

        if (Input.GetKey(KeyCode.T))
        {
            ShowOverheadView();
        }
    }
}
