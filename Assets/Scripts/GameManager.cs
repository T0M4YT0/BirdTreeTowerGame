using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public enum State { Title, Playing, Dead }

    [Header("References")]
    [SerializeField] private PLayerMovement player;
    [SerializeField] private ObstacleSpawner spawner;
    [SerializeField] private FogWall fog;
    [SerializeField] private CameraFollow cam;

    [Header("UI")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text messageText;

    [Header("Settings")]
    [SerializeField] private Vector3 playerStart = new Vector3(0f, 2f, 0f);
    [SerializeField] private float cameraStartY = 10f;
    [SerializeField] private float playerGravityScale = 1f;

    private State state;
    private int highScore;

    public State CurrentState => state;

    public State SetState 
    {
        set => state = value;
    }

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
                if (AnyButtonPressed())
                    StartGame();
                break;

            case State.Playing:
                if (scoreText != null)
                    scoreText.text = player.Score.ToString();
                if (player.IsDead)
                    GameOver();
                break;

            case State.Dead:
                if (RestartPressed())
                    GoToTitle();
                break;
        }
    }

    private void GoToTitle()
    {
        state = State.Title;
        player.ResetPlayer(playerGravityScale);
        spawner.ResetAll();
        fog.ResetFog();
        cam.ResetCamera(cameraStartY);

        SetUI("BIRDUP\n\nPress X to start", false);
    }

    private void StartGame()
    {
        state = State.Playing;
        player.Unfreeze(playerGravityScale);
        fog.Activate();

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
            + "Press OPTIONS to retry";

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

    private bool AnyButtonPressed()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            return true;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        var gp = Gamepad.current;
        if (gp != null)
        {
            if (gp.buttonSouth.wasPressedThisFrame) return true;
            if (gp.buttonWest.wasPressedThisFrame) return true;
            if (gp.buttonEast.wasPressedThisFrame) return true;
            if (gp.buttonNorth.wasPressedThisFrame) return true;
            if (gp.startButton.wasPressedThisFrame) return true;
        }

        return false;
    }

    private bool RestartPressed()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.rKey.wasPressedThisFrame) return true;
            if (Keyboard.current.spaceKey.wasPressedThisFrame) return true;
        }

        var gp = Gamepad.current;
        if (gp != null)
        {
            if (gp.startButton.wasPressedThisFrame) return true;
            if (gp.buttonSouth.wasPressedThisFrame) return true;
        }

        return false;
    }
}
