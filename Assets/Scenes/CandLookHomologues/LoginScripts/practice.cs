using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using Util;
using Varjo.XR;
using static Varjo.XR.VarjoEyeTracking;
//using Npgsql;

public class practice : MonoBehaviour
{
    //public SubjectInfo sinfo;
    private DatabaseUtil db;
    public GameObject SetupCanvas;
    public GameObject DropdownGroup;
    public GameObject DropdownSubject;
    public GameObject Notes;
    public GameObject BeginButton;
    private bool setupfinished;
    //private List<Group> grouplist;
    private bool groupsfilled;
    public GameObject xrrig;
    private VarjoEventManager em;
    private EyeTracker et;
    // Start is called before the first frame update
    void Start()
    {
        setupfinished = false;
        //clear default options
        groupsfilled = false;

        
        if (!UnityEngine.XR.XRSettings.isDeviceActive)
        {
            SimpleSmoothMouseLook mouseController = xrrig.AddComponent<SimpleSmoothMouseLook>();
        }
        em = VarjoEventManager.Instance;
        et = new VarjoET(Camera.main);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void beginclick()
    {
        Subject s = gameObject.GetComponent<DatabaseUtil>().Current;
        Subjectinfo.instance.SetSubjectInInfo(s);
        Subjectinfo.instance.SetNotes("Notes dont matter here");
        setupfinished = true;
        et.calibrate();
        Debug.Log(VarjoEyeTracking.GetGazeCalibrationQuality());
        SceneManager.LoadScene("MainMenu");
    }

    public void restart()
    {
        SceneManager.LoadScene("Practice");
    }

    

    public void CreateButton(Transform panel, Vector3 position, Vector2 size, UnityEngine.Events.UnityAction method)
    {
        GameObject button = new GameObject();
        button.transform.parent = panel;
        button.AddComponent<RectTransform>();
        button.AddComponent<Button>();
        button.transform.position = position;
        button.GetComponent<RectTransform>().sizeDelta = size;
        button.GetComponent<Button>().onClick.AddListener(method);
    }

}

