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

    Dictionary<Team, List<BaseUnit>> unitsByTeam = new Dictionary<Team, List<BaseUnit>>();

    List<BaseUnit> team1Entities = new List<BaseUnit>();
    List<BaseUnit> team2Entities = new List<BaseUnit>();


    int unitsPerTeam = 4;

    public List<BaseUnit> GetUnitsAgainst(Team otherTeam)
    {
        if (otherTeam == Team.Team1)
            return team2Entities;
        else return team1Entities;
    }
    public void DebugFight()
    {
        for (int i = 0; i < unitsPerTeam; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, unitDatabase.allUnits.Count);
            BaseUnit newEntity = Instantiate(unitDatabase.allUnits[randomIndex].prefab, team2Parent);

            team2Entities.Add(newEntity);

            newEntity.Setup(Team.Team2, GridManager.Instance.GetFreeNode(Team.Team2));
        }
    }

    public void OnUnitBought(UnitDatabaseSO.UnitData entityData)
    {
        BaseUnit newEntity = Instantiate(entityData.prefab, team1Parent);
        newEntity.gameObject.name = entityData.name;
        team1Entities.Add(newEntity);

        newEntity.Setup(Team.Team1, GridManager.Instance.GetFreeNode(Team.Team1));
    }

    public void UnitDead(BaseUnit unit)
    {
        unitsByTeam[unit.myTeam].Remove(unit);

        Destroy(unit.gameObject);
    }

}

public enum Team
{
    Team1,
    Team2
}
