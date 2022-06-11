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
using Newtonsoft.Json;
using UnityEngine.XR.Management;
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
    System.Action<string> _createNewSubjectCallback;
    private XRGeneralSettings initsettings;
    private bool returning;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("ACTIVE LOADER ON ENTER LOGIN: " + XRGeneralSettings.Instance.Manager.activeLoader.GetType().ToString());

        //List<XRLoader> loaders = XRGeneralSettings.Instance.Manager.loaders;

        //XRGeneralSettings.Instance.Manager.InitializeLoader();
        /*
        if (GameObject.Find("InitalLoaderState") != null)
        {
            if (GameObject.Find("InitalLoaderState").GetComponent<InitalLoaderState>().getReturning())
            {
                initsettings = GameObject.Find("InitalLoaderState").GetComponent<InitalLoaderState>().getSettings();

                //initsettings.Instance.Manager.InitializeLoader();
            }
            else
            {
                GameObject.Find("InitalLoaderState").GetComponent<InitalLoaderState>().setSettings();
            }
        }
        */
        /*
        for (int loaderIndex = loaders.Count - 1; loaderIndex >= 0; --loaderIndex)
        {
            XRLoader loader = loaders[loaderIndex];

            Debug.Log("Loader type: " + loader.GetType());
            if (loader.GetType().ToString() == "Unity.XR.OpenVR.OpenVRLoader")
            {
                loader.Initialize();
                loader.Start();
            }

            if (loader.GetType().ToString() == "Varjo.XR.VarjoLoader")
            {
                loader.Deinitialize();
                loader.Stop();
            }
            //  Debug.Log("Loader: "+ loader);
            //  loaders.RemoveAt(1);
            //  Debug.Log("Removed loader ");
        }
        */
        //List<XRLoader> loaders = XRGeneralSettings.Instance.Manager.loaders;
        Debug.Log("Active Loader here should be OpenVRLoader, active loader is: " + XRGeneralSettings.Instance.Manager.activeLoader.GetType().ToString());

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

        _createNewSubjectCallback = (jsonArray) =>
        {
            Subject Current = new Subject();
            List<Subject> sl = JsonConvert.DeserializeObject<List<Subject>>(jsonArray);
            Debug.Log(sl);
            foreach (Subject sh in sl)
            {
                Current = sh;
            }
            

            Subjectinfo.instance.SetSubjectInInfo(Current);

            Subjectinfo.instance.SetNotes("Notes dont matter here");
            setupfinished = true;

            //DEINITIALIZE ALL LOADERS THAT ARE NOT VARJO
            List<XRLoader> loaders = XRGeneralSettings.Instance.Manager.loaders;
            for (int loaderIndex = loaders.Count - 1; loaderIndex >= 0; --loaderIndex)
            {
                XRLoader loader = loaders[loaderIndex];

                Debug.Log("Loader type: " + loader.GetType());

                if (loader.GetType().ToString() == "Varjo.XR.VarjoLoader")
                {
                    
                    loader.Initialize();
                    loader.Start();

                }



                //  Debug.Log("Loader: "+ loader);
                //  loaders.RemoveAt(1);
                //  Debug.Log("Removed loader ");

            }

            /*
            foreach (XRLoader currloader in loaders)
            {
                if (currloader.GetType().ToString() == "Varjo.XR.VarjoLoader")
                {
                    currloader.Initialize();
                    currloader.Start();
                }
            }
            */
            Debug.Log("Varjo should now be initialized, but active loader should still be OpenVRLoader, active loader is: " + XRGeneralSettings.Instance.Manager.activeLoader.GetType().ToString());
            et.calibrate();
            Debug.Log(VarjoEyeTracking.GetGazeCalibrationQuality());
            SceneManager.LoadScene("MainMenu");

            

        };
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
            Debug.Log(gameObject.GetComponent<DatabaseUtil>().currentGroup.id.ToString());
            gameObject.GetComponent<DatabaseUtil>().CreateNewSubject(subjectinputfield.transform.GetComponent<InputField>().text, gameObject.GetComponent<DatabaseUtil>().currentGroup.id, _createNewSubjectCallback);

            
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
        List<XRLoader> loaders = XRGeneralSettings.Instance.Manager.loaders;
        for (int loaderIndex = loaders.Count - 1; loaderIndex >= 0; --loaderIndex)
        {
            XRLoader loader = loaders[loaderIndex];

            Debug.Log("Loader type: " + loader.GetType());

            if (loader.GetType().ToString() == "Varjo.XR.VarjoLoader")
            {

                loader.Initialize();
                loader.Start();

            }



            //  Debug.Log("Loader: "+ loader);
            //  loaders.RemoveAt(1);
            //  Debug.Log("Removed loader ");

        }

        /*
        foreach (XRLoader currloader in loaders)
        {
            if (currloader.GetType().ToString() == "Varjo.XR.VarjoLoader")
            {
                currloader.Initialize();
                currloader.Start();
            }
        }
        */
        Debug.Log("Active Loader here should be Varjo, active loader is: " + XRGeneralSettings.Instance.Manager.activeLoader.GetType().ToString());

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

