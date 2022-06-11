using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using Varjo.XR;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    public GameObject xrrig;
    private VarjoEventManager em;
    private EyeTracker et;
    public GameObject canvas;
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;
    public GameObject button4;
    public GameObject button5;
    public GameObject button6;
    public GameObject button7;
    private List<GameObject> buttons;
    private int currselection;
    public GameObject controllers;


    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("SubjectInfo") != null)
        {
            Debug.Log(GameObject.Find("SubjectInfo").GetComponent<Subjectinfo>().GetId().ToString());
            Debug.Log(GameObject.Find("SubjectInfo").GetComponent<Subjectinfo>().GetName());
        }
        buttons = new List<GameObject>();
        buttons.Add(button1);
        buttons.Add(button2);
        buttons.Add(button3);
        buttons.Add(button4);
        buttons.Add(button5);
        buttons.Add(button6);
        buttons.Add(button7);
        if (!UnityEngine.XR.XRSettings.isDeviceActive)
        {
            //SimpleSmoothMouseLook mouseController = xrrig.AddComponent<SimpleSmoothMouseLook>();
        }
        else
        {
            em = VarjoEventManager.Instance;
            et = new VarjoET(Camera.main);
        }
        currselection = 0;
        //button1.GetComponent<Button>().GetComponent<Image>().color = Color.gray;
    }

    // Update is called once per frame
    void Update()
    {
        controllers.transform.position = xrrig.transform.position;
        /*
        if (Input.GetKeyDown(KeyCode.Space))
           et.calibrate();

        if (Input.GetKeyUp(KeyCode.Escape) && (xrrig.GetComponent<SimpleSmoothMouseLook>() != null))
        {
            Destroy(xrrig.GetComponent<SimpleSmoothMouseLook>());
        }

        if (UnityEngine.XR.XRSettings.isDeviceActive)
        {
            if (em.GetButtonDown(1) || Input.GetKeyUp(KeyCode.A))
            {
                if (currselection < 6)
                {
                   currselection++;
                   buttons[currselection].GetComponent<Button>().GetComponent<Image>().color = Color.gray;
                   buttons[currselection-1].GetComponent<Button>().GetComponent<Image>().color = Color.white;
                }
                else
                {
                    currselection = 0;
                    button1.GetComponent<Button>().GetComponent<Image>().color = Color.gray;
                    buttons[6].GetComponent<Button>().GetComponent<Image>().color = Color.white;
                }
            }
            if (em.GetButtonDown(0) || Input.GetKeyUp(KeyCode.Backspace))
            {
                buttons[currselection].GetComponent<Button>().onClick.Invoke();
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                if (currselection < 6)
                {
                    currselection++;
                    buttons[currselection].GetComponent<Button>().GetComponent<Image>().color = Color.gray;
                    buttons[currselection - 1].GetComponent<Button>().GetComponent<Image>().color = Color.white;
                }
                else
                {
                    currselection = 0;
                    button1.GetComponent<Button>().GetComponent<Image>().color = Color.gray;
                    buttons[6].GetComponent<Button>().GetComponent<Image>().color = Color.white;
                }
            }
            if (Input.GetKeyUp(KeyCode.Backspace))
            {
                buttons[currselection].GetComponent<Button>().onClick.Invoke();
            }
        }
        */
    }
        
}
