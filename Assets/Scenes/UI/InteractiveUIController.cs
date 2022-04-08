using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveUIController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount == 1)
        {
            // move to within reach on startup

            gameObject.transform.position = Camera.main.transform.position + 0.4f * Camera.main.transform.forward;
            gameObject.transform.rotation = Camera.main.transform.rotation;
        }
    }
}
