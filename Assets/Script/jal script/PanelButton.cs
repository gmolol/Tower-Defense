using UnityEngine;

// Scene buttons can't reference the persistent UI in the Inspector (it only exists at runtime)
public class PanelButton : MonoBehaviour
{
    public void OpenSettings()   => PersistentUI.Instance.OpenSettings();
    public void OpenCharacters() => PersistentUI.Instance.OpenCharacters();
}