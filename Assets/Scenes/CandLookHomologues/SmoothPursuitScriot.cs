using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;
using Util.Model;
using Varjo.XR;

public class SmoothPursuitScriot : MonoBehaviour
{
    WebRequest webrequest = new WebRequest();
    public GameObject ball;
    public GameObject endball;
    public GameObject startballrl;
    public GameObject endballrl;
    public GameObject leftgazepoint;
    public GameObject rightgazepoint;

    public GameObject xrrig;

    public GameObject DirectionDropdown;

    public GameObject SettingsCanvas;

    public GameObject speedslider;

    public GameObject horizontalButton;

    public GameObject verticalButton;

    public GameObject speedtext;

    public GameObject repetitionSlider;

    public GameObject repetitionText;

    public GameObject startReplayButton;
    public GameObject PauseButton;
    public GameObject ExitReplayButton;

    private Vector3 lrinit_pos;
    private Vector3 rlinit_pos;
    private Vector3 rlinit_end_pos;
    private Vector3 lrinit_end_pos;
    private Vector3 currenthorizontalendpoint;
    private Vector3 currentendpos;
    private Vector3 currentstartpos;
    private EyeTracker et;
    private VarjoEventManager em;
    private GameRecorder recorder;
    private bool started;
    private float elapsed;

    private int direction;
    private int speed;
    private int repetitions;
    private int gametype;
    private int verticalsteps;
    private Vector3 distancevertical;
    private float t;
    private bool replay;
    private GameObject replayobject;
    private string[] gameprams;

    private Vector3 tempposvector;
    private bool pause;
    public int subject_id;
    private bool usersignedinn;
    private bool waitrunning;
    private float timer;
    private DB db;

    public GameObject countdowntimer;
    private Action<string> _createGetTaskGazeDataCallback;
    private Recording currentrecdata;
    private int currentframefordata;
    private float nanosecondssincelastupdate;


    //private Varjo.XR.VarjoEventManager em;

