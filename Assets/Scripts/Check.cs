using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Check : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<RectTransform>().offsetMin = new Vector2(25, 0);
        GetComponent<RectTransform>().offsetMax = new Vector2(-25, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
