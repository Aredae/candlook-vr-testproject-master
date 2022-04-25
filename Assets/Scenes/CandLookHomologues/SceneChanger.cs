using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
   public void DiagFixRL()
    {
        SceneManager.LoadScene("DiagFixRL");
    }
    public void DiagFixLR()
    {
        SceneManager.LoadScene("BallGame");
    }
    public void SmoothDiagRL()
    {
        SceneManager.LoadScene("SmoothDiagRL");
    }
    public void SmoothDiagLR()
    {
        SceneManager.LoadScene("SmoothDiagLR");
    }
    public void SmoothHorizontalLR()
    {
        SceneManager.LoadScene("SmoothHorizontalLR");
    }
    public void SmoothHorizontalRL()
    {
        SceneManager.LoadScene("SmoothHorizontalRL");
    }

    public void VerticalFixationsLR()
    {
        SceneManager.LoadScene("VerticalFixations");
    }

    public void Replays()
    {
        SceneManager.LoadScene("Replays");
    }
}
