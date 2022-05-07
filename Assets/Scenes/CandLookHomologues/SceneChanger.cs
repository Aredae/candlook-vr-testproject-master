using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;
using Varjo.XR;

public class SceneChanger : MonoBehaviour
{
    public GameObject xrrig;
    private EyeTracker et;
    private VarjoEventManager em;
    public GameObject user;

    void Start()
    {
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
        if(GameObject.Find("SubjectInfo") != null)
        {
            user = GameObject.Find("SubjectInfo");
        }
        else{
            Debug.Log("This should not be possible in VR. How?");
        }
        
    }

    public void FixationTasks()
    {
        SceneManager.LoadScene("FixationScene");
    }
    public void SmoothPursuitTasks()
    {
        SceneManager.LoadScene("SmoothPursuitScene");
    }
    public void ReadingTasks()
    {
        SceneManager.LoadScene("ReadingScene");
    }
    public void D2Test()
    {
        SceneManager.LoadScene("D2test");
    }
    public void practice()
    {
        SceneManager.LoadScene("Practice");
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Trying to return to Main Menu");
    }
    public void Login()
    {
        Destroy(user);
        SceneManager.LoadScene("LoginScene");
    }

    public void recalibrateEyetracking()
    {
        if (!UnityEngine.XR.XRSettings.isDeviceActive)
        {
            et.calibrate();
        }
        else
        {
            Debug.Log("Can't detect headset, do you have the headset properly connected?");
        }
    }

    public void exit()
    {
        Application.Quit();
    }


    public void Replays()
    {
        SceneManager.LoadScene("Replays");
    }
}
