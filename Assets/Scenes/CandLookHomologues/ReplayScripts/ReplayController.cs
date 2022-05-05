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
    WebRequest webrequest = new WebRequest();
    public GameObject usertext;

    public GameObject Canvas;
    private System.Action<string> _createGetTaskNamesCallback;
    public GameObject initbutton;
    private string[] gameparameters;
    public GameObject TaskNameText;
    public GameObject RecordingTimeText;
    public GameObject ExtraInfo1Text;
    public GameObject ExtraInfo2Text;
    public GameObject ExtraInfo3Text;
    public GameObject ExtraInfo4Text;
    public GameObject ToReplayButton;
    public GameObject Versiontext;
    private GameObject previouslySelectedGameObject;
    public GameObject DetailsForReplay;
    // Start is called before the first frame update
    void Start()
    {
        usertext.transform.GetComponent<Text>().text += " " + Subjectinfo.instance.GetName();

        TaskNameText.SetActive(false);
        RecordingTimeText.SetActive(false);
        ExtraInfo1Text.SetActive(false);
        ExtraInfo2Text.SetActive(false);
        ExtraInfo3Text.SetActive(false);
        ExtraInfo4Text.SetActive(false);
        ToReplayButton.SetActive(false);
        Versiontext.SetActive(false);

        _createGetTaskNamesCallback = (jsonArray) => {
            //Separate entries and store in array
            List<GameInfo> recordingslist = GameInfoFromJson(jsonArray);
            int i = 0;
            foreach(GameInfo g in recordingslist)
            {
                //Generate Button for each game
                GameObject newButton = DefaultControls.CreateButton(
                new DefaultControls.Resources()
                );
                newButton.transform.SetParent(Canvas.transform, false);
                newButton.transform.position = initbutton.transform.position;
                newButton.transform.position = new Vector3(newButton.transform.position.x, newButton.transform.position.y - newButton.GetComponent<RectTransform>().rect.height*i, newButton.transform.position.z);
                newButton.GetComponent<Text>().text = g.GameName;
                //Add differnt OnClick Events for eacn button
                newButton.GetComponent<Button>().onClick.AddListener(ButtonClickEvent);
                newButton.AddComponent<GameInfoMono>();
                newButton.GetComponent<GameInfoMono>().GameName = g.GameName;
                newButton.GetComponent<GameInfoMono>().Version = g.Version;
                newButton.GetComponent<GameInfoMono>().timestamp = g.timestamp;
                i++;
            }

            //Generate button for each entry
            //if(gamename == something, create button for that game and send data with that button)
        };

    }


    void ButtonClickEvent()
    {
        //if()
        previouslySelectedGameObject = EventSystem.current.currentSelectedGameObject;
        ToReplayButton.GetComponent<Button>().onClick.RemoveAllListeners();
        gameparameters = EventSystem.current.currentSelectedGameObject.GetComponent<GameInfoMono>().GameName.Split('_');
        TaskNameText.GetComponent<Text>().text = "Task: " + gameparameters[0];
        RecordingTimeText.GetComponent<Text>().text = "Timestamp: " + EventSystem.current.currentSelectedGameObject.GetComponent<GameInfoMono>().timestamp.ToString();
        Versiontext.GetComponent<Text>().text = "Task Version: " + EventSystem.current.currentSelectedGameObject.GetComponent<GameInfoMono>().Version;

        if (gameparameters[0] == "SmoothPursuit")
        {
            ExtraInfo1Text.GetComponent<Text>().text = "Direction: " + gameparameters[1];
            ExtraInfo2Text.GetComponent<Text>().text = "Start and Endpoint: " + gameparameters[2];
            ExtraInfo3Text.GetComponent<Text>().text = "Duration per smooth pursuit: " + gameparameters[4];
            ExtraInfo4Text.GetComponent<Text>().text = "Repetitions: " + gameparameters[5];
            ToReplayButton.GetComponent<Button>().onClick.AddListener(GoToReplay);
        }
        else if (gameparameters[0] == "ReadingTask")
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
        }
        ToReplayButton.SetActive(true);
    }
    void ReadingButtonClickEvent()
    {
        //if()
        TaskNameText.SetActive(true);
        RecordingTimeText.SetActive(true);
        ExtraInfo1Text.SetActive(true);
        ExtraInfo2Text.SetActive(true);
        ExtraInfo3Text.SetActive(true);
        ExtraInfo4Text.SetActive(true);
    }
    void FixationButtonClickEvent()
    {
        //if()
        TaskNameText.SetActive(true);
        RecordingTimeText.SetActive(true);
        ExtraInfo1Text.SetActive(true);
        ExtraInfo2Text.SetActive(true);
        ExtraInfo3Text.SetActive(true);
        ExtraInfo4Text.SetActive(true);
    }
    void DepthButtonClickEvent()
    {
        //if()
        TaskNameText.SetActive(true);
        RecordingTimeText.SetActive(true);
        ExtraInfo1Text.SetActive(true);
        ExtraInfo2Text.SetActive(true);
        ExtraInfo3Text.SetActive(true);
        ExtraInfo4Text.SetActive(true);
    }

    void GoToReplay()
    {
        GameInfoMono g = new GameInfoMono(previouslySelectedGameObject.GetComponent<GameInfoMono>().GameName, previouslySelectedGameObject.GetComponent<GameInfoMono>().Version, previouslySelectedGameObject.GetComponent<GameInfoMono>().timestamp);
        gameparameters = g.GameName.Split('_');
        DetailsForReplay.GetComponent<GameInfoMono>().GameName = g.GameName;
        DetailsForReplay.GetComponent<GameInfoMono>().Version = g.Version;
        DetailsForReplay.GetComponent<GameInfoMono>().timestamp = g.timestamp;
        if (gameparameters[0] == "SmoothPursuit")
        {
            DontDestroyOnLoad(DetailsForReplay);
            SceneManager.LoadScene("SmoothPursuitScene");
        }
        else if (gameparameters[0] == "ReadingTask")
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
        
    }

    public List<GameInfo> GameInfoFromJson(string json)
    {
        Debug.Log(json);
        List<GameInfo> recordinglist = JsonConvert.DeserializeObject<List<GameInfo>>(json);
        return recordinglist;
    }

    public void GetTaskNamesFromRecordingsOfUser(int userid, System.DateTime starttime)
    {
        //StartCoroutine(webrequest.GetResultsFromSubject("http://158.37.193.176/SaveNewD2Score.php", resultsobject, _createSaveCallback));
    }
    public void GetETResults(int userid, string gamename, int version, System.DateTime starttime)
    {
        //StartCoroutine(webrequest.GetResultsFromSubject("http://158.37.193.176/SaveNewD2Score.php", resultsobject, _createSaveCallback));
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
