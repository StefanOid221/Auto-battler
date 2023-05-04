using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : Manager<GameManager>
{

    public List<BaseUnit> allUnitsPrefab;

    public UnitDatabaseSO unitDatabase;
    public Transform team1Parent;
    public Transform team2Parent;

    List<BaseUnit> team1Units = new List<BaseUnit>();
    List<BaseUnit> team2Units = new List<BaseUnit>();

    public List<BaseUnit> team1BenchUnits = new List<BaseUnit>();
    public List<BaseUnit> team2BenchUnits = new List<BaseUnit>();

    public List<BaseUnit> team1BoardhUnits = new List<BaseUnit>();
    public List<BaseUnit> team2BoardhUnits = new List<BaseUnit>();


    int unitsPerTeam = 4;

    public List<BaseUnit> GetUnitsAgainst(Team otherTeam)
    {
        if (otherTeam == Team.Team1)
            return team2Units;
        else return team1Units;
    }
    public void DebugFight()
    {
        for (int i = 0; i < unitsPerTeam; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, unitDatabase.allUnits.Count);
            BaseUnit newUnit = Instantiate(unitDatabase.allUnits[randomIndex].prefab, team2Parent);

            team2Units.Add(newUnit);

            newUnit.Setup(Team.Team2, GridManager.Instance.GetFreeNode(Team.Team2));
            newUnit.isBenched = false;
        }
    }

    public void OnUnitBought(UnitDatabaseSO.UnitData entityData)
    {
        BaseUnit newUnit = Instantiate(entityData.prefab, team1Parent);
        newUnit.gameObject.name = entityData.name;
        team1Units.Add(newUnit);
        team1BenchUnits.Add(newUnit);

        newUnit.Setup(Team.Team1, GridManager.Instance.GetFreeShopNode(Team.Team1));
    }

    public void UnitDead(BaseUnit unit)
    {
        team1Units.Remove(unit);
        team2Units.Remove(unit);

        Destroy(unit.gameObject);
    }

    public void removeAtTile(Node tileNode)
    {
        List<BaseUnit> unitsToRemove = new List<BaseUnit>();

        foreach (BaseUnit unit in team1BenchUnits)
        {
            if (unit.CurrentNode == tileNode)
            {
                unitsToRemove.Add(unit);
            }
        }

        foreach (BaseUnit unit in unitsToRemove)
        {
            team1BenchUnits.Remove(unit);
        }
    }

}

public enum Team
{
    Team1,
    Team2
}
