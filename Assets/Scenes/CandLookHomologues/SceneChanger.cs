using PrettyConsole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;
using Util;
using Varjo.XR;

public class SceneChanger : MonoBehaviour
{
    public GameObject xrrig;
    private EyeTracker et;
    private VarjoEventManager em;
    public GameObject user;

    public GameObject toFixButton;
    public GameObject toSmoothButton;
    public GameObject toReadingButton;
    public GameObject toReplayButton;
    public GameObject toEtButton;
    public GameObject returnToLogin;



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
        if (GameObject.Find("InitalLoaderState") != null)
        {
            GameObject.Find("InitalLoaderState").GetComponent<InitalLoaderState>().setReturning(true);
        }
        SceneManager.LoadScene("LoginScene");
       // Application.Quit();
    }

    /*
    public IEnumerator InitRegularLoadersandDeinitVarjoLoader(List<XRLoader> loaders)
    {
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        foreach (XRLoader l in loaders)
        {
            if (l.GetType().ToString() == "Varjo.XR.VarjoLoader")
            {
                StartCoroutine(InitRegularLoaders(l));
            }
        }

    }

    public IEnumerator InitRegularLoaders(XRLoader loader)
    {
        yield return loader.Deinitialize();
        XRGeneralSettings.Instance.Manager.InitializeLoader();
    }
    */
    public void recalibrateEyetracking()
    {
        XRGeneralSettings.Instance.Manager.StartSubsystems();

        /*
        List<XRLoader> loaders = XRGeneralSettings.Instance.Manager.loaders;
        for (int loaderIndex = loaders.Count - 1; loaderIndex >= 0; --loaderIndex)
        {
            XRLoader loader = loaders[loaderIndex];

            Debug.Log("Loader type: " + loader.GetType());

            if (loader.GetType().ToString() == "Varjo.XR.VarjoLoader")
            {

                loader.Initialize();
                loader.Start();

            }



            //  Debug.Log("Loader: "+ loader);
            //  loaders.RemoveAt(1);
            //  Debug.Log("Removed loader ");

        }

            /*
            foreach (XRLoader currloader in loaders)
            {
                if (currloader.GetType().ToString() == "Varjo.XR.VarjoLoader")
                {
                    currloader.Initialize();
                    currloader.Start();
                }
            }
            comment out here
        */
        Debug.Log("next code run");

        if (UnityEngine.XR.XRSettings.isDeviceActive)
        {
            Console.Log("et should start recalibration");
            et.calibrate();
            
        }
        else
        {
            Console.Log("Can't detect headset, do you have the headset properly connected?");
        }
    }

    IEnumerator WaitAndPerformEt()
    {
        toFixButton.SetActive(false);
        toSmoothButton.SetActive(false);
        toReadingButton.SetActive(false);
        toReplayButton.SetActive(false);
        toEtButton.SetActive(false);
        returnToLogin.SetActive(false);
        et.calibrate();
        yield return new WaitUntil(() => VarjoEyeTracking.GetGaze().status.Equals(VarjoEyeTracking.GazeStatus.Valid));
        toFixButton.SetActive(true);
        toSmoothButton.SetActive(true);
        toReadingButton.SetActive(true);
        toReplayButton.SetActive(true);
        toEtButton.SetActive(true);
        returnToLogin.SetActive(true);
    }

    public void exit()
    {
     //   Application.Quit();
    }


    public void Replays()
    {
        SceneManager.LoadScene("Replays");
    }
}
