using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameScene = "Game";

    public void Play() => SceneFader.Instance.Load(gameScene);

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Application.Quit does nothing in the Editor
#else
        Application.Quit();
#endif
    }
}