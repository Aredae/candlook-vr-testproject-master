using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;
using Varjo.XR;
using something;

public class ReadingTaskScript : MonoBehaviour
{
    WebRequest webrequest = new WebRequest();
    public GameObject xrrig;

    public GameObject leftgazepoint;
    public GameObject rightgazepoint;

    public GameObject dropdownlanguage;

    public GameObject text;

    public GameObject finishedButton;

    public GameObject settingscanvas;

    public GameObject countdowntimer;

    public GameObject shortbutton;
    public GameObject mediumbutton;
    public GameObject longbutton;

    public GameObject startReplayButton;
    public GameObject ExitReplayButton;
    public GameObject PauseButton;
    private int currentframefordata;
    private DB db;

    private int length;

    private string language;

    private bool started;
    private bool pause;
    private float timer;

    private string init_text;
    private bool waitrunning;

    private GameRecorder recorder;
    private VarjoEventManager em;
    private EyeTracker et;
    private GameObject replayobject;
    private bool replay;
    private string[] gameprams;
    public int subject_id;
    private bool usersignedinn;
    private System.Action<string> _createGetTaskGazeDataCallback;
    private Util.Model.Recording currentrecdata;
    private float nanosecondssincelastupdate;
    public GameObject controllers;


    // Start is called before the first frame update
    void Start()
    {
        currentframefordata = 0;
        if (GameObject.Find("DetailsForReplay") != null)
        {
            db = new DB();
            replay = true;
            replayobject = GameObject.Find("DetailsForReplay").gameObject;
        }
        else { replay = false; }

        if (GameObject.Find("SubjectInfo") != null)
        {
            subject_id = GameObject.Find("SubjectInfo").GetComponent<Subjectinfo>().GetId();
            usersignedinn = true;
        }
        else
        {
            usersignedinn = false;
            //Noone is logged in and info should not be saved
        }

        countdowntimer.SetActive(false);
        pause = false;
        waitrunning = false;
        init_text = text.GetComponent<Text>().text;
        finishedButton.SetActive(false);
        if (replay)
        {
            currentframefordata = 0;
            settingscanvas.SetActive(false);

            //Set all settings to that of replay
            gameprams = replayobject.GetComponent<GameInfoMono>().GameName.Split('_');
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

            _createGetTaskGazeDataCallback = (jsonArray) =>
            {
                db.Database.BeginTransaction();
                something.ThrowawayIntTye s = JsonConvert.DeserializeObject<something.ThrowawayIntTye>(jsonArray);
                
                currentrecdata = db.Recordings.Find(Int32.Parse(s.recording_id));
                db.Dispose();
            };

            GetGazeData(GameObject.Find("SubjectInfo").GetComponent<Subjectinfo>().GetId(), GameObject.Find("DetailsForReplay").GetComponent<GameInfoMono>().timestamp);

            
               
            
            startReplayButton.SetActive(true);
            ExitReplayButton.SetActive(true);
            PauseButton.SetActive(false);
        }
        else
        {
            startReplayButton.SetActive(false);
            ExitReplayButton.SetActive(false);
            PauseButton.SetActive(false);
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
        if (!replay)
        {
            leftgazepoint.SetActive(false);
            rightgazepoint.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        controllers.transform.position = xrrig.transform.position;
        if (waitrunning)
        {
            timer += Time.deltaTime;
            int time = 3-(int)System.Math.Round(timer);
            countdowntimer.GetComponent<Text>().text = "Starting in: " + time;
        }
        if (started)
        {
            if (!pause) {
                if (replay)
                {
                   
                    if (currentframefordata+1 == currentrecdata.TimestampNS.Count)
                    {
                        gameFinished();
                    }
                    else
                    {
                        nanosecondssincelastupdate += Time.deltaTime*1000000000;
                        if (currentrecdata.TimestampNS[currentframefordata + 1] - currentrecdata.TimestampNS[currentframefordata] <= nanosecondssincelastupdate)
                        {
                            
                            try
                            {
                                Vector3 leftpositiontest = new Vector3(currentrecdata.LeftEyePosX[currentframefordata], currentrecdata.LeftEyePosY[currentframefordata], currentrecdata.LeftEyePosZ[currentframefordata]);
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.Log(e);
                                gameFinished();

                            }
                            Vector3 leftposition = new Vector3(currentrecdata.LeftEyePosX[currentframefordata], currentrecdata.LeftEyePosY[currentframefordata], currentrecdata.LeftEyePosZ[currentframefordata]);
                            Vector3 rightposition = new Vector3(currentrecdata.RightEyePosX[currentframefordata], currentrecdata.RightEyePosY[currentframefordata], currentrecdata.RightEyePosZ[currentframefordata]);
                            Vector3 leftgazedir = new Vector3(currentrecdata.LeftGazeDirX[currentframefordata], currentrecdata.LeftGazeDirY[currentframefordata], currentrecdata.LeftGazeDirZ[currentframefordata]);
                            Vector3 rightgazedir = new Vector3(currentrecdata.RightGazeDirX[currentframefordata], currentrecdata.RightGazeDirY[currentframefordata], currentrecdata.RightGazeDirZ[currentframefordata]);
                            Vector3 leftgazedirrel = new Vector3(currentrecdata.LeftGazeDirRelX[currentframefordata], currentrecdata.LeftGazeDirRelY[currentframefordata], currentrecdata.LeftGazeDirRelZ[currentframefordata]);
                            Vector3 rightgazedirrel = new Vector3(currentrecdata.RightGazeDirRelX[currentframefordata], currentrecdata.RightGazeDirRelY[currentframefordata], currentrecdata.RightGazeDirRelZ[currentframefordata]);

                            Eye left = new Eye();
                            left.position = leftposition;
                            left.gazeDirection = leftgazedir;
                            left.gazeDirectionRel = leftgazedirrel;
                            Eye right = new Eye();
                            right.position = rightposition;
                            right.gazeDirection = rightgazedir;
                            right.gazeDirectionRel = rightgazedirrel;
                            Eye average = new Eye();
                            average.position = (rightposition + leftposition) / 2;
                            average.gazeDirection = (rightgazedir + leftgazedir) / 2;
                            average.gazeDirectionRel = (rightgazedirrel + leftgazedirrel) / 2;
                            EyeData eyeData = new EyeData();
                            eyeData.left = left;
                            eyeData.right = right;
                            eyeData.average = average;

                            leftgazepoint.transform.position = eyeData.left.position + (currentrecdata.approxFocusDist[currentframefordata] + 2f) * eyeData.left.gazeDirection;
                            rightgazepoint.transform.position = eyeData.right.position + (currentrecdata.approxFocusDist[currentframefordata] + 2f) * eyeData.right.gazeDirection;
                            //GazeVisualizer.spawn
                            currentframefordata++;
                            nanosecondssincelastupdate = 0;

                        }
                    }

                        
                }
                    
            }


                if (UnityEngine.XR.XRSettings.isDeviceActive && !replay && usersignedinn)
                {
                    recorder.Update();
                }

            }
            //record stuff
        
    }
    public void GetGazeData(int userid, string timestamp)
    {
        StartCoroutine(webrequest.getGazeDataForRecording("http://localhost/getGazeDataForTask.php", userid, timestamp, _createGetTaskGazeDataCallback));
    }

    public void startGame()
    {
        StartCoroutine(waitAndGenerateText());
        if (replay)
        {
            startReplayButton.SetActive(false);
            ExitReplayButton.SetActive(false);
            PauseButton.SetActive(true);
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
        if (UnityEngine.XR.XRSettings.isDeviceActive && !replay && usersignedinn)
        {
            OnDestroy();
        }
        started = false;
        if (replay)
        {
            startReplayButton.SetActive(true);
            ExitReplayButton.SetActive(true);
            PauseButton.SetActive(false);
            currentframefordata = 0;
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
            text.GetComponent<Text>().text = "Wearable technology, or “wearables”, is the name for the type of electronic devices we can wear as accessories, implanted in our clothing or even in our body. Wearables are hands-free gadgets with microprocessors and a connection to the internet.";

            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay && usersignedinn)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "Reading Task_Short_English",
                    Version = 1,
                }, et, subject_id, DateTime.Now.ToString());
            }
        }
        else if (language == "English" && length == 1)
        {
            text.GetComponent<Text>().text = "Wearable technology, or “wearables”, is the name for the type of electronic devices we can wear as accessories, implanted in our clothing or even in our body.Wearables are hands - free gadgets with microprocessors and a connection to the internet." +
                "The first popular electronic wearable technology was Fitness trackers, like ‘Fitbits’, which became popular in the 2010s. They monitor your heart and movement and help you keep fit. Now, wearable technology helps people stay healthy in new ways. For example, the ‘iTBra’ is a patch. Women wear it inside their bras, and it checks for breast cancer.";

            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay && usersignedinn)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "Reading Task_Medium_English",
                    Version = 1,
                }, et, subject_id, DateTime.Now.ToString());
            }
        }
        else if (language == "English" && length == 2)
        {
            text.GetComponent<Text>().text = "Wearable technology, or “wearables”, is the name for the type of electronic devices we can wear as accessories, implanted in our clothing or even in our body. Wearables are hands-free gadgets with microprocessors and a connection to the internet." +
                "The first popular electronic wearable technology was Fitness trackers, like ‘Fitbits’, which became popular in the 2010s. They monitor your heart and movement and help you keep fit. Now, wearable technology helps people stay healthy in new ways. For example, the ‘iTBra’ is a patch. Women wear it inside their bras, and it checks for breast cancer." +
                " ‘Heartguide’ looks like a smartwatch, but it can measure blood pressure. It can also track information about a person’s lifestyle, for example, how much they exercise. Then it shares this information with a doctor so that the doctor can give better advice. ‘SmartSleep’ is a soft headband. It helps people to sleep better. It collects information about people’s sleep patterns, gives advice and makes sounds to help people fall asleep. ";
            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay && usersignedinn)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "Reading Task_Long_English",
                    Version = 1,
                }, et, subject_id, DateTime.Now.ToString());
            }
        }
        else if (language == "Norwegian" && length == 0)
        {
            text.GetComponent<Text>().text = "Deilfinen mangler ytterøre. men dens hørselsorgan er blant de best utviklede i dyreriket. Delfinen kan være under "
                + "vann i 15 minutter uten å måtte opp for å puste. Den er i bevegelse under mesteparten av sitt trettiårige liv og "
                + "sover litt bare nå og da. Øynene er vanligvis igjen i 30 sekunder, men noen ganger lenger - opp til fem minutter.";
            
            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay && usersignedinn)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "Reading Task_Short_Norwegian",
                    Version = 1,
                }, et, subject_id, DateTime.Now.ToString());
            }
        }
        else if (language == "Norwegian" && length == 1)
        {
            text.GetComponent<Text>().text = "Deilfinen mangler ytterøre. men dens hørselsorgan er blant de best utviklede i dyreriket. Delfinen kan være under "
                + "vann i 15 minutter uten å måtte opp for å puste. Den er i bevegelse under mesteparten av sitt trettiårige liv og "
                + "sover litt bare nå og da. Øynene er vanligvis igjen i 30 sekunder, men noen ganger lenger - opp til fem minutter."
                + "Jeg har en gang vært med på å se at en delfin blir født, det er noe jeg aldri skal glemme. En god venn, som er "
                + "biolog, hadde invitert meg til den lykkelige begivenheten som skjedde i et kjempeakvarium i California. Den "
                + "blågrå, over to meter lange hunndelfinen stod nær bunne på akvariet da hun fødde ungen. Nedkomsten tok litt "
                + "over en halvtime. Så reiv hunnen av navlestrengen med et kraftig rykk, og den nesten meterlange ungen var fri.";
            
            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay && usersignedinn)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "Reading Task_Medium_Norwegian",
                    Version = 1,
                }, et,subject_id, DateTime.Now.ToString());
            }
            
        }
        else
        {
            text.GetComponent<Text>().text = "Deilfinen mangler ytterøre. men dens hørselsorgan er blant de best utviklede i dyreriket. Delfinen kan være under " //+System.Environment.NewLine
                + "vann i 15 minutter uten å måtte opp for å puste. Den er i bevegelse under mesteparten av sitt trettiårige liv og " //+ System.Environment.NewLine
                + "sover litt bare nå og da. Øynene er vanligvis igjen i 30 sekunder, men noen ganger lenger - opp til fem minutter." //+ System.Environment.NewLine
                + "Jeg har en gang vært med på å se at en delfin blir født, det er noe jeg aldri skal glemme. En god venn, som er " //+ System.Environment.NewLine
                + "biolog, hadde invitert meg til den lykkelige begivenheten som skjedde i et kjempeakvarium i California. Den " //+ System.Environment.NewLine
                + "blågrå, over to meter lange hunndelfinen stod nær bunne på akvariet da hun fødde ungen. Nedkomsten tok litt " //+ System.Environment.NewLine
                + "over en halvtime. Så reiv hunnen av navlestrengen med et kraftig rykk, og den nesten meterlange ungen var fri." //+ System.Environment.NewLine
                + "Uten ett øyeblikks nølig svømte den opp til overflaten, stakk hodet over vannet, pustet og vendte tilbake til " //+ System.Environment.NewLine
                + "moren. Min venn fortalte; «Ungen kan høre umiddelbart etter fødsel. Den oppfatter mors «stemme» og" //+ System.Environment.NewLine
                + "«snakker» selv med plystring og grynt». Vi så hvordan ungen drakk av to brystvorter som var plassert nær" //+ System.Environment.NewLine
                + "hunnens hale. Når hun dro sammen bukmusklene, sprutet hun melk inn i ungens munn.";
            
            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay && usersignedinn)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "Reading Task_Long_Norwegian",
                    Version = 1,
                }, et,subject_id, DateTime.Now.ToString());
            }
            
        }
        if (!replay)
        {
            finishedButton.SetActive(true);
        }
        timer = 0;
        started = true;
    }

    public void ExitReplay()
    {
        Destroy(replayobject);
        SceneManager.LoadScene("Replays");
    }

    public void PauseGame()
    {
        
        if (pause)
        {
            PauseButton.GetComponent<Image>().color = shortbutton.GetComponent<Button>().colors.normalColor;
            pause = false;
            PauseButton.transform.GetChild(0).GetComponent<Text>().text = "Pause";
        }
        else
        {
            PauseButton.GetComponent<Image>().color = shortbutton.GetComponent<Button>().colors.selectedColor;
            pause = true;
            PauseButton.transform.GetChild(0).GetComponent<Text>().text = "Resume";
        }
        
    }

    void OnDestroy()
    {
        if (recorder != null)
        {
            recorder.Commit();
        }
    }
}
