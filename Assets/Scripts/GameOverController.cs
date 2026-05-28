using UnityEngine;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text scoreTxt;
    [SerializeField] private Transform gameUI;

    private PLayerMovement playerMovement;
    private ObstacleSpawner obstacleSpawner;
    private CameraFollow cam;
    private ScoreDisplay scoreDisplay;
    private BackgroundTiller bgTiller;

    [SerializeField]private GameObject gameOverScreen;
    
    private void Start()
    {
        playerMovement = FindFirstObjectByType<PLayerMovement>();
        obstacleSpawner = FindFirstObjectByType<ObstacleSpawner>();
        cam = FindFirstObjectByType<CameraFollow>();
        scoreDisplay = gameUI.GetComponent<ScoreDisplay>();
        bgTiller = FindFirstObjectByType<BackgroundTiller>();
    }

    private void Update()
    {
        if (playerMovement.IsDead)
        {
            gameUI.gameObject.SetActive(false);
            gameOverScreen.SetActive(true);
        }
        else
            gameOverScreen.SetActive(false);
    }

    public void DisplayScore(int score)
    {
        scoreTxt.text = score.ToString();
    }

    public void RestartGame()
    {
        playerMovement.ResetPlayer();
        cam.ResetCamera();
        obstacleSpawner.ResetAll();
        bgTiller.ResetTiler();
        scoreTxt.text = "0";
        gameUI.gameObject.SetActive(true);
        scoreDisplay.UpdateScoreDisplay(0);
        playerMovement.Unfreeze(1f);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
