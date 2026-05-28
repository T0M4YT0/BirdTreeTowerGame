using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text scoreText;

    public void UpdateScoreDisplay(int score) => scoreText.text = score.ToString();
}
