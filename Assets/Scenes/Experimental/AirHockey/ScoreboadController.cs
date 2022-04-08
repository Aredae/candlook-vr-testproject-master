using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboadController : MonoBehaviour
{
    public UnityEngine.UI.Text playerScore;
    public UnityEngine.UI.Text enemyScore;

    public void SetScore(uint[] score)
    {
        playerScore.text = score[0].ToString();
        enemyScore.text = score[1].ToString();
    }
}
