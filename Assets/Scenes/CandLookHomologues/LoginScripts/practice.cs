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
    public GameObject CreateSubjectCanvas;
    public GameObject CreateGroupCanvas;

    public GameObject subjectinputfield;
    public GameObject groupinputfield;

    public GameObject notsubmittedTextUser;
    public GameObject notsumbittedTextGroup;

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
    public GameObject toCreateSubjectButton;
    public GameObject toCreateGroupButton;
    public GameObject controllers;

    // Start is called before the first frame update
    void Start()
    {
        setupfinished = false;
        //clear default options
        groupsfilled = false;
        notsubmittedTextUser.SetActive(false);
        notsumbittedTextGroup.SetActive(false);
        CreateSubjectCanvas.SetActive(false);
        CreateGroupCanvas.SetActive(false);

        
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
        controllers.transform.position = xrrig.transform.position;
    }

    public void toCreateSubject()
    {
        SetupCanvas.SetActive(false);
        CreateGroupCanvas.SetActive(false);
        CreateSubjectCanvas.SetActive(true);
    }

    public void toCreateGroup()
    {
        SetupCanvas.SetActive(false);
        CreateSubjectCanvas.SetActive(false);
        CreateGroupCanvas.SetActive(true);
    }

    public void SubmitGroupandgoToCreateUser()
    {
        if (groupinputfield.transform.GetComponent<InputField>().text == "" || groupinputfield.transform.GetComponent<InputField>().text == null)
        {
            notsumbittedTextGroup.SetActive(true);
        }
        else
        {
            notsumbittedTextGroup.SetActive(false);
            //save group in DB
            //Repopulate group dropdowns
            gameObject.GetComponent<DatabaseUtil>().CreateNewGroup(groupinputfield.transform.GetComponent<InputField>().text);

            //change selected dropdown option of group dropdowns to created group
            gameObject.GetComponent<DatabaseUtil>().CreateGroups();
            toCreateSubject();
        }
    }

    public void SubmitUserAndGoToMainMenu()
    {
        if(subjectinputfield.transform.GetComponent<InputField>().text == "" || subjectinputfield.transform.GetComponent<InputField>().text == null)
        {
            notsubmittedTextUser.SetActive(true);
        }
        else
        {
            notsubmittedTextUser.SetActive(false);
            //Save user in set group to db
            gameObject.GetComponent<DatabaseUtil>().CreateNewSubject(groupinputfield.transform.GetComponent<InputField>().text, gameObject.GetComponent<DatabaseUtil>().currentGroup.id);

            //set subjectinfo instance to user object created
            Subject s = gameObject.GetComponent<DatabaseUtil>().Current;
            Subjectinfo.instance.SetSubjectInInfo(s);
            Subjectinfo.instance.SetNotes("Notes dont matter here");
            setupfinished = true;
            et.calibrate();
            Debug.Log(VarjoEyeTracking.GetGazeCalibrationQuality());
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ReturnToUserSelection()
    {
        CreateSubjectCanvas.SetActive(false);
        CreateGroupCanvas.SetActive(false);
        SetupCanvas.SetActive(true);
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

