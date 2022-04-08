using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonClicked : MonoBehaviour
{
    public void click()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject;
        button.transform.GetChild(3).gameObject.SetActive(true);
        int x = button.GetComponent<ButtonValue>().value;
        int nbot = (x >> 4) & 0xF;
        int ntop = (x >> 8) & 0xF;
        int letter = x & 1;
        if (nbot + ntop != 2 && letter != 1)
        {
            
        }
    }
}
