using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectPanel : MonoBehaviour
{
    public CharacterData[] characters;
    public Image portrait;
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    int index;

    void OnEnable() => Show();

    public void Next()     { index = (index + 1) % characters.Length; Show(); }
    public void Previous() { index = (index - 1 + characters.Length) % characters.Length; Show(); }

    public void Confirm()
    {
        PlayerSelection.character = characters[index];
        PersistentUI.Instance.CloseAll();
    }

    void Show()
    {
        var c = characters[index];
        portrait.sprite = c.portrait;
        nameText.text = c.displayName;
        descriptionText.text = c.description;
    }
}