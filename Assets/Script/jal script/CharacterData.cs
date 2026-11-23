using UnityEngine;

[CreateAssetMenu(menuName = "Tower Defense/Character")]
public class CharacterData : ScriptableObject
{
    public string displayName;
    [TextArea] public string description;
    public Sprite portrait;
    public GameObject prefab;
}