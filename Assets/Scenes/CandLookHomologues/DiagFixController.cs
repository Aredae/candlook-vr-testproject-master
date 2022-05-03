using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Varjo.XR;

public class DiagFixController : MonoBehaviour
{

    public GameObject ball;
    public GameObject endball;
    public GameObject startballrl;
    public GameObject endballrl;

    public GameObject xrrig;

    public GameObject canvas;

    public GameObject typedropdown;

    public GameObject SettingsCanvas;

    public GameObject timerslider;
    private int WaitingTime = 1;
    private static float POS_DURATION = 1.0f;
    private static int N_SMALL_STEPS = 9;
    private static int N_Back_steps = 0;
    private Vector3 lrinit_pos;
    private Vector3 rlinit_pos;
    private Vector3 rlinit_end_pos;
    private Vector3 lrinit_end_pos;
    private Vector3 init_pos;
    private Vector3 endpos;
    private Vector3 step;
    private EyeTracker et;
    private float elapsed;
    private float timer;
    private int N_forward_steps;
    private int N_repetitions = 3;
    private VarjoEventManager em;
    private GameRecorder recorder;
    public GameObject numsteps;
    private Vector3 distance;
    private Vector3 distancehorizontal;
    private bool started;
    private int gametype;
    public GameObject timetext;
    public GameObject steptext;
    private int horizontalsteps;
    private Vector3 lasthorrizontalpos;
    private int numhorizontals;

    public GameObject HorizontalStepSlider;
    public GameObject HorizontalStepText;
    //private Varjo.XR.VarjoEventManager em;

    // Start is called before the first frame update
    void Start()
    {
        started = false;
        endball.SetActive(false);
        startballrl.SetActive(false);
        endballrl.SetActive(false);
        HorizontalStepSlider.SetActive(false);
        HorizontalStepText.SetActive(false);
        lrinit_pos = ball.transform.position;
        rlinit_pos = startballrl.transform.position;
        lrinit_end_pos = endball.transform.position;
        rlinit_end_pos = endballrl.transform.position;
        //init_pos = ball.transform.position;
        //endpos = endball.transform.position;
        elapsed = UnityEngine.Time.realtimeSinceStartup;
        N_forward_steps = 0;
        lasthorrizontalpos = lrinit_pos;
        if (!UnityEngine.XR.XRSettings.isDeviceActive)
        {
            SimpleSmoothMouseLook mouseController = xrrig.AddComponent<SimpleSmoothMouseLook>();
        }
        else
        {
            em = VarjoEventManager.Instance;
            et = new VarjoET(Camera.main);
            //GazeVisualizer.spawn(et);
            recorder = new GameRecorder(new Util.Model.Game
            {
                Name = "BallGame",
                Version = 1,
            }, et);
        }

        numsteps.GetComponent<Slider>().onValueChanged.AddListener(delegate
        {
            stepschanged(numsteps);
        });

        HorizontalStepSlider.GetComponent<Slider>().onValueChanged.AddListener(delegate
        {
            horizontalstepschanged(HorizontalStepSlider);
        });

        timerslider.GetComponent<Slider>().onValueChanged.AddListener(delegate
        {
            timerchanged(timerslider);
        });

        typedropdown.GetComponent<Dropdown>().onValueChanged.AddListener(delegate
        {
           movestartpoint(typedropdown);
        });

        steptext.GetComponent<Text>().text = "Number of Steps: " + numsteps.GetComponent<Slider>().value;

        timetext.GetComponent<Text>().text = "Seconds Per Fixation: " + timerslider.GetComponent<Slider>().value;

        HorizontalStepText.GetComponent<Text>().text = "Number of Horizontal Steps: " + HorizontalStepSlider.GetComponent<Slider>().value;

    }

    void stepschanged(GameObject numsteps)
    {
        if(typedropdown.GetComponent<Dropdown>().value == 2)
        {
            steptext.GetComponent<Text>().text = "Number of Vertical Steps: " + numsteps.GetComponent<Slider>().value;
        }
        else
        {
            steptext.GetComponent<Text>().text = "Number of Steps: " + numsteps.GetComponent<Slider>().value;
        }
    }

    void horizontalstepschanged(GameObject HorizontalStepSlider)
    {
            HorizontalStepText.GetComponent<Text>().text = "Number of Horizontal Steps: " + HorizontalStepSlider.GetComponent<Slider>().value;
    }

    void movestartpoint(GameObject dropdown)
    {
        if(dropdown.GetComponent<Dropdown>().value == 0 || dropdown.GetComponent<Dropdown>().value == 2)
        {
            ball.transform.position = lrinit_pos;
        }
        else
        {
            ball.transform.position = rlinit_pos;
        }
        if(dropdown.GetComponent<Dropdown>().value == 2)
        {
            HorizontalStepSlider.SetActive(true);
            HorizontalStepText.SetActive(true);
            steptext.GetComponent<Text>().text = "Number of Vertical Steps: " + numsteps.GetComponent<Slider>().value;
        }
    }


    void timerchanged(GameObject timerslider)
    {
        timetext.GetComponent<Text>().text = "Seconds Per Fixation: " + timerslider.GetComponent<Slider>().value;
    }

    Vector3 MoveForward(Vector3 curr_pos)
    {
        curr_pos += distance;
        //curr_pos.x = (float)(curr_pos.x + 1);
        //curr_pos.y = (float)(curr_pos.y - 0.5);
        return curr_pos;

    }
    Vector3 MoveBackwards(Vector3 curr_pos)
    {
        curr_pos -= distance;
        /*
        curr_pos.x = (float)(curr_pos.x - 1);
        curr_pos.y = (float)(curr_pos.y + 0.5);
        */
        return curr_pos;

    }

