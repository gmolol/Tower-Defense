using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Fades to black, loads a scene in the background, then fades back in
public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;

    public CanvasGroup group;
    public Slider progressBar;        // optional
    public float fadeTime = 0.4f;
    public float minLoadTime = 0.6f;  // stops the loading screen from just flashing

    bool busy;

    void Awake()
    {
        if (Instance == null) Instance = this;
        SetAlpha(0f);
    }

    public void Load(string scene)
    {
        if (!busy) StartCoroutine(LoadRoutine(scene));
    }

    IEnumerator LoadRoutine(string scene)
    {
        busy = true;
        PersistentUI.Instance.CloseAll();
        yield return Fade(0f, 1f);

        AsyncOperation op = SceneManager.LoadSceneAsync(scene);
        op.allowSceneActivation = false;

        // progress stops at 0.9 until we allow activation
        for (float t = 0f; op.progress < 0.9f || t < minLoadTime; t += Time.unscaledDeltaTime)
        {
            if (progressBar != null) progressBar.value = Mathf.Min(op.progress / 0.9f, t / minLoadTime);
            yield return null;
        }

        op.allowSceneActivation = true;
        yield return op;
        yield return null;  // let the new scene's Start() run (map generation) behind the black screen

        Time.timeScale = 1f;
        yield return Fade(1f, 0f);
        busy = false;
    }

    // Unscaled time so it still works while the game is paused
    IEnumerator Fade(float from, float to)
    {
        for (float t = 0f; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            SetAlpha(Mathf.Lerp(from, to, t / fadeTime));
            yield return null;
        }
        SetAlpha(to);
    }

    void SetAlpha(float a)
    {
        group.alpha = a;
        group.blocksRaycasts = a > 0f;  // blocks clicks during the transition
    }
}