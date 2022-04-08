using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using Util;
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

    // Start is called before the first frame update
    void Start()
    {
        setupfinished = false;
        //clear default options
        groupsfilled = false;

        //SimpleSmoothMouseLook mouseController = xrrig.AddComponent<SimpleSmoothMouseLook>();
        //if (!UnityEngine.XR.XRSettings.isDeviceActive)
        //{
        //    SimpleSmoothMouseLook mouseController = xrrig.AddComponent<SimpleSmoothMouseLook>();
        //}
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
        SceneManager.LoadScene("Main Menu");
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