    Vector3 MoveHorizontally(Vector3 currenthorizontalpos)
    {
        currenthorizontalpos += distancehorizontal;
        return currenthorizontalpos;
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


        if (UnityEngine.XR.XRSettings.isDeviceActive)
        {
            if (em.GetButtonDown(0))
            {
                canvas.SetActive(false);
            }
        }
        //DiagFixLR
        if (started && gametype ==0 || started && gametype == 1)
        {
            if (UnityEngine.XR.XRSettings.isDeviceActive)
            {
                recorder.Update();
            }
            timer += Time.deltaTime;
            if (started && timer > WaitingTime && N_repetitions > 0)
            {
                if (timer > WaitingTime && N_repetitions > 0)
                {
                    if (N_forward_steps < numsteps.GetComponent<Slider>().value)
                    {
                        step = MoveForward(step);
                        ball.transform.position = step;
                        timer = 0;
                        N_forward_steps++;
                    }
                    else
                    {
                        if (N_Back_steps < numsteps.GetComponent<Slider>().value)
                        {
                            step = MoveBackwards(step);
                            ball.transform.position = step;
                            timer = 0;
                            N_Back_steps++;
                        }
                        else
                        {
                            N_Back_steps = N_SMALL_STEPS;
                            N_forward_steps = N_SMALL_STEPS;
                            N_repetitions--;
                        }

                    }
                }
            }
            else if (N_repetitions == 0 && timer > WaitingTime)
            {
                canvas.SetActive(true);
                if (UnityEngine.XR.XRSettings.isDeviceActive)
                {
                    OnDestroy();
                }
                started = false;
                N_repetitions = 2;
                N_forward_steps = 0;
                N_Back_steps = 0;
                SettingsCanvas.SetActive(true);
            }
        }
        
        
        //Vertical fix
        if(started && gametype == 2)
        {
            if (UnityEngine.XR.XRSettings.isDeviceActive)
            {
                recorder.Update();
            }
            timer += Time.deltaTime;
            if (started && timer > WaitingTime && ball.transform.position != endpos)
            {
                if (timer > WaitingTime && N_repetitions > 0)
                {
                    if (N_forward_steps < numsteps.GetComponent<Slider>().value)
                    {
                        step = MoveForward(step);
                        ball.transform.position = step;
                        timer = 0;
                        N_forward_steps++;
                    }
                }
            }
            else if (ball.transform.position == endpos && timer > WaitingTime)
            {
                Debug.Log(ball.transform.position);
                if (numhorizontals != HorizontalStepSlider.GetComponent<Slider>().value)
                {
                    endpos = MoveHorizontally(endpos);
                    lasthorrizontalpos = MoveHorizontally(lasthorrizontalpos);
                    ball.transform.position = lasthorrizontalpos;
                    step = ball.transform.position;
                    N_forward_steps = 0;
                    numhorizontals++;
                    timer = 0;
                }
                else
                {
                    canvas.SetActive(true);
                    lasthorrizontalpos = lrinit_pos;
                    if (UnityEngine.XR.XRSettings.isDeviceActive)
                    {
                        OnDestroy();
                    }
                    started = false;
                    ball.transform.position = lrinit_pos;
                    endpos = lrinit_end_pos;
                    N_repetitions = 2;
                    N_forward_steps = 0;
                    N_Back_steps = 0;
                    numhorizontals = 0;
                    SettingsCanvas.SetActive(true);
                    timer = 0;
                }
            }
        }

    }

    public void startGame()
    {
        gametype = typedropdown.GetComponent<Dropdown>().value;
        WaitingTime = (int)timerslider.GetComponent<Slider>().value;
        if (gametype == 0)
        {
            //ball.transform.position = lrinit_pos;
            init_pos = ball.transform.position;
            endpos = endball.transform.position;
            step = ball.transform.position;
            if (UnityEngine.XR.XRSettings.isDeviceActive)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "Fixation_Diagonal_LeftRight_" + WaitingTime + "_SecondsPerFixation_" + (int)numsteps.GetComponent<Slider>().value + "_FixationSteps",
                    Version = 1,
                }, et);
            }
        }
        else if(gametype == 1)
        {
            //ball.transform.position = rlinit_pos;
            init_pos = startballrl.transform.position;
            endpos = endballrl.transform.position;
            step = ball.transform.position;
            if (UnityEngine.XR.XRSettings.isDeviceActive)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "Fixation_Diagonal_RightLeft_" + WaitingTime + "_SecondsPerFixation_" + (int)numsteps.GetComponent<Slider>().value + "_FixationSteps",
                    Version = 1,
                }, et);
            }
        }
        else
        {
            init_pos = ball.transform.position;
            endpos = endballrl.transform.position;
            step = ball.transform.position;
            horizontalsteps = (int)HorizontalStepSlider.GetComponent<Slider>().value;
            Vector3 linehorizontal = lrinit_end_pos - rlinit_end_pos;
            distancehorizontal = linehorizontal / horizontalsteps;
            if (UnityEngine.XR.XRSettings.isDeviceActive)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "Fixation_Vertical_LeftRight_" + WaitingTime + "_SecondsPerFixation_" + (int)numsteps.GetComponent<Slider>().value + "_VerticalSteps_"+ horizontalsteps +"_HorizontalSteps",
                    Version = 1,
                }, et);
            }
        }
        float value = numsteps.GetComponent<Slider>().value;
        Vector3 line = endpos - init_pos;
        distance = line / value;

        SettingsCanvas.SetActive(false);
        started = true;
    }

    void OnDestroy()
    {
        recorder.Commit();
    }
}
