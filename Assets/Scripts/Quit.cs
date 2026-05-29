using UnityEngine;
using UnityEngine.InputSystem;

public class Quit : MonoBehaviour
{
    public void OnQuit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
