using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class IA_Agent : Agent
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject terrain;

    private bool isActionCompleted;
    private bool isRewardChecked;
    private bool allMoved;
    private int gamesWonAI;
    private int gamesWonPlayer;
    public override void OnEpisodeBegin()
    {
        //gameManager.resetGame();
        isActionCompleted = true;
        
        isRewardChecked = false;
        gamesWonAI = gameManager.gamesWonAI;
        gamesWonPlayer = gameManager.gamesWonPlayer;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        if (isActionCompleted)
        {
            Tile[] tiles = FindObjectsOfType<Tile>();
            foreach (BaseUnit unit in gameManager.team1BoardUnits)
            {
                if (unit != null)
                    sensor.AddObservation(unit.transform.position);
            }

            foreach (BaseUnit unit in gameManager.team2BoardUnits)
            {
                if (unit != null)
                    sensor.AddObservation(unit.transform.position);
            }

            sensor.AddObservation(IAData.Instance.level);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        
        if (isActionCompleted && !allMoved && GameManager.Instance.gameState.Equals(GameState.Decision))
        {
            
            isActionCompleted = false;
            Tile[] tiles = FindObjectsOfType<Tile>();

            List<Tile> tiles_to_move = new List<Tile>();
            foreach (Tile t in tiles)
            {
                if (t.team == Team.Team2)
                    tiles_to_move.Add(t);
            }
            for (int i = 0; i < gameManager.team2Units.Count; i++)
            {


                Node node = gridManager.GetNodeForTile(tiles_to_move[actions.DiscreteActions[i]]);
                if (!node.IsOccupied)
                {
                    gameManager.team2Units[i].moveToNode(node);
                }
                allMoved = true;
                

            }
            int benchUnits = gameManager.team2BenchUnits.Count;
            float benchReward = benchUnits * 0.1f;
            AddReward(benchReward);
            CheckExceededUnits();
            gameManager.FightCompleted += OnFightCompleted;
        }
        
        


    }

    public void checkReward()
    {
        if (!isRewardChecked)
        {
            //Debug.Log(gamesWonAI);
            //Debug.Log(gameManager.gamesWonAI);
            if (gameManager.gamesWonAI > gamesWonAI)
            {
                AddReward(0.7f);
                
            }
            else if (gameManager.gamesWonPlayer > gamesWonPlayer)
            {
                AddReward(-0.7f);
            }
            isRewardChecked = true; // Mark the reward as checked
            EndEpisode();
        }
    }

    private void OnFightCompleted()
    {
        // Remove the event listener

        gameManager.FightCompleted -= OnFightCompleted;
        // Call checkReward() only if it has not been checked already in this round
        if (!isRewardChecked)
        {
            checkReward();
             // Mark the reward as checked
        }

        // Reset relevant variables for the next round
        isActionCompleted = true;
        allMoved = false;

        // Request the agent to take the next decision
        RequestDecision();
    }
    private void CheckExceededUnits()
    {

        int placedUnits = 0;
        Tile[] tiles = FindObjectsOfType<Tile>();

        List<Node> node_to_move = new List<Node>();
        foreach (Tile t in tiles)
        {
            if (t.team == Team.Team2 && t.isBench)
            {
                Node node = gridManager.GetNodeForTile(t);
                if (node.IsOccupied == false)
                    node_to_move.Add(node);
            }
               
        }
        placedUnits = GameManager.Instance.team2BoardUnits.Count;
        if (placedUnits > IAData.Instance.level)
        {
            // Move exceeded units to the bench
            int unitsToMove = placedUnits - IAData.Instance.level;
            List<BaseUnit> temp = new List<BaseUnit>(gameManager.team2BoardUnits);
            for (int i = 0; i < unitsToMove; i++)
            {
                foreach (Node node in node_to_move)
                {
                    if (!node.IsOccupied)
                    {
                        temp[i].moveToNode(node);
                        AddReward(-0.2f);
                        break;
                    }
                }
            }

        }

    }


}
