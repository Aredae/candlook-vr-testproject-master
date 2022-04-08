using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Player
{
    Human = 0,
    Computer = 1
}

public class GameController : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject table;
    public Collider room;
    public ScoreboadController scoreboard;
    public GameObject puck;
    public GameObject playerPad;
    public GameObject enemyPad;
    public BoxCollider playerGoalCollider;
    public BoxCollider enemyGoalCollider;

    private const float PUCK_SPAWN_X = 0.84f;
    private const float PADDLE_SPAWN_X = 0.981f;

    private Player lastScorer = Player.Computer;

    private uint[] scores = new uint[2] { 0, 0 };

    void Start()
    {
        Physics.gravity = Vector3.down;
        PuckController puckController = puck.AddComponent<PuckController>();
        puckController.gameController = this;
        scoreboard.SetScore(scores);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || Varjo.XR.VarjoEventManager.Instance.GetButtonDown(0))
        {
            scores = new uint[2] { 0, 0 };
            scoreboard.SetScore(scores);
            lastScorer = Player.Computer;
            RespawnPad(Player.Human);
            RespawnPad(Player.Computer);
            RespawnPuck();
        }
    }

    public void RespawnPad(Player player)
    {
        GameObject pad = player == Player.Human ? playerPad : enemyPad;
        Rigidbody padBody = pad.GetComponent<Rigidbody>();
        padBody.velocity = Vector3.zero;
        padBody.angularVelocity = Vector3.zero;
        pad.transform.localPosition = new Vector3(
            player == Player.Human ? -PADDLE_SPAWN_X : PADDLE_SPAWN_X,
            1.0f,
            0f);
        pad.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
    }

    public void RespawnPuck()
    {
        Rigidbody puckBody = puck.GetComponent<Rigidbody>();
        puckBody.velocity = Vector3.zero;
        puckBody.angularVelocity = Vector3.zero;
        puck.transform.localPosition = new Vector3(
            lastScorer == Player.Human ? PUCK_SPAWN_X : -PUCK_SPAWN_X, // spawn on looser's side
            1.0f,
            0f);
        puck.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
    }

    public void ScorePoint(Player scorer)
    {
        scores[(int)scorer] += 1;
        lastScorer = scorer;
        scoreboard.SetScore(scores);
        RespawnPuck();
    }
}

[RequireComponent(typeof(Collider))]
class PuckController : MonoBehaviour
{
    public GameController gameController;
    void OnCollisionEnter(Collision collision)
    {
        if (GameObject.ReferenceEquals(collision.gameObject, gameController.room.gameObject))
            gameController.RespawnPuck();
        else if (GameObject.ReferenceEquals(collision.gameObject, gameController.playerGoalCollider.gameObject))
            gameController.ScorePoint(Player.Computer);
        else if (GameObject.ReferenceEquals(collision.gameObject, gameController.enemyGoalCollider.gameObject))
            gameController.ScorePoint(Player.Human);
    }
}
