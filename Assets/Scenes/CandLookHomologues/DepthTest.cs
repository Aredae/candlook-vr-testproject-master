using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varjo.XR;

public class DepthTest : MonoBehaviour
{
    public GameObject sphere1;
    public GameObject sphere2;
    public GameObject sphere3;
    public GameObject sphere4;
    public GameObject Canvas;
    public GameObject xrrig;
    // Start is called before the first frame update
    void Start()
    {
        /*
        sphere1.SetActive(false);
        sphere2.SetActive(false);
        sphere3.SetActive(false);
        sphere4.SetActive(false);
        */
        //MoveAndScale(900, 1800, 0.25f, sphere4);

        if (!UnityEngine.XR.XRSettings.isDeviceActive)
        {
            SimpleSmoothMouseLook mouseController = xrrig.AddComponent<SimpleSmoothMouseLook>();
        }
        else
        {
            /*
            em = VarjoEventManager.Instance;
            et = new VarjoET(Camera.main);
            //GazeVisualizer.spawn(et);
            recorder = new GameRecorder(new Util.Model.Game
            {
                Name = "BallGame",
                Version = 1,
            }, et);
            */
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && (xrrig.GetComponent<SimpleSmoothMouseLook>() != null))
        {
            Destroy(xrrig.GetComponent<SimpleSmoothMouseLook>());
        }
        if (Input.GetKeyUp(KeyCode.Escape) && (xrrig.GetComponent<SimpleSmoothMouseLook>() == null))
        {
            xrrig.AddComponent<SimpleSmoothMouseLook>();
        }
    }

}


