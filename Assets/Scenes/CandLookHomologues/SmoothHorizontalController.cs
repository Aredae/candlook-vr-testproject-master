using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using Varjo.XR;

public class SmoothHorizontalController : MonoBehaviour
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
    private int N_Lines = 5;
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

    Vector3 MoveDown(Vector3 curr_pos)
    {
        curr_pos.y = (float)(curr_pos.y - 1);
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

        if (!canvas.activeInHierarchy && N_Lines > 0)
        {
            if (N_Lines > 0)
            {
                if (recorder != null)
                {
                    recorder.Update();
                }
                ball.transform.position = Vector3.MoveTowards(ball.transform.position, destination.transform.position, speed);

                if(ball.transform.position == destination.transform.position)
                {
                    curr_start = MoveDown(curr_start);
                    curr_dest = MoveDown(curr_dest);
                    ball.transform.position = curr_start;
                    destination.transform.position = curr_dest;
                    N_Lines--;
                }
            }
        }
        else if (N_Lines == 0)
        {
            ball.transform.position = ball_init_pos;
            destination.transform.position = dest_init_pos;
            curr_start = ball_init_pos;
            curr_dest = dest_init_pos;
            canvas.SetActive(true);
            N_Lines = 5;
        }
    }
}
