using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
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
        SceneManager.LoadScene("D2Test");
    }


    public void Replays()
    {
        SceneManager.LoadScene("Replays");
    }
}
