using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 0;
    }

    public void Play()
    {
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
