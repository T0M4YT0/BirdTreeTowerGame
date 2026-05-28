using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        FindFirstObjectByType<PLayerMovement>().Frozen = true;
        Time.timeScale = 0;
    }

    public void Play()
    {
        FindFirstObjectByType<PLayerMovement>().Frozen = false;
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