    // Start is called before the first frame update
    void Start()
    {
        currentframefordata = 0;
        nanosecondssincelastupdate = 0;
        pause = false;
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

        started = false;
        endball.SetActive(false);
        startballrl.SetActive(false);
        endballrl.SetActive(false);
        countdowntimer.SetActive(false);
        waitrunning = false;
        timer = 0;
        lrinit_pos = ball.transform.position;
        rlinit_pos = startballrl.transform.position;
        lrinit_end_pos = endball.transform.position;
        rlinit_end_pos = endballrl.transform.position;
        //init_pos = ball.transform.position;
        //endpos = endball.transform.position;
        elapsed = UnityEngine.Time.realtimeSinceStartup;
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
        if (replay)
        {
            SettingsCanvas.SetActive(false);

            //Set all settings to that of replay
            gameprams= replayobject.GetComponent<GameInfoMono>().GameName.Split('_');
            if(gameprams[2] == "Left Right")
            {
                direction = 0;
            }else if(gameprams[2] == "Right Left")
            {
                direction = 1;
            }
            else
            {
                Debug.Log("something went wrong recognizing direction");
            }
            string[] speedparam = gameprams[3].Split(' ');
            try
            {
                speed = Int32.Parse(speedparam[0]);
                Console.WriteLine(speed);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unable to parse '{speedparam[0]}'");
            }
            try
            {
                repetitions = Int32.Parse(gameprams[4]);
                Console.WriteLine(repetitions);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unable to parse '{gameprams[0]}'");
            }

            if (gameprams[1] == "Diagonal")
            {
                gametype = 0;
            }
            else if (gameprams[1] == "Horizontal")
            {
                gametype = 1;
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
            direction = 0;
            speed = 2;
            repetitions = 1;
            gametype = 0;
            repetitionText.GetComponent<Text>().text = "Repetitons: " + repetitionSlider.GetComponent<Slider>().value;
            speedtext.GetComponent<Text>().text = "Speed (Duration of Saccadic Movement):" + System.Environment.NewLine + speedslider.GetComponent<Slider>().value + " Seconds";

            DirectionDropdown.GetComponent<Dropdown>().onValueChanged.AddListener(delegate
            {
                ChangeDirection(DirectionDropdown);
            });

            speedslider.GetComponent<Slider>().onValueChanged.AddListener(delegate
            {
                speedChanged(speedslider);
            });

            repetitionSlider.GetComponent<Slider>().onValueChanged.AddListener(delegate
            {
                repetitionsChanged(repetitionSlider);
            });
        }
        

    }

    public void GetGazeData(int userid, string timestamp)
    {
        StartCoroutine(webrequest.getGazeDataForRecording("http://localhost/getGazeDataForTask.php", userid, timestamp, _createGetTaskGazeDataCallback));
    }
    public void DiagonalPressed()
    {
        gametype = 0;
    }
    public void VerticalPressed()
    {
        gametype = 1;
    }

    void repetitionsChanged(GameObject repetitionSlider)
    {
        repetitions = (int)repetitionSlider.GetComponent<Slider>().value;
        repetitionText.GetComponent<Text>().text = "Repetitons: " + repetitionSlider.GetComponent<Slider>().value;
    }

    void speedChanged(GameObject speedslider)
    {
        speed = (int)speedslider.GetComponent<Slider>().value;
        speedtext.GetComponent<Text>().text = "Speed (Duration of Saccadic Movement):" + System.Environment.NewLine + speedslider.GetComponent<Slider>().value + " Seconds";
    }

    void ChangeDirection(GameObject dropdown)
    {
        if (dropdown.GetComponent<Dropdown>().value == 0)
        {
            ball.transform.position = lrinit_pos;
            direction = 0;
        }
        else
        {
            ball.transform.position = rlinit_pos;
            direction = 1;
        }
    }
    
    void SwitchEndandStart(Vector3 start, Vector3 end)
    {
        Vector3 temp = start;
        currentstartpos = end;
        currentendpos = temp;
    }

    Vector3 MoveVertically(Vector3 currentverticalstartpos)
    {
        currentverticalstartpos += distancevertical;
        return currentverticalstartpos;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && UnityEngine.XR.XRSettings.isDeviceActive)
            et.calibrate();

        if (Input.GetKeyUp(KeyCode.Escape) && (xrrig.GetComponent<SimpleSmoothMouseLook>() != null))
        {
            Destroy(xrrig.GetComponent<SimpleSmoothMouseLook>());
        }

        if (waitrunning)
        {
            timer += Time.deltaTime;
            int time = 3 - (int)System.Math.Round(timer);
            countdowntimer.GetComponent<Text>().text = "Starting in: " + time;
        }

        if (started)
        {

            if (!pause)
            {
                if (replay)
                {
                    if (currentframefordata + 1 == currentrecdata.TimestampNS.Count)
                    {

                    }
                    else
                    {
                        nanosecondssincelastupdate += Time.deltaTime * 1000000000;
                        if (currentrecdata.TimestampNS[currentframefordata + 1] - currentrecdata.TimestampNS[currentframefordata] <= nanosecondssincelastupdate)
                        {

                            try
                            {
                                Vector3 leftpositiontest = new Vector3(currentrecdata.LeftEyePosX[currentframefordata], currentrecdata.LeftEyePosY[currentframefordata], currentrecdata.LeftEyePosZ[currentframefordata]);
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.Log(e);

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

                            leftgazepoint.transform.position = eyeData.left.position + currentrecdata.approxFocusDist[currentframefordata] * eyeData.left.gazeDirection;
                            rightgazepoint.transform.position = eyeData.right.position + currentrecdata.approxFocusDist[currentframefordata] * eyeData.right.gazeDirection;
                            //GazeVisualizer.spawn
                            currentframefordata++;
                            nanosecondssincelastupdate = 0;

                        }
                    }
                }
                if (UnityEngine.XR.XRSettings.isDeviceActive && !replay && usersignedinn)
                {
                    recorder.Update();
                }
                //DiagSmooth
                if (gametype == 0)
                {
                    if (repetitions >= 0)
                    {
                        if (ball.transform.position == endball.transform.position)
                        {
                            repetitions--;
                            if (repetitions != -1)
                            {
                                SwitchEndandStart(currentstartpos, currentendpos);
                                endball.transform.position = currentendpos;
                                ball.transform.position = currentstartpos;
                                t = 0;
                            }
                        }
                        t += Time.deltaTime / speed;
                        ball.transform.position = Vector3.Lerp(currentstartpos, endball.transform.position, t);
                    }
                    else
                    {
                        if (direction == 0)
                        {
                            ball.transform.position = lrinit_pos;
                            endball.transform.position = lrinit_end_pos;
                        }
                        else
                        {
                            ball.transform.position = rlinit_pos;
                            endball.transform.position = rlinit_end_pos;
                        }
                        t = 0;
                        if (UnityEngine.XR.XRSettings.isDeviceActive && !replay && usersignedinn)
                        {
                            OnDestroy();
                        }
                        started = false;
                        if (!replay)
                        {
                            repetitions = (int)repetitionSlider.GetComponent<Slider>().value;
                            SettingsCanvas.SetActive(true);
                        }
                        else
                        {
                            currentframefordata = 0;
                            nanosecondssincelastupdate = 0;
                            startReplayButton.SetActive(true);
                            ExitReplayButton.SetActive(true);
                            PauseButton.SetActive(false);
                            try
                            {
                                repetitions = Int32.Parse(gameprams[4]);
                                Console.WriteLine(repetitions);
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine($"Unable to parse '{gameprams[0]}'");
                            }
                        }

                    }
                }
                //HorizontalSmooth
                else
                {
                    if (repetitions >= 0)
                    {
                        if (ball.transform.position == endball.transform.position)
                        {
                            repetitions--;
                            if (repetitions != -1)
                            {
                                currentstartpos = MoveVertically(currentstartpos);
                                currenthorizontalendpoint = MoveVertically(currenthorizontalendpoint);
                                ball.transform.position = currentstartpos;
                                endball.transform.position = currenthorizontalendpoint;
                                t = 0;
                            }
                        }
                        t += Time.deltaTime / speed;
                        ball.transform.position = Vector3.Lerp(currentstartpos, endball.transform.position, t);
                    }
                    else
                    {
                        if (direction == 0)
                        {
                            ball.transform.position = lrinit_pos;
                            endball.transform.position = lrinit_end_pos;
                        }
                        else
                        {
                            ball.transform.position = rlinit_pos;
                            endball.transform.position = rlinit_end_pos;
                        }
                        t = 0;
                        if (UnityEngine.XR.XRSettings.isDeviceActive)
                        {
                            OnDestroy();
                        }
                        started = false;
                        repetitions = (int)repetitionSlider.GetComponent<Slider>().value;
                        SettingsCanvas.SetActive(true);
                    }
                }
            }
            
        }
    }
    IEnumerator waitAndApplySettings()
    {
        waitrunning = true;
        countdowntimer.SetActive(true);
        yield return new WaitForSeconds(3);
        waitrunning = false;
        countdowntimer.SetActive(false);
        if (direction == 0)
        {
            if (gametype == 1)
            {
                ball.transform.position = lrinit_pos;
                verticalsteps = (int)repetitionSlider.GetComponent<Slider>().value;
                Vector3 linehorizontal = lrinit_end_pos - rlinit_pos;
                distancevertical = linehorizontal / verticalsteps;
                currenthorizontalendpoint = new Vector3(lrinit_end_pos.x, lrinit_pos.y, lrinit_pos.z);
                endball.transform.position = currenthorizontalendpoint;
                currentstartpos = lrinit_pos;

                if (UnityEngine.XR.XRSettings.isDeviceActive && !replay && usersignedinn)
                {
                    recorder = new GameRecorder(new Util.Model.Game
                    {
                        Name = "Smooth Pursuit_Horizontal_Left Right_" + speed + " Seconds_" + repetitions + "_Repetitions",
                        Version = 1,
                    }, et, subject_id, DateTime.Now.ToString());
                }
            }
            else
            {
                ball.transform.position = lrinit_pos;
                endball.transform.position = lrinit_end_pos;
                currentendpos = lrinit_end_pos;
                currentstartpos = lrinit_pos;

                if (UnityEngine.XR.XRSettings.isDeviceActive && !replay && usersignedinn)
                {
                    recorder = new GameRecorder(new Util.Model.Game
                    {
                        Name = "Smooth Pursuit_Diagonal_Left Right_" + speed + " Seconds_" + repetitions + "_Repetitions",
                        Version = 1,
                    }, et, subject_id, DateTime.Now.ToString());
                }
            }
        }
        else
        {
            if (gametype == 1)
            {
                ball.transform.position = rlinit_pos;
                verticalsteps = (int)repetitionSlider.GetComponent<Slider>().value;
                Vector3 linehorizontal = rlinit_end_pos - lrinit_pos;
                distancevertical = linehorizontal / verticalsteps;
                currenthorizontalendpoint = new Vector3(rlinit_end_pos.x, rlinit_pos.y, rlinit_pos.z);
                endball.transform.position = currenthorizontalendpoint;
                currentstartpos = rlinit_pos;
                if (UnityEngine.XR.XRSettings.isDeviceActive && !replay && usersignedinn)
                {
                    recorder = new GameRecorder(new Util.Model.Game
                    {
                        Name = "Smooth Pursuit_Horizontal_Right Left_" + speed + " Seconds_" + repetitions + "_Repetitions",
                        Version = 1,
                    }, et, subject_id, DateTime.Now.ToString());
                }
            }
            else
            {
                ball.transform.position = rlinit_pos;
                endball.transform.position = rlinit_end_pos;
                currentendpos = rlinit_end_pos;
                currentstartpos = rlinit_pos;
                if (UnityEngine.XR.XRSettings.isDeviceActive && !replay && usersignedinn)
                {
                    recorder = new GameRecorder(new Util.Model.Game
                    {
                        Name = "Smooth Pursuit_Diagonal_Right Left_" + speed + " Seconds_" + repetitions + "_Repetitions",
                        Version = 1,
                    }, et, subject_id, DateTime.Now.ToString());
                }
            }
        }

        t = 0;
        if (!replay)
        {
            speed = (int)speedslider.GetComponent<Slider>().value;
            repetitions = (int)repetitionSlider.GetComponent<Slider>().value;
            SettingsCanvas.SetActive(false);
        }
        else
        {
            startReplayButton.SetActive(false);
            ExitReplayButton.SetActive(false);
            PauseButton.SetActive(true);
        }
        timer = 0;
        started = true;
    }
    public void startGame()
    {
        StartCoroutine(waitAndApplySettings());
        if (replay)
        {
            startReplayButton.SetActive(false);
            ExitReplayButton.SetActive(false);
            PauseButton.SetActive(true);
        }
        else
        {
            SettingsCanvas.SetActive(false);
        }
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
            PauseButton.GetComponent<Image>().color = startReplayButton.GetComponent<Button>().colors.normalColor;
            pause = false;
            PauseButton.transform.GetChild(0).GetComponent<Text>().text = "Pause";
        }
        else
        {
            PauseButton.GetComponent<Image>().color = startReplayButton.GetComponent<Button>().colors.selectedColor;
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
