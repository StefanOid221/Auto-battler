using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData Database", menuName = "CustomSO/UnitDataDatabase")]
public class UnitDatabaseSO : ScriptableObject
{
    [System.Serializable]
    public struct UnitData
    {
        public BaseUnit prefab;
        public string name;
        public Sprite icon;

        public int cost;
    }

    public List<UnitData> allUnits;
}