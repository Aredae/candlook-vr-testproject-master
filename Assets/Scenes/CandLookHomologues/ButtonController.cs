using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public GameObject canvas;
    public bool start = false;
    public void Clicked()
    {
        canvas.gameObject.SetActive(false);
        start = true;
    }
}
