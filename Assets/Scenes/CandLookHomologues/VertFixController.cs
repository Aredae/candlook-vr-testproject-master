using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using Varjo.XR;

public class VertFixController : MonoBehaviour
{

    public GameObject ball;

    public GameObject xrrig;

    public GameObject canvas;

    private int WaitingTime = 1;
    private static float POS_DURATION = 1.0f;
    private static int N_SMALL_STEPS = 9;
    private static int N_BIG_STEPS = 4;
    private Vector3 init_pos;
    private Vector3 step;
    private EyeTracker et;
    private float elapsed;
    private float timer;
    private int N_forward_steps;
    private float steplength = 0.5f;
    private bool done = false;
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
            Name="VerticalFixationGame",
            Version=1,
            }, et);
        }

    }

    Vector3 MoveForward(Vector3 curr_pos)
    {
        //curr_pos.x = (float)(curr_pos.x + 1);
        curr_pos.y = (float)(curr_pos.y - steplength);
        return curr_pos;
        
    }
    Vector3 MoveRight(Vector3 curr_pos)
    {
        curr_pos.x = (float)(curr_pos.x + 2);
        curr_pos.y = (float)(curr_pos.y + (steplength*N_SMALL_STEPS));
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
        if (!canvas.activeInHierarchy && timer > WaitingTime && !done) {
                    if (N_forward_steps > 0)
                    {
                        step = MoveForward(step);
                        ball.transform.position = step;
                        timer = 0;
                        N_forward_steps--;
                    }
                    else if (N_BIG_STEPS > 0)
                    {
                        step = MoveRight(step);
                        ball.transform.position = step;
                        timer = 0;
                        N_forward_steps = N_SMALL_STEPS;
                        N_BIG_STEPS--;
                    } else {
                     done = true;
                    }
                     recorder.Update();

        }
        else if(done)
        {
            canvas.SetActive(true);
            ball.transform.position = init_pos;
            step = ball.transform.position;
            N_forward_steps = N_SMALL_STEPS;
            N_BIG_STEPS = 4;
            done = false;
        }
    }
}
