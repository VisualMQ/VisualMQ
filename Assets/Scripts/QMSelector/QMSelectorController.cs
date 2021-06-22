using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QMSelectorController : MonoBehaviour
{
    public GameObject QM1;
    public GameObject QM2;
    public GameObject ToggleQM1;
    public GameObject ToggleQM2;
    public GameObject Authentication;

    // Start is called before the first frame update
    void Start()
    {
        QM1.SetActive(false);
        QM2.SetActive(false);
        ToggleQM1.SetActive(false);
        ToggleQM2.SetActive(false);
        Authentication.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
