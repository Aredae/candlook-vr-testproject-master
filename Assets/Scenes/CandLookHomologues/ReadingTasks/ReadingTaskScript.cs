using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private int length;

    private string language;

    private bool started;
    private float timer;

    private string init_text;
    private bool waitrunning;

    private GameRecorder recorder;
    private VarjoEventManager em;
    private EyeTracker et;
    // Start is called before the first frame update
    void Start()
    {
        countdowntimer.SetActive(false);
        waitrunning = false;
        init_text = text.GetComponent<Text>().text;
        finishedButton.SetActive(false);
        language = "English";
        length = 0;
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
            
            //record stuff
        }
    }

    public void startGame()
    {
        StartCoroutine(waitAndGenerateText());
        settingscanvas.SetActive(false);
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
        started = false;
        settingscanvas.SetActive(true);
        finishedButton.SetActive(false);
        text.GetComponent<Text>().text = init_text;
        //stoprecording
    }

    IEnumerator waitAndGenerateText()
    {
        waitrunning = true;
        countdowntimer.SetActive(true);
        yield return new WaitForSeconds(3);
        waitrunning = false;
        countdowntimer.SetActive(false);
        //waitrunning = true;
        if (dropdownlanguage.GetComponent<Dropdown>().value == 0)
        {
            language = "English";
        }
        else
        {
            language = "Norwegian";
        }

        if (language == "English" && length == 0)
        {
            text.GetComponent<Text>().text = "English Short";

            if (UnityEngine.XR.XRSettings.isDeviceActive)
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

            if (UnityEngine.XR.XRSettings.isDeviceActive)
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
            if (UnityEngine.XR.XRSettings.isDeviceActive)
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
            /*
            if (UnityEngine.XR.XRSettings.isDeviceActive)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "ReadingTask_Short_Norwegian",
                    Version = 1,
                }, et);
            }
            */
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
            /*
            if (UnityEngine.XR.XRSettings.isDeviceActive)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "ReadingTask_Medium_Norwegian",
                    Version = 1,
                }, et);
            }
            */
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
            /*
            if (UnityEngine.XR.XRSettings.isDeviceActive)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "ReadingTask_Long_Norwegian",
                    Version = 1,
                }, et);
            }
            */
        }
        finishedButton.SetActive(true);
        started = true;
    }

    void OnDestroy()
    {
        recorder.Commit();
    }
}
