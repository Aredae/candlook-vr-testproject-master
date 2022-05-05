using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;
using Varjo.XR;

public class ReadingTaskScript : MonoBehaviour
{

    public GameObject xrrig;

    public GameObject dropdownlanguage;

    public GameObject text;

    public GameObject finishedButton;

    public GameObject settingscanvas;

    public GameObject countdowntimer;

    public GameObject shortbutton;
    public GameObject mediumbutton;
    public GameObject longbutton;

    public GameObject startReplayButton;

    private int length;

    private string language;

    private bool started;
    private float timer;

    private string init_text;
    private bool waitrunning;

    private GameRecorder recorder;
    private VarjoEventManager em;
    private EyeTracker et;
    private GameObject replayobject;
    private bool replay;
    private string[] gameprams;
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("DetailForReplay") != null)
        {
            replay = true;
            replayobject = GameObject.Find("DetailForReplay");
        }
        else { replay = false; }

        countdowntimer.SetActive(false);
        waitrunning = false;
        init_text = text.GetComponent<Text>().text;
        finishedButton.SetActive(false);
        if (replay)
        {
            settingscanvas.SetActive(false);

            //Set all settings to that of replay
            gameprams = replayobject.GetComponent<GameInfo>().GameName.Split('_');
            //Set replay language
            if(gameprams[2] == "English")
            {
                language = "English";
            }else if(gameprams[2] == "Norwegian")
            {
                language = "Norwegian";
            }
            else
            {
                Debug.Log("Could not find language");
            }
            //Set replay length
            if (gameprams[1] == "Short")
            {
                length = 0;
            }
            else if (gameprams[1] == "Medium")
            {
                length = 1;
            }
            else if (gameprams[1] == "Long")
            {
                length = 2;
            }
            else
            {
                Debug.Log("Could not find length");
            }

            startReplayButton.SetActive(true);
        }
        else
        {
            startReplayButton.SetActive(false);
            language = "English";
            length = 0;
            
        }

        if (!UnityEngine.XR.XRSettings.isDeviceActive)
        {
            SimpleSmoothMouseLook mouseController = xrrig.AddComponent<SimpleSmoothMouseLook>();
        }
        else
        {
            em = VarjoEventManager.Instance;
            et = new VarjoET(Camera.main);
            //GazeVisualizer.spawn(et);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (waitrunning)
        {
            timer += Time.deltaTime;
            int time = (int)System.Math.Floor(timer);
            countdowntimer.GetComponent<Text>().text = "Starting in: " + time;
        }
        if (started)
        {
            if (replay)
            {
                //TODO make gaze visualizers move each frame equal to et data from db
                //
                //
                //
            }


            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
            {
                recorder.Update();
            }
            //record stuff
        }
    }

    public void startGame()
    {
        StartCoroutine(waitAndGenerateText());
        if (replay)
        {
            startReplayButton.SetActive(false);
        }
        else
        {
            settingscanvas.SetActive(false);
        }
    }

    public void shortpressed()
    {
        length = 0;
        shortbutton.GetComponent<Image>().color = shortbutton.GetComponent<Button>().colors.selectedColor;
        mediumbutton.GetComponent<Image>().color = shortbutton.GetComponent<Button>().colors.normalColor;
        longbutton.GetComponent<Image>().color = shortbutton.GetComponent<Button>().colors.normalColor;
    }
    public void mediumpressed()
    {
        length = 1;
        mediumbutton.GetComponent<Image>().color = shortbutton.GetComponent<Button>().colors.selectedColor;
        shortbutton.GetComponent<Image>().color = shortbutton.GetComponent<Button>().colors.normalColor;
        longbutton.GetComponent<Image>().color = shortbutton.GetComponent<Button>().colors.normalColor;
    }
    public void longpressed()
    {
        length = 2;
        mediumbutton.GetComponent<Image>().color = shortbutton.GetComponent<Button>().colors.normalColor;
        shortbutton.GetComponent<Image>().color = shortbutton.GetComponent<Button>().colors.normalColor;
        longbutton.GetComponent<Image>().color = shortbutton.GetComponent<Button>().colors.selectedColor;
    }

    public void gameFinished()
    {
        if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
        {
            OnDestroy();
        }
        started = false;
        if (replay)
        {
            startReplayButton.SetActive(true);
        }
        else
        {
            settingscanvas.SetActive(true);
        }
        finishedButton.SetActive(false);
        text.GetComponent<Text>().text = init_text;
    }

    IEnumerator waitAndGenerateText()
    {
        waitrunning = true;
        countdowntimer.SetActive(true);
        yield return new WaitForSeconds(3);
        waitrunning = false;
        countdowntimer.SetActive(false);
        //waitrunning = true;
        if (!replay)
        {
            if (dropdownlanguage.GetComponent<Dropdown>().value == 0)
            {
                language = "English";
            }
            else
            {
                language = "Norwegian";
            }
        }
        if (language == "English" && length == 0)
        {
            text.GetComponent<Text>().text = "English Short";

            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "ReadingTask_Short_English",
                    Version = 1,
                }, et);
            }
        }
        else if (language == "English" && length == 1)
        {
            text.GetComponent<Text>().text = "English Medium";

            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "ReadingTask_Medium_English",
                    Version = 1,
                }, et);
            }
        }
        else if (language == "English" && length == 2)
        {
            text.GetComponent<Text>().text = "English Long";
            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "ReadingTask_Long_English",
                    Version = 1,
                }, et);
            }
        }
        else if (language == "Norwegian" && length == 0)
        {
            text.GetComponent<Text>().text = "Deilfinen mangler ytter�re. men dens h�rselsorgan er blant de best utviklede i dyreriket. Delfinen kan v�re under "
                + "vann i 15 minutter uten � m�tte opp for � puste. Den er i bevegelse under mesteparten av sitt tretti�rige liv og "
                + "sover litt bare n� og da. �ynene er vanligvis igjen i 30 sekunder, men noen ganger lenger - opp til fem minutter.";
            
            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "ReadingTask_Short_Norwegian",
                    Version = 1,
                }, et);
            }
        }
        else if (language == "Norwegian" && length == 1)
        {
            text.GetComponent<Text>().text = "Deilfinen mangler ytter�re. men dens h�rselsorgan er blant de best utviklede i dyreriket. Delfinen kan v�re under "
                + "vann i 15 minutter uten � m�tte opp for � puste. Den er i bevegelse under mesteparten av sitt tretti�rige liv og "
                + "sover litt bare n� og da. �ynene er vanligvis igjen i 30 sekunder, men noen ganger lenger - opp til fem minutter."
                + "Jeg har en gang v�rt med p� � se at en delfin blir f�dt, det er noe jeg aldri skal glemme. En god venn, som er "
                + "biolog, hadde invitert meg til den lykkelige begivenheten som skjedde i et kjempeakvarium i California. Den "
                + "bl�gr�, over to meter lange hunndelfinen stod n�r bunne p� akvariet da hun f�dde ungen. Nedkomsten tok litt "
                + "over en halvtime. S� reiv hunnen av navlestrengen med et kraftig rykk, og den nesten meterlange ungen var fri.";
            
            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "ReadingTask_Medium_Norwegian",
                    Version = 1,
                }, et);
            }
            
        }
        else
        {
            text.GetComponent<Text>().text = "Deilfinen mangler ytter�re. men dens h�rselsorgan er blant de best utviklede i dyreriket. Delfinen kan v�re under " //+System.Environment.NewLine
                + "vann i 15 minutter uten � m�tte opp for � puste. Den er i bevegelse under mesteparten av sitt tretti�rige liv og " //+ System.Environment.NewLine
                + "sover litt bare n� og da. �ynene er vanligvis igjen i 30 sekunder, men noen ganger lenger - opp til fem minutter." //+ System.Environment.NewLine
                + "Jeg har en gang v�rt med p� � se at en delfin blir f�dt, det er noe jeg aldri skal glemme. En god venn, som er " //+ System.Environment.NewLine
                + "biolog, hadde invitert meg til den lykkelige begivenheten som skjedde i et kjempeakvarium i California. Den " //+ System.Environment.NewLine
                + "bl�gr�, over to meter lange hunndelfinen stod n�r bunne p� akvariet da hun f�dde ungen. Nedkomsten tok litt " //+ System.Environment.NewLine
                + "over en halvtime. S� reiv hunnen av navlestrengen med et kraftig rykk, og den nesten meterlange ungen var fri." //+ System.Environment.NewLine
                + "Uten ett �yeblikks n�lig sv�mte den opp til overflaten, stakk hodet over vannet, pustet og vendte tilbake til " //+ System.Environment.NewLine
                + "moren. Min venn fortalte; �Ungen kan h�re umiddelbart etter f�dsel. Den oppfatter mors �stemme� og" //+ System.Environment.NewLine
                + "�snakker� selv med plystring og grynt�. Vi s� hvordan ungen drakk av to brystvorter som var plassert n�r" //+ System.Environment.NewLine
                + "hunnens hale. N�r hun dro sammen bukmusklene, sprutet hun melk inn i ungens munn.";
            
            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "ReadingTask_Long_Norwegian",
                    Version = 1,
                }, et);
            }
            
        }
        if (!replay)
        {
            finishedButton.SetActive(true);
        }
        started = true;
    }

    public void ExitReplay()
    {
        Destroy(replayobject);
        SceneManager.LoadScene("Replays");
    }

    void OnDestroy()
    {
        recorder.Commit();
    }
}
