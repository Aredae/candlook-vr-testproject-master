using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using Varjo.XR;


public class SmoothDiagController : MonoBehaviour
{

    public GameObject ball;

    public GameObject xrrig;

    public GameObject canvas;

    public GameObject destination;

    private int WaitingTime = 1;
    private Vector3 ball_init_pos;
    private Vector3 dest_init_pos;
    private Vector3 curr_start;
    private Vector3 curr_dest;
    private EyeTracker et;
    private float elapsed;
    private float timer;
    private int N_Times = 2;
    private int times;
    private VarjoEventManager em;
    private GameRecorder recorder;
    private float speed = 0.004f;

    // Start is called before the first frame update
    void Start()
    {
        ball_init_pos = ball.transform.position;
        dest_init_pos = destination.transform.position;
        curr_start = ball_init_pos;
        curr_dest = dest_init_pos;
        elapsed = UnityEngine.Time.realtimeSinceStartup;
        times = N_Times * 2;
        //step = ball.transform.position;

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

        if (!canvas.activeInHierarchy && times >= 0)
        {
            if (times >= 0)
            {
                if (recorder != null)
                {
                    recorder.Update();
                }
                ball.transform.position = Vector3.MoveTowards(ball.transform.position, destination.transform.position, speed);

                if (ball.transform.position == destination.transform.position)
                {
                    if (times % 2 == 0) {
                        destination.transform.position = dest_init_pos;
                        times--;
                    }
                    else
                    {
                        destination.transform.position = ball_init_pos;
                        times--;
                    }
                }
            }
        }
        else if (times < 0)
        {
            ball.transform.position = ball_init_pos;
            destination.transform.position = dest_init_pos;
            curr_start = ball_init_pos;
            curr_dest = dest_init_pos;
            canvas.SetActive(true);
            times = N_Times*2;
        }
    }
}
