using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : Manager<PlayerData>
{
    public int Money { get; private set; }

    public System.Action OnUpdate;
    public List<BaseUnit> benchUnits = new List<BaseUnit>();

    private void Start()
    {
        Money = 250;
    }

    public bool CanAfford(int amount)
    {
        return amount <= Money;
    }

    public void SpendMoney(int amount)
    {
        Money -= amount;
        OnUpdate?.Invoke();
    }

    public void removeAtTile(Node node)
    {
        foreach(BaseUnit unit in benchUnits)
        {
            if (unit.CurrentNode == node) {
                benchUnits.Remove(unit);
                Debug.Log(benchUnits.Count);
            }
               
        }
    }    
    
}