using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameManager : Manager<GameManager>
{

    public List<BaseUnit> allUnitsPrefab;

    public UnitDatabaseSO unitDatabase;
    public Transform team1Parent;
    public Transform team2Parent;

    public int gamesWonPlayer = 0;
    public int gamesWonAI = 0;

    public List<BaseUnit> team1Units = new List<BaseUnit>();
    public List<BaseUnit> team2Units = new List<BaseUnit>();

    public List<BaseUnit> team1BenchUnits = new List<BaseUnit>();
    public List<BaseUnit> team2BenchUnits = new List<BaseUnit>();

    public List<BaseUnit> team1BoardUnits = new List<BaseUnit>();
    public List<BaseUnit> team2BoardUnits = new List<BaseUnit>();

    public List<BaseUnit> team1CopyBoardUnits = new List<BaseUnit>();
    public List<BaseUnit> team2CopyBoardUnits = new List<BaseUnit>();

    public GameObject playerShop;
    public UIShop playerShopRef;

    public Text time;
    public Text state;
    public Text roundsPlayer;
    public Text roundsAi;

    public bool unitsFighting = false;


    int unitsPerTeam = 4;

    public float decisionTime = 10f; 
    public GameState gameState;
    private float stateTimer;

    void Start()
    {
        SetState(GameState.Decision);
        roundsPlayer.text = "Rounds won by player: " + gamesWonPlayer.ToString();
        roundsAi.text = "Rounds won by opponent: " + gamesWonAI.ToString();

        playerShopRef = playerShop.GetComponent<UIShop>();
    }

    void Update()
    {
        switch (gameState)
        {
            case GameState.Decision:
                state.text = "Decision round";
                stateTimer += Time.deltaTime;
                
                if (stateTimer >= decisionTime)
                {
                    SetState(GameState.Fight);
                    unitsFighting = true;
                }
                IA_Manager.Instance.buyCard();
                break;
            case GameState.Fight:
                state.text = "Fight round";
                DebugFight();
                break;
        }
        time.text = "Time:" + (decisionTime -((int)stateTimer)).ToString();
    }

    void SetState(GameState newState)
    {
        gameState = newState;
        stateTimer = 0f;
    }


    public List<BaseUnit> GetUnitsAgainst(Team otherTeam)
    {
        if (otherTeam == Team.Team1)
            return team2CopyBoardUnits;
        else return team1CopyBoardUnits;
    }
    public void DebugFight()
    {
        if (unitsFighting) {
            foreach (BaseUnit unit in team1BoardUnits){
                team1CopyBoardUnits.Add(unit);
            }
            foreach (BaseUnit unit in team2BoardUnits)
            {
                team2CopyBoardUnits.Add(unit);
            }

            //team2CopyBoardUnits = team2BoardUnits;
            //for (int i = 0; i < unitsPerTeam; i++)
            //{
            //    int randomIndex = UnityEngine.Random.Range(0, unitDatabase.allUnits.Count);
            //    BaseUnit newUnit = Instantiate(unitDatabase.allUnits[randomIndex].prefab, team2Parent);

            //    team2Units.Add(newUnit);
            //    team2BoardUnits.Add(newUnit);

            //    newUnit.Setup(Team.Team2, GridManager.Instance.GetFreeNode(Team.Team2));
            //    newUnit.isBenched = false;
            //}
            unitsFighting = false;
        }
        if (team1CopyBoardUnits.Count == 0 || team2CopyBoardUnits.Count == 0)
        {
            SetState(GameState.Decision);
            IA_Manager.Instance.shopRef.RefreshEndRound();
            playerShopRef.RefreshEndRound();
            team1CopyBoardUnits.Clear();
            team2CopyBoardUnits.Clear();
            foreach (BaseUnit unit in team1BoardUnits)
            {
                unit.respawn();
            }
            foreach(BaseUnit unit in team2BoardUnits)
            {
                unit.respawn();
            }
            if (team1CopyBoardUnits.Count == 0)
                gamesWonAI += 1;
            else gamesWonPlayer += 1;
            roundsPlayer.text = "Rounds won by player: " + gamesWonPlayer.ToString();
            roundsAi.text = "Rounds won by opponent: " + gamesWonAI.ToString();
            PlayerData.Instance.moneyEndRound();
            IAData.Instance.moneyEndRound();
        }
        
        
    }

    public void OnUnitBought(UnitDatabaseSO.UnitData entityData, Player player)
    {
        if (player == Player.Player)
        {
            BaseUnit newUnit = Instantiate(entityData.prefab, team1Parent);
            newUnit.gameObject.name = entityData.name;
            team1Units.Add(newUnit);
            team1BenchUnits.Add(newUnit);

            newUnit.Setup(Team.Team1, GridManager.Instance.GetFreeShopNode(Team.Team1));
            checkLevelUp(newUnit, player);
        }
        else if (player == Player.IA_Player)
        {
            BaseUnit newUnit = Instantiate(entityData.prefab, team2Parent);
            newUnit.gameObject.name = entityData.name;
            team2Units.Add(newUnit);
            team2BenchUnits.Add(newUnit);

            newUnit.Setup(Team.Team2, GridManager.Instance.GetFreeShopNode(Team.Team2));
            checkLevelUp(newUnit, player);
        }
        
    }

    public void UnitDead(BaseUnit unit)
    {
        team1CopyBoardUnits.Remove(unit);
        team2CopyBoardUnits.Remove(unit);

        unit.gameObject.SetActive(false);
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

    public void checkLevelUp(BaseUnit unit, Player player)
    {
        List<BaseUnit> unitsToRemove = new List<BaseUnit>();
        BaseUnit unitLevelUp = null;
        if (unit.level == 3)
            return;
        if (player == Player.Player)
        {
            foreach (BaseUnit u in team1Units)
            {
                if (u.unitType == unit.unitType && u.level == unit.level)
                {
                    if (unitsToRemove.Count == 2)
                    {
                        unitLevelUp = u;
                        break;
                    }
                    else unitsToRemove.Add(u);
                }
            }
            if (unitsToRemove.Count < 2 || unitLevelUp == null) return;
            else
            {
                foreach (BaseUnit un in unitsToRemove)
                {
                    team1Units.Remove(un);
                    team1BenchUnits.Remove(un);
                    un.CurrentNode.SetOccupied(false);
                    Destroy(un.gameObject);
                }
                foreach (BaseUnit un2 in team1Units)
                {
                    if (GameObject.ReferenceEquals(un2.gameObject, unitLevelUp.gameObject))
                    {
                        un2.levelUp();
                        if (!un2.isBenched)
                        {
                            team1BoardUnits.Remove(un2);
                        }

                    }
                }
            }
        }
        else if (player == Player.IA_Player)
        {
            foreach (BaseUnit u in team2Units)
            {
                if (u.unitType == unit.unitType && u.level == unit.level)
                {
                    if (unitsToRemove.Count == 2)
                    {
                        unitLevelUp = u;
                        break;
                    }
                    else unitsToRemove.Add(u);
                }
            }
            if (unitsToRemove.Count < 2 || unitLevelUp == null) return;
            else
            {
                foreach (BaseUnit un in unitsToRemove)
                {
                    team2Units.Remove(un);
                    team2BenchUnits.Remove(un);
                    un.CurrentNode.SetOccupied(false);
                    Destroy(un.gameObject);
                }
                foreach (BaseUnit un2 in team2Units)
                {
                    if (GameObject.ReferenceEquals(un2.gameObject, unitLevelUp.gameObject))
                    {
                        un2.levelUp();
                        if (!un2.isBenched)
                        {
                            team2BoardUnits.Remove(un2);
                        }

                    }
                }
            }
        }
        
    }



}

public enum Team
{
    Team1,
    Team2
}
public enum GameState
{
    Decision,
    Fight
}
