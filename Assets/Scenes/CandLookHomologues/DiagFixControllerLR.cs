using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using Varjo.XR;

public class DiagFixControllerLR : MonoBehaviour
{

    public GameObject ball;

    public GameObject xrrig;

    public GameObject canvas;

    private int WaitingTime = 1;
    private static float POS_DURATION = 1.0f;
    private static int N_SMALL_STEPS = 9;
    private static int N_Back_steps = 9;
    private Vector3 init_pos;
    private Vector3 step;
    private EyeTracker et;
    private float elapsed;
    private float timer;
    private int N_forward_steps;
    private int N_repetitions = 2;
    private VarjoEventManager em;
    private GameRecorder recorder;

    // Start is called before the first frame update
    void Start()
    {
        init_pos = ball.transform.position;
        elapsed = UnityEngine.Time.realtimeSinceStartup;
        step = ball.transform.position;
        N_forward_steps = N_SMALL_STEPS;

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
                Name = "BallGameLR",
                Version = 1,
            }, et);
        }
    }

    Vector3 MoveForward(Vector3 curr_pos)
    {
        curr_pos.x = (float)(curr_pos.x - 1);
        curr_pos.y = (float)(curr_pos.y - 0.5);
        return curr_pos;

    }
    Vector3 MoveBackwards(Vector3 curr_pos)
    {
        curr_pos.x = (float)(curr_pos.x + 1);
        curr_pos.y = (float)(curr_pos.y + 0.5);
        return curr_pos;

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

        timer += Time.deltaTime;
        if (!canvas.activeInHierarchy && timer > WaitingTime && N_repetitions > 0)
        {
            if (timer > WaitingTime && N_repetitions > 0)
            {
                if (recorder != null)
                {
                    recorder.Update();
                }
                if (N_forward_steps > 0)
                {
                    step = MoveForward(step);
                    ball.transform.position = step;
                    timer = 0;
                    N_forward_steps--;
                }
                else
                {
                    if (N_Back_steps > 0)
                    {
                        step = MoveBackwards(step);
                        ball.transform.position = step;
                        timer = 0;
                        N_Back_steps--;
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
            N_repetitions = 2;
        }
    }
}
