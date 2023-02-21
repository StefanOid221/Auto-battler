using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{

    public List<BaseUnit> allUnitsPrefab;

    Dictionary<Team, List<BaseUnit>> unitsByTeam = new Dictionary<Team, List<BaseUnit>>();

    int unitsPerTeam = 1;

    private void Start()
    {

        InstantiateUnits();
    }

    private void InstantiateUnits()
    {
        unitsByTeam.Add(Team.Team1, new List<BaseUnit>());
        unitsByTeam.Add(Team.Team2, new List<BaseUnit>());
        for (int i = 0; i< unitsPerTeam; i++)
        {
            //unit team 1
            int randomIndex = UnityEngine.Random.Range(0, allUnitsPrefab.Count - 1);
            BaseUnit newUnit = Instantiate(allUnitsPrefab[randomIndex]);
            unitsByTeam[Team.Team1].Add(newUnit);

            newUnit.Setup(Team.Team1, GridManager.Instance.GetFreeNode(Team.Team1));

            //unit team 2
            randomIndex = UnityEngine.Random.Range(0, allUnitsPrefab.Count - 1);
            newUnit = Instantiate(allUnitsPrefab[randomIndex]);
            unitsByTeam[Team.Team2].Add(newUnit);

            newUnit.Setup(Team.Team2, GridManager.Instance.GetFreeNode(Team.Team2));


        }
    }



}

public enum Team
{
    Team1,
    Team2
}
