using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;
using Varjo.XR;

public class SmoothPursuitScriot : MonoBehaviour
{
    public GameObject ball;
    public GameObject endball;
    public GameObject startballrl;
    public GameObject endballrl;

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


    //private Varjo.XR.VarjoEventManager em;

    // Start is called before the first frame update
    void Start()
    {
        if(GameObject.Find("DetailForReplay")!= null)
        {
            replay = true;
            replayobject = GameObject.Find("DetailForReplay");
        }
        else { replay = false; }

        started = false;
        endball.SetActive(false);
        startballrl.SetActive(false);
        endballrl.SetActive(false);
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
            gameprams= replayobject.GetComponent<GameInfo>().GameName.Split('_');
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

            startReplayButton.SetActive(true);

        }
        else
        {
            startReplayButton.SetActive(false);
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

        if (started)
        {
            //TODO make gaze visualizers move each frame equal to et data from db
            //
            //
            //


            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
            {
                recorder.Update();
            }
            //DiagSmooth
            if(gametype == 0)
            {
                if(repetitions >= 0)
                {
                    if (ball.transform.position == endball.transform.position)
                    {
                        repetitions--;
                        if(repetitions != -1)
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
                    if(direction == 0)
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
                    if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
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
                        startReplayButton.SetActive(true);
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
                if(repetitions >= 0)
                {
                    if(ball.transform.position == endball.transform.position)
                    {
                        repetitions--;
                        if(repetitions != -1)
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

    public void startGame()
    {
        if(direction == 0)
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

                if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
                {
                    recorder = new GameRecorder(new Util.Model.Game
                    {
                        Name = "Smooth Pursuit_Horizontal_Left Right_" + speed + " Seconds_" + repetitions + "_Repetitions",
                        Version = 1,
                    }, et);
                }
            }
            else
            {
                ball.transform.position = lrinit_pos;
                endball.transform.position = lrinit_end_pos;
                currentendpos = lrinit_end_pos;
                currentstartpos = lrinit_pos;

                if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
                {
                    recorder = new GameRecorder(new Util.Model.Game
                    {
                        Name = "Smooth Pursuit_Diagonal_Left Right_" + speed + " Seconds_" + repetitions + "_Repetitions",
                        Version = 1,
                    }, et);
                }
            }
        }
        else
        {
            if(gametype == 1)
            {
                ball.transform.position = rlinit_pos;
                verticalsteps = (int)repetitionSlider.GetComponent<Slider>().value;
                Vector3 linehorizontal = rlinit_end_pos - lrinit_pos;
                distancevertical = linehorizontal / verticalsteps;
                currenthorizontalendpoint = new Vector3(rlinit_end_pos.x, rlinit_pos.y, rlinit_pos.z);
                endball.transform.position = currenthorizontalendpoint;
                currentstartpos = rlinit_pos;
                if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
                {
                    recorder = new GameRecorder(new Util.Model.Game
                    {
                        Name = "Smooth Pursuit_Horizontal_Right Left_" + speed + " Seconds_" + repetitions + "_Repetitions",
                        Version = 1,
                    }, et);
                }
            }
            else
            {
                ball.transform.position = rlinit_pos;
                endball.transform.position = rlinit_end_pos;
                currentendpos = rlinit_end_pos;
                currentstartpos = rlinit_pos;
                if (UnityEngine.XR.XRSettings.isDeviceActive &&!replay)
                {
                    recorder = new GameRecorder(new Util.Model.Game
                    {
                        Name = "Smooth Pursuit_Diagonal_Right Left_" + speed + " Seconds_" + repetitions + "_Repetitions",
                        Version = 1,
                    }, et);
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
