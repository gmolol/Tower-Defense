using UnityEngine;

[CreateAssetMenu(menuName = "Tower Defense/Character")]
public class CharacterData : ScriptableObject
{
    public string displayName;
    [TextArea] public string description;
    public Sprite portrait;
    public GameObject prefab;
    public int unlockCost = 0;   // 0 = free from the start

    // Saved under the asset's file name, so don't rename the asset once players have unlocks
    string SaveKey => "Unlocked_" + name;

    public bool IsUnlocked => unlockCost == 0 || PlayerPrefs.GetInt(SaveKey, 0) == 1;

    public void Unlock()
    {
        PlayerPrefs.SetInt(SaveKey, 1);
        PlayerPrefs.Save();
    }

    public void ResetUnlock() => PlayerPrefs.DeleteKey(SaveKey);
}