using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "UnitList", menuName = "Scriptable Objects/UnitList")]
public class UnitList : ScriptableObject
{
    public List<UnitData> unit_list = new List<UnitData>();
}
