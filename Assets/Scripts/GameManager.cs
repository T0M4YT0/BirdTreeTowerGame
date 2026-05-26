using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum State { Title, Playing, Dead }

    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private ObstacleSpawner spawner;
    [SerializeField] private FogWall fog;
    [SerializeField] private CameraFollow cam;

    [Header("UI")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text messageText;

    [Header("Settings")]
    [SerializeField] private Vector3 playerStart = new Vector3(0f, 2f, 0f);
    [SerializeField] private float cameraStartY = 10f;

    private State state;
    private int highScore;

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        GoToTitle();
    }

    private void Update()
    {
        switch (state)
        {
            case State.Title:
                if (FlapPressed())
                    StartGame();
                break;

            case State.Playing:
                if (scoreText != null)
                    scoreText.text = player.Score.ToString();
                if (player.IsDead)
                    GameOver();
                break;

            case State.Dead:
                if (Input.GetKeyDown(KeyCode.R))
                    GoToTitle();
                break;
        }
    }

    private void GoToTitle()
    {
        state = State.Title;
        player.ResetBird(playerStart);
        spawner.ResetAll();
        fog.ResetFog();
        cam.ResetCamera(cameraStartY);

        SetUI("BIRDUP\n\nTap to start", false);
    }

    private void StartGame()
    {
        state = State.Playing;
        fog.Activate();
        player.Flap(); // first flap to get going

        SetUI("", true);
    }

    private void GameOver()
    {
        state = State.Dead;
        int score = player.Score;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        string msg = "GAME OVER\n\n"
            + "Score: " + score + "\n"
            + "Height: " + player.HighestY.ToString("F0") + "m\n"
            + "Best: " + highScore + "\n\n"
            + "Press R to retry";

        SetUI(msg, true);
    }

    private void SetUI(string message, bool showScore)
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(message.Length > 0);
            messageText.text = message;
        }
        if (scoreText != null)
            scoreText.gameObject.SetActive(showScore);
    }

    private bool FlapPressed()
    {
        return Input.GetKeyDown(KeyCode.Space)
            || Input.GetMouseButtonDown(0)
            || Input.GetKeyDown(KeyCode.UpArrow)
            || Input.GetKeyDown(KeyCode.W);
    }
}
