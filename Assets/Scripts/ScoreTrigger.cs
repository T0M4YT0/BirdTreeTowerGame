using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    private ScoreDisplay scoreDisplay;

    private void FixedUpdate()
    {
        if (scoreDisplay == null)
            scoreDisplay = FindFirstObjectByType<ScoreDisplay>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.TryGetComponent(out PLayerMovement player);
        player.Score += 1;

        scoreDisplay.UpdateScoreDisplay(player.Score);

        Destroy(gameObject);
    }
}
