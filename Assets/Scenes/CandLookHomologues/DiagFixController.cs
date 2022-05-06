using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private int numstepsslidervalue;

    private Vector3 distance;
    private Vector3 distancehorizontal;
    private bool started;
    private int gametype;
    public GameObject timetext;
    public GameObject steptext;
    private int horizontalsteps;
    private Vector3 lasthorrizontalpos;
    private int numhorizontals;
    private float stepslidervalue;

    public GameObject HorizontalStepSlider;
    private int horizontalstepvalue;

    public GameObject HorizontalStepText;
    public GameObject startReplayButton;
    public GameObject PauseButton;
    public GameObject ExitReplayButton;

    private bool replay;
    private GameObject replayobject;
    private string[] gameprams;
    private bool pause;

    //private Varjo.XR.VarjoEventManager em;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("DetailForReplay") != null)
        {
            replay = true;
            replayobject = GameObject.Find("DetailForReplay");
        }
        else { replay = false; }
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
        }
        if (replay)
        {
            horizontalsteps = 0;
            SettingsCanvas.SetActive(false);

            //Set all settings to that of replay
            gameprams = replayobject.GetComponent<GameInfo>().GameName.Split('_');
            if (gameprams[1] + "_" + gameprams[2] == "Diagonal_Left Right")
            {
                gametype = 0;
                movestartpoint(typedropdown);
            }
            else if (gameprams[1] + "_" + gameprams[2] == "Diagonal_Right Left")
            {
                gametype = 1;
                movestartpoint(typedropdown);
            }
            else if (gameprams[1] + "_" + gameprams[2] == "Vertical_Left Right")
            {
                gametype = 2;
                movestartpoint(typedropdown);
                try
                {
                    horizontalsteps = Int32.Parse(gameprams[7]);
                    Console.WriteLine(horizontalsteps);
                }
                catch (FormatException)
                {
                    Console.WriteLine($"Unable to parse '{gameprams[7]}'");
                }
            }
            else
            {
                Debug.Log("something went wrong recognizing direction");
            }

            
            try
            {
                numstepsslidervalue = Int32.Parse(gameprams[5]);
                Console.WriteLine(numstepsslidervalue);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unable to parse '{gameprams[5]}'");
            }

            try
            {
                WaitingTime = Int32.Parse(gameprams[3]);
                Console.WriteLine(WaitingTime);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unable to parse '{gameprams[3]}'");
            }

            startReplayButton.SetActive(true);
            ExitReplayButton.SetActive(true);
            PauseButton.SetActive(false);

        }
        else
        {
            startReplayButton.SetActive(false);
            ExitReplayButton.SetActive(false);
            PauseButton.SetActive(false);
            gametype = typedropdown.GetComponent<Dropdown>().value;
            horizontalsteps = (int)HorizontalStepSlider.GetComponent<Slider>().value;
            WaitingTime = (int)timerslider.GetComponent<Slider>().value;
            numstepsslidervalue = (int)numsteps.GetComponent<Slider>().value;
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

        

    }

    void stepschanged(GameObject numsteps)
    {
        numstepsslidervalue = (int)numsteps.GetComponent<Slider>().value;
        if (typedropdown.GetComponent<Dropdown>().value == 2)
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
        horizontalsteps = (int)HorizontalStepSlider.GetComponent<Slider>().value;
        HorizontalStepText.GetComponent<Text>().text = "Number of Horizontal Steps: " + HorizontalStepSlider.GetComponent<Slider>().value;
    }

    void movestartpoint(GameObject typedropdown)
    {
        if (!replay)
        {
            gametype = typedropdown.GetComponent<Dropdown>().value;
        }
        if (gametype == 0 || gametype == 2)
        {
            ball.transform.position = lrinit_pos;
        }
        else
        {
            ball.transform.position = rlinit_pos;
        }
        if(gametype == 2)
        {
            HorizontalStepSlider.SetActive(true);
            HorizontalStepText.SetActive(true);
            steptext.GetComponent<Text>().text = "Number of Vertical Steps: " + numstepsslidervalue;
        }
    }


    void timerchanged(GameObject timerslider)
    {
        WaitingTime = (int)timerslider.GetComponent<Slider>().value;
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
        if (Input.GetKeyDown(KeyCode.Space) && UnityEngine.XR.XRSettings.isDeviceActive && !replay)
            et.calibrate();

        if (Input.GetKeyUp(KeyCode.Escape) && (xrrig.GetComponent<SimpleSmoothMouseLook>() != null))
        {
            Destroy(xrrig.GetComponent<SimpleSmoothMouseLook>());
        }


        if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
        {
            if (em.GetButtonDown(0))
            {
                canvas.SetActive(false);
            }
        }
        //DiagFixLR

        if (started)
        {
            if (!pause)
            {
                if (replay)
                {
                    //TODO Spawn Gaze Visualizers that update pos each frame equal to input gaze data
                }


                if (gametype == 0 || gametype == 1)
                {
                    if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
                    {
                        recorder.Update();
                    }
                    timer += Time.deltaTime;
                    if (started && timer > WaitingTime && N_repetitions > 0)
                    {
                        if (timer > WaitingTime && N_repetitions > 0)
                        {
                            if (N_forward_steps < numstepsslidervalue)
                            {
                                step = MoveForward(step);
                                ball.transform.position = step;
                                timer = 0;
                                N_forward_steps++;
                            }
                            else
                            {
                                if (N_Back_steps < numstepsslidervalue)
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
                        if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
                        {
                            OnDestroy();
                        }
                        started = false;
                        N_repetitions = 2;
                        N_forward_steps = 0;
                        N_Back_steps = 0;
                        if (!replay)
                        {
                            SettingsCanvas.SetActive(true);
                        }
                        else
                        {
                            startReplayButton.SetActive(true);
                            ExitReplayButton.SetActive(true);
                            PauseButton.SetActive(false);
                        }
                    }
                }


                //Vertical fix
                if (gametype == 2)
                {
                    if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
                    {
                        recorder.Update();
                    }
                    timer += Time.deltaTime;
                    if (started && timer > WaitingTime && ball.transform.position != endpos)
                    {
                        if (timer > WaitingTime && N_repetitions > 0)
                        {
                            if (N_forward_steps < numstepsslidervalue)
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
                        if (numhorizontals != horizontalsteps)
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
                            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
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
                            if (!replay)
                            {
                                SettingsCanvas.SetActive(true);
                            }
                            else
                            {
                                startReplayButton.SetActive(true);
                                ExitReplayButton.SetActive(true);
                                PauseButton.SetActive(false);
                            }
                            timer = 0;
                        }
                    }
                }
            }
            
        }
        

    }

    public void startGame()
    {
        
        if (gametype == 0)
        {
            //ball.transform.position = lrinit_pos;
            init_pos = ball.transform.position;
            endpos = endball.transform.position;
            step = ball.transform.position;
            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "Fixation_Diagonal_Left Right_" + WaitingTime + "_Seconds Per Fixation_" + numstepsslidervalue + "_Fixation Steps",
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
            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "Fixation_Diagonal_Right Left_" + WaitingTime + "_Seconds Per Fixation_" + numstepsslidervalue + "_Fixation Steps",
                    Version = 1,
                }, et);
            }
        }
        else
        {
            init_pos = ball.transform.position;
            endpos = endballrl.transform.position;
            step = ball.transform.position;
            Vector3 linehorizontal = lrinit_end_pos - rlinit_end_pos;
            distancehorizontal = linehorizontal / horizontalsteps;
            if (UnityEngine.XR.XRSettings.isDeviceActive && !replay)
            {
                recorder = new GameRecorder(new Util.Model.Game
                {
                    Name = "Fixation_Vertical_Left Right_" + WaitingTime + "_Seconds Per Fixation_" + numstepsslidervalue + "_Vertical Steps_"+ horizontalsteps +"_Horizontal Steps",
                    Version = 1,
                }, et);
            }
        }
        float value = numstepsslidervalue;
        Vector3 line = endpos - init_pos;
        distance = line / value;
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
            PauseButton.GetComponent<Image>().color = startReplayButton.GetComponent<Button>().colors.normalColor;
            pause = false;
            PauseButton.GetComponent<Text>().text = "Pause";
        }
        else
        {
            PauseButton.GetComponent<Image>().color = startReplayButton.GetComponent<Button>().colors.selectedColor;
            pause = true;
            PauseButton.GetComponent<Text>().text = "Resume";
        }

    }

    void OnDestroy()
    {
        recorder.Commit();
    }
}
