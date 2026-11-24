using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// One champion card: shows the character and whether it's locked, unlocked, or picked
public class CharacterCard : MonoBehaviour
{
    public Image portrait;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public Button pickButton;
    public TMP_Text pickLabel;
    public GameObject lockIcon;                                  // optional
    public Color lockedTint = new Color(0.15f, 0.15f, 0.15f);    // dark silhouette while locked

    CharacterData data;

    public void Show(CharacterData c)
    {
        data = c;
        portrait.sprite = c.portrait;
        portrait.enabled = c.portrait != null; // hides the empty white box until there's art
        nameText.text = c.displayName;
        descriptionText.text = c.description;
    }

    public void Refresh(bool picked)
    {
        StopAllCoroutines();
        bool unlocked = data.IsUnlocked;

        portrait.color = unlocked ? Color.white : lockedTint;
        if (lockIcon != null) lockIcon.SetActive(!unlocked);

        if (!unlocked) pickLabel.text = "BUY " + data.unlockCost;
        else pickLabel.text = picked ? "PICKED" : "PICK";

        pickButton.interactable = !(unlocked && picked);
    }

    // Briefly swaps the button text, e.g. "NEED 100"
    public void Flash(string message)
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine(message));
    }

    IEnumerator FlashRoutine(string message)
    {
        string old = pickLabel.text;
        pickLabel.text = message;
        yield return new WaitForSecondsRealtime(1f); // realtime: the game is paused while menus are open
        pickLabel.text = old;
    }
}