using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ReplayController : MonoBehaviour
{
    public GameObject controllers;
    public GameObject xrrig;
    WebRequest webrequest = new WebRequest();
    public GameObject usertext;

    public GameObject Canvas;
    private System.Action<string> _createGetTaskNamesCallback;
    private System.Action<string> _createGetD2ResultsCallback;
    public GameObject initbutton;
    private string[] gameparameters;
    public GameObject TaskNameText;
    public GameObject RecordingTimeText;
    public GameObject ExtraInfo1Text;
    public GameObject ExtraInfo2Text;
    public GameObject ExtraInfo3Text;
    public GameObject ExtraInfo4Text;
    public GameObject ExtraInfo5Text;
    public GameObject ExtraInfo6Text;
    public GameObject ExtraInfo7Text;
    public GameObject ExtraInfo8Text;
    public GameObject ExtraInfo9Text;
    public GameObject ExtraInfo10Text;
    public GameObject ToReplayButton;
    public GameObject Versiontext;
    private GameObject previouslySelectedGameObject;
    public GameObject DetailsForReplay;
    private GameObject lastbutton;
    private Subjectinfo subject;
    private int i;

    // Start is called before the first frame update
    void Start()
    {
        i = 0;
        if (GameObject.Find("SubjectInfo") != null)
        {
            subject = GameObject.Find("SubjectInfo").GetComponent<Subjectinfo>();
            usertext.transform.GetComponent<Text>().text += " " + subject.GetName();
        }

        TaskNameText.SetActive(false);
        RecordingTimeText.SetActive(false);
        ExtraInfo1Text.SetActive(false);
        ExtraInfo2Text.SetActive(false);
        ExtraInfo3Text.SetActive(false);
        ExtraInfo4Text.SetActive(false);
        ExtraInfo5Text.SetActive(false);
        ExtraInfo6Text.SetActive(false);
        ExtraInfo7Text.SetActive(false);
        ExtraInfo8Text.SetActive(false);
        ExtraInfo9Text.SetActive(false);
        ExtraInfo10Text.SetActive(false);
        ToReplayButton.SetActive(false);
        Versiontext.SetActive(false);
        initbutton.SetActive(false);


        //TODO php and stuff for getting task names connected to user
        _createGetTaskNamesCallback = (jsonArray) => {
            //Separate entries and store in array
            List<GameInfo> recordingslist = GameInfoFromJson(jsonArray);
            Debug.Log(recordingslist[0].ToString());
            if(recordingslist != null)
            {
                foreach (GameInfo g in recordingslist)
                {
                    //Generate Button for each game
                    GameObject newButton = DefaultControls.CreateButton(
                    new DefaultControls.Resources()
                    );
                    newButton.transform.SetParent(Canvas.transform, false);
                    newButton.transform.position = initbutton.transform.position;
                    newButton.GetComponent<RectTransform>().sizeDelta = new Vector2(initbutton.GetComponent<RectTransform>().rect.width, initbutton.GetComponent<RectTransform>().rect.height);

                    newButton.transform.position = new Vector3(initbutton.transform.position.x, initbutton.transform.position.y - (initbutton.GetComponent<RectTransform>().rect.height *i)/200, initbutton.transform.position.z);
                    string[] stringtab = g.game_name.Split('_');
                    Debug.Log(stringtab[0]);
                    try
                    {
                        newButton.transform.GetChild(0).GetComponent<Text>().text = stringtab[0] + " Timestamp: " + g.recordingtime;
                        newButton.transform.GetChild(0).GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                        newButton.transform.GetChild(0).GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
                    }
                    catch (NullReferenceException e)
                    {
                        Debug.Log(newButton.GetComponent<Text>().text);
                    }

                    //Add differnt OnClick Events for eacn button
                    newButton.GetComponent<Button>().onClick.AddListener(ButtonClickEvent);
                    newButton.AddComponent<GameInfoMono>();
                    newButton.GetComponent<GameInfoMono>().GameName = g.game_name;
                    newButton.GetComponent<GameInfoMono>().Version = g.game_version;
                    newButton.GetComponent<GameInfoMono>().timestamp = g.recordingtime;
                    
                    if (i == recordingslist.Count - 1)
                    {
                        lastbutton = newButton;
                    }
                    
                    i++;
                }
            }
            
        };

        //returns all d2 results, enter d2 results after everything else?
        //TODO php and stuff for getting d2 results of user
        _createGetD2ResultsCallback = (jsonArray) =>
        {
            List<ResultEntity> d2resultlist = D2ResultFromJson(jsonArray);
            if(d2resultlist != null)
            {
                foreach (ResultEntity r in d2resultlist)
                {
                    GameObject newButton = DefaultControls.CreateButton(
                        new DefaultControls.Resources()
                        );
                    newButton.transform.SetParent(Canvas.transform, false);
                    newButton.transform.position = lastbutton.transform.position;
                    initbutton.transform.position = new Vector3(initbutton.transform.position.x, initbutton.transform.position.y - (50 * i), initbutton.transform.position.z);
                    newButton.GetComponent<Text>().text = "D2 test, Recorded on:" + r.start_date;
                    newButton.GetComponent<Button>().onClick.AddListener(D2ResultButtonClicked);
                    newButton.AddComponent<ResultEntityMono>();
                    newButton.GetComponent<ResultEntityMono>().subject_id = r.subject_id;
                    newButton.GetComponent<ResultEntityMono>().notes = r.notes;
                    newButton.GetComponent<ResultEntityMono>().start_date = r.start_date;
                    newButton.GetComponent<ResultEntityMono>().tn = r.tn;
                    newButton.GetComponent<ResultEntityMono>().e = r.e;
                    newButton.GetComponent<ResultEntityMono>().tn_e = r.tn_e;
                    newButton.GetComponent<ResultEntityMono>().e1 = r.e1;
                    newButton.GetComponent<ResultEntityMono>().e2 = r.e2;
                    newButton.GetComponent<ResultEntityMono>().e_percent = r.e_percent;
                    newButton.GetComponent<ResultEntityMono>().cp = r.cp;
                    newButton.GetComponent<ResultEntityMono>().fr = r.fr;
                    newButton.GetComponent<ResultEntityMono>().ed = r.ed;
                    newButton.GetComponent<ResultEntityMono>().d2 = r.d2;
                    if (i == d2resultlist.Count - 1)
                    {
                        lastbutton = newButton;
                    }
                    i++;
                }
            }
        };

        if (GameObject.Find("SubjectInfo") != null)
        {
            GetRecordings(subject.GetId());
            GetD2Results(subject.GetId());
        }
    }

    public void GetRecordings(int userid)
    {
        StartCoroutine(webrequest.getRecordingsFromUser("http://localhost/GetRecordingsFromUser.php", userid, _createGetTaskNamesCallback));
    }

    public void GetD2Results(int userid)
    {
        StartCoroutine(webrequest.getRecordingsFromUser("http://localhost/getD2ResultsFromUser.php", userid, _createGetD2ResultsCallback));
    }

    
    void D2ResultButtonClicked()
    {
        ToReplayButton.SetActive(false);
        previouslySelectedGameObject = EventSystem.current.currentSelectedGameObject;
        GameObject currentbutton = EventSystem.current.currentSelectedGameObject;
        TaskNameText.GetComponent<Text>().text = "Task: D2 Test";
        RecordingTimeText.GetComponent<Text>().text = "Date of Recording: " + currentbutton.GetComponent<ResultEntityMono>().start_date;
        Versiontext.GetComponent<Text>().text = "Results:";
        ExtraInfo1Text.GetComponent<Text>().text = "TN ->" + currentbutton.GetComponent<ResultEntityMono>().tn;
        ExtraInfo2Text.GetComponent<Text>().text = "E ->" + currentbutton.GetComponent<ResultEntityMono>().e;
        ExtraInfo3Text.GetComponent<Text>().text = "TN-E ->" + currentbutton.GetComponent<ResultEntityMono>().tn_e;
        ExtraInfo4Text.GetComponent<Text>().text = "E1 ->" + currentbutton.GetComponent<ResultEntityMono>().e1;
        ExtraInfo5Text.GetComponent<Text>().text = "E2 ->" + currentbutton.GetComponent<ResultEntityMono>().e2;
        ExtraInfo6Text.GetComponent<Text>().text = "E Percent ->" + currentbutton.GetComponent<ResultEntityMono>().e_percent;
        ExtraInfo7Text.GetComponent<Text>().text = "CP ->" + currentbutton.GetComponent<ResultEntityMono>().cp;
        ExtraInfo8Text.GetComponent<Text>().text = "FR ->" + currentbutton.GetComponent<ResultEntityMono>().fr;
        ExtraInfo9Text.GetComponent<Text>().text = "ED ->" + currentbutton.GetComponent<ResultEntityMono>().ed;
        ExtraInfo10Text.GetComponent<Text>().text = "D2 ->" + currentbutton.GetComponent<ResultEntityMono>().d2;

        TaskNameText.SetActive(true);
        RecordingTimeText.SetActive(true);
        Versiontext.SetActive(true);
        ExtraInfo1Text.SetActive(true);
        ExtraInfo2Text.SetActive(true);
        ExtraInfo3Text.SetActive(true);
        ExtraInfo4Text.SetActive(true);
        ExtraInfo5Text.SetActive(true);
        ExtraInfo6Text.SetActive(true);
        ExtraInfo7Text.SetActive(true);
        ExtraInfo8Text.SetActive(true);
        ExtraInfo9Text.SetActive(true);
        ExtraInfo10Text.SetActive(true);


    }

    void ButtonClickEvent()
    {
        //if()
        previouslySelectedGameObject = EventSystem.current.currentSelectedGameObject;
        ToReplayButton.GetComponent<Button>().onClick.RemoveAllListeners();
        gameparameters = EventSystem.current.currentSelectedGameObject.GetComponent<GameInfoMono>().GameName.Split('_');
        TaskNameText.GetComponent<Text>().text = "Task: " + gameparameters[0];
        RecordingTimeText.GetComponent<Text>().text = "Timestamp: " + EventSystem.current.currentSelectedGameObject.GetComponent<GameInfoMono>().timestamp;
        Versiontext.GetComponent<Text>().text = "Task Version: " + EventSystem.current.currentSelectedGameObject.GetComponent<GameInfoMono>().Version;

        if (gameparameters[0] == "Smooth Pursuit")
        {
            ExtraInfo1Text.GetComponent<Text>().text = "Direction: " + gameparameters[1];
            ExtraInfo2Text.GetComponent<Text>().text = "Start and Endpoint: " + gameparameters[2];
            ExtraInfo3Text.GetComponent<Text>().text = "Duration per smooth pursuit: " + gameparameters[4];
            ExtraInfo4Text.GetComponent<Text>().text = "Repetitions: " + gameparameters[5];
            ToReplayButton.GetComponent<Button>().onClick.AddListener(GoToReplay);
        }
        else if (gameparameters[0] == "Reading Task")
        {
            ExtraInfo1Text.GetComponent<Text>().text = "Language: " + gameparameters[2];
            ExtraInfo2Text.GetComponent<Text>().text = "Length: " + gameparameters[1];
            ToReplayButton.GetComponent<Button>().onClick.AddListener(GoToReplay);

        }
        else if (gameparameters[0] == "Fixation")
        {
            ExtraInfo1Text.GetComponent<Text>().text = "Direction: " + gameparameters[1] + " " + gameparameters[2];
            ExtraInfo2Text.GetComponent<Text>().text = gameparameters[4] + ": " + gameparameters[3];
            ExtraInfo3Text.GetComponent<Text>().text = gameparameters[6] + ": " + gameparameters[5];
            if (gameparameters.Length > 7)
            {
                ExtraInfo4Text.GetComponent<Text>().text = gameparameters[8] + ": " + gameparameters[7];
            }
            ToReplayButton.GetComponent<Button>().onClick.AddListener(GoToReplay);
        }
        if(gameparameters[0] == "Depth")
        {
            TaskNameText.SetActive(true);
            RecordingTimeText.SetActive(true);
            Versiontext.SetActive(true);
            ExtraInfo1Text.SetActive(false);
            ExtraInfo2Text.SetActive(false);
            ExtraInfo3Text.SetActive(false);
            ExtraInfo4Text.SetActive(false);
            ExtraInfo5Text.SetActive(false);
            ExtraInfo6Text.SetActive(false);
            ExtraInfo7Text.SetActive(false);
            ExtraInfo8Text.SetActive(false);
            ExtraInfo9Text.SetActive(false);
            ExtraInfo10Text.SetActive(false);
            ToReplayButton.GetComponent<Button>().onClick.AddListener(GoToReplay);
        }
        else
        {
            TaskNameText.SetActive(true);
            RecordingTimeText.SetActive(true);
            Versiontext.SetActive(true);
            ExtraInfo1Text.SetActive(true);
            ExtraInfo2Text.SetActive(true);
            ExtraInfo3Text.SetActive(true);
            ExtraInfo4Text.SetActive(true);
            ExtraInfo5Text.SetActive(false);
            ExtraInfo6Text.SetActive(false);
            ExtraInfo7Text.SetActive(false);
            ExtraInfo8Text.SetActive(false);
            ExtraInfo9Text.SetActive(false);
            ExtraInfo10Text.SetActive(false);
        }
        ToReplayButton.SetActive(true);
    }

    void GoToReplay()
    {
        GameInfoMono g = new GameInfoMono(previouslySelectedGameObject.GetComponent<GameInfoMono>().GameName, previouslySelectedGameObject.GetComponent<GameInfoMono>().Version, previouslySelectedGameObject.GetComponent<GameInfoMono>().timestamp);
        gameparameters = g.GameName.Split('_');
        DetailsForReplay.GetComponent<GameInfoMono>().GameName = g.GameName;
        DetailsForReplay.GetComponent<GameInfoMono>().Version = g.Version;
        DetailsForReplay.GetComponent<GameInfoMono>().timestamp = g.timestamp;
        if (gameparameters[0] == "Smooth Pursuit")
        {
            DontDestroyOnLoad(DetailsForReplay);
            SceneManager.LoadScene("SmoothPursuitScene");
        }
        else if (gameparameters[0] == "Reading Task")
        {
            DontDestroyOnLoad(DetailsForReplay);
            SceneManager.LoadScene("ReadingScene");
        }
        else if (gameparameters[0] == "Fixation")
        {
            DontDestroyOnLoad(DetailsForReplay);
            SceneManager.LoadScene("FixationScene");
        }
        if (gameparameters[0] == "Depth")
        {
            DontDestroyOnLoad(DetailsForReplay);
            //SceneManager.LoadScene("DepthScene");
            Debug.Log("No Scene Manager For Depth Scene Yet");
        }
    }

    // Update is called once per frame
    void Update()
    {
        controllers.transform.position = xrrig.transform.position;
    }

    public List<GameInfo> GameInfoFromJson(string json)
    {
        Debug.Log(json);
        List<GameInfo> recordinglist = JsonConvert.DeserializeObject<List<GameInfo>>(json);
        return recordinglist;
    }

    public List<ResultEntity> D2ResultFromJson(string json)
    {
        Debug.Log(json);
        List<ResultEntity> recordinglist = JsonConvert.DeserializeObject<List<ResultEntity>>(json);
        return recordinglist;
    }

    public void CreateButton(Transform panel, Vector3 position, Vector2 size, UnityEngine.Events.UnityAction method)
    {
        GameObject button = new GameObject();
        button.transform.parent = panel;
        button.AddComponent<RectTransform>();
        button.AddComponent<Button>();
        button.transform.position = position;
        button.GetComponent<RectTransform>().sizeDelta.Set(size.x, size.y);
        button.GetComponent<Button>().onClick.AddListener(method);
    }

}
