using UnityEngine;
using UnityEngine.InputSystem;

// Holds the Settings and Character Select panels; survives scene changes
public class PersistentUI : MonoBehaviour
{
    public static PersistentUI Instance;

    public GameObject settingsPanel;
    public GameObject characterPanel;
    public SettingsPanel settings;

    float prevTimeScale = 1f;

    // Spawns the prefab from Resources before any scene loads, so it exists in every scene
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        var prefab = Resources.Load<GameObject>("PersistentUI");
        if (prefab != null) Instantiate(prefab);
        else Debug.LogError("PersistentUI prefab missing from Assets/Resources");
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        settingsPanel.SetActive(false);
        characterPanel.SetActive(false);
    }

    void Start()
    {
        settings.ApplySaved(); // apply saved volume at launch, even if the panel is never opened
    }

    // Esc toggles settings, which doubles as a pause menu in-game
    void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame) return;
        if (AnyOpen()) CloseAll();
        else OpenSettings();
    }

    public void OpenSettings()   => Open(settingsPanel);
    public void OpenCharacters() => Open(characterPanel);

    void Open(GameObject panel)
    {
        if (!AnyOpen()) { prevTimeScale = Time.timeScale; Time.timeScale = 0f; } // pause the game
        settingsPanel.SetActive(panel == settingsPanel);
        characterPanel.SetActive(panel == characterPanel);
    }

    public void CloseAll()
    {
        if (!AnyOpen()) return;
        settingsPanel.SetActive(false);
        characterPanel.SetActive(false);
        Time.timeScale = prevTimeScale; // restores 2x speed etc., not just 1
    }

    bool AnyOpen() => settingsPanel.activeSelf || characterPanel.activeSelf;
}