using UnityEngine;
using TMPro;

// Fills the champion cards, handles buying locked champions, and saves the player's pick
public class CharacterSelectPanel : MonoBehaviour
{
    public CharacterData[] characters;
    public CharacterCard[] cards;       // Char1, Char2, Char3 in order
    public TMP_Text coinsText;
    public bool closeOnPick = false;

    void Awake()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            int index = i; // local copy so each button remembers its own card
            cards[i].pickButton.onClick.AddListener(() => Pick(index));
        }
    }

    void OnEnable()
    {
        Wallet.Changed += RefreshCoins;

        for (int i = 0; i < cards.Length; i++)
        {
            bool hasCharacter = i < characters.Length;
            cards[i].gameObject.SetActive(hasCharacter);
            if (hasCharacter) cards[i].Show(characters[i]);
        }

        if (PlayerSelection.character == null)
            PlayerSelection.character = FirstUnlocked();

        Refresh();
    }

    void OnDisable() => Wallet.Changed -= RefreshCoins;

    // PICK button: buys the champion if locked, then selects it
    public void Pick(int index)
    {
        CharacterData c = characters[index];

        if (!c.IsUnlocked)
        {
            if (!Wallet.TrySpend(c.unlockCost))
            {
                cards[index].Flash("NEED " + c.unlockCost);
                return;
            }
            c.Unlock();
        }

        PlayerSelection.character = c;
        Refresh();
        if (closeOnPick) PersistentUI.Instance.CloseAll();
    }

    void Refresh()
    {
        for (int i = 0; i < cards.Length && i < characters.Length; i++)
            cards[i].Refresh(characters[i] == PlayerSelection.character);
        RefreshCoins();
    }

    void RefreshCoins()
    {
        if (coinsText != null) coinsText.text = "COINS: " + Wallet.Coins;
    }

    CharacterData FirstUnlocked()
    {
        foreach (var c in characters)
            if (c.IsUnlocked) return c;
        return null;
    }

    // Testing helpers: in Play mode, click the ⋮ on this component in the Inspector
    [ContextMenu("Debug: Add 100 Coins")]
    void AddTestCoins() => Wallet.Add(100);

    [ContextMenu("Debug: Reset Coins and Unlocks")]
    void ResetProgress()
    {
        Wallet.Reset();
        foreach (var c in characters) c.ResetUnlock();
        PlayerSelection.character = FirstUnlocked();
        if (isActiveAndEnabled) Refresh();
    }
}