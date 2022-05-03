using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private Vector3 tempposvector;


    //private Varjo.XR.VarjoEventManager em;

    // Start is called before the first frame update
    void Start()
    {
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
        if (Input.GetKeyDown(KeyCode.Space))
            et.calibrate();

        if (Input.GetKeyUp(KeyCode.Escape) && (xrrig.GetComponent<SimpleSmoothMouseLook>() != null))
        {
            Destroy(xrrig.GetComponent<SimpleSmoothMouseLook>());
        }

        if (started)
        {
            if (UnityEngine.XR.XRSettings.isDeviceActive)
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
                    if (UnityEngine.XR.XRSettings.isDeviceActive)
                    {
                        OnDestroy();
                    }
                    started = false;
                    repetitions = (int)repetitionSlider.GetComponent<Slider>().value;
                    SettingsCanvas.SetActive(true);
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

                if (UnityEngine.XR.XRSettings.isDeviceActive)
                {
                    recorder = new GameRecorder(new Util.Model.Game
                    {
                        Name = "SmoothPursuit_Horizontal_LeftRight_" + speed + "_Seconds_" + repetitions + "_Repetitions",
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

                if (UnityEngine.XR.XRSettings.isDeviceActive)
                {
                    recorder = new GameRecorder(new Util.Model.Game
                    {
                        Name = "SmoothPursuit_Diagonal_LeftRight_" + speed + "_Seconds_" + repetitions + "_Repetitions",
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
                if (UnityEngine.XR.XRSettings.isDeviceActive)
                {
                    recorder = new GameRecorder(new Util.Model.Game
                    {
                        Name = "SmoothPursuit_Horizontal_RightLeft_" + speed + "_Seconds_" + repetitions + "_Repetitions",
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
                if (UnityEngine.XR.XRSettings.isDeviceActive)
                {
                    recorder = new GameRecorder(new Util.Model.Game
                    {
                        Name = "SmoothPursuit_Diagonal_RightLeft_" + speed + "_Seconds_" + repetitions + "_Repetitions",
                        Version = 1,
                    }, et);
                }
            }
        }

        t = 0;
        speed = (int)speedslider.GetComponent<Slider>().value;
        repetitions = (int)repetitionSlider.GetComponent<Slider>().value;
        SettingsCanvas.SetActive(false);
        started = true;
    }

    void OnDestroy()
    {
        recorder.Commit();
    }
}
