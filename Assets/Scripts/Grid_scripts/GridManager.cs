using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : Manager<GridManager>
{

    public GameObject terrainGrid;

    Graph graph;
    protected Dictionary<Team, int> startPositionPerTeam;

    public List<Tile> allTiles = new List<Tile>();


    public Node GetFreeNode(Team forTeam)
    {
        Tile[] tiles = FindObjectsOfType<Tile>();

        foreach (Tile tile in tiles)
        {
            if (!tile.isBench && tile.team == forTeam)
            {
                Node node = GetNodeForTile(tile);
                if (!node.IsOccupied)
                    return node;
            }
        }
        return null;
    }

    public Node GetRandomFreeNode(Team forTeam)
    {
        Tile[] tiles = FindObjectsOfType<Tile>();
        bool encontrado = false;
        System.Random random = new System.Random();

        while (!encontrado)
        {
            
            int randomNumber = random.Next(0, 27);
            if (!tiles[randomNumber].isBench && tiles[randomNumber].team == forTeam)
            {
                Node node = GetNodeForTile(tiles[randomNumber]);
                if (!node.IsOccupied)
                    return node;
            }
        }
        return null;
    }

    public void resetNodes()
    {
        Tile[] tiles = FindObjectsOfType<Tile>();

        foreach (Tile tile in tiles)
        {
            
             Node node = GetNodeForTile(tile);
             node.SetOccupied(false);
            
        }
        
    }

    public void correctNodes()
    {
        List<Node> nodes_list = new List<Node>();

        foreach(BaseUnit u in GameManager.Instance.team1Units)
        {
            nodes_list.Add(u.CurrentNode);
        }
        foreach (BaseUnit u in GameManager.Instance.team2Units)
        {
            nodes_list.Add(u.CurrentNode);
        }

        foreach (Node nodes in graph.Nodes)
        {
            if (!nodes_list.Contains(nodes))
                nodes.SetOccupied(false);
        }
    }


    public Node GetFreeShopNode(Team forTeam)
    {
        Tile[] tiles = FindObjectsOfType<Tile>();

        foreach(Tile tile in tiles)
        {
            if (tile.isBench && tile.team == forTeam)
            {
                Node node = GetNodeForTile(tile);
                if (!node.IsOccupied)
                    return node;
            }
        }
        return null;
    }

    public List<Node> GetNodesCloseTo(Node to)
    {
        return graph.Neighbors(to);
    }

    public List<Node> GetPath(Node from, Node to)
    {
        return graph.GetShortestPath(from, to);
    }

    public Node GetNodeForTile(Tile t)
    {
        var allNodes = graph.Nodes;

        for (int i = 0; i < allNodes.Count; i++)
        {
            if (t.transform.GetSiblingIndex() == allNodes[i].index)
            {
                return allNodes[i];
            }
        }

        return null;
    }
    

    public Tile GetTileForNode(Node n)
    {
        Tile[] tiles = FindObjectsOfType<Tile>();

        foreach (Tile tile in tiles)
        {
            if (n == GetNodeForTile(tile))
            {
                return tile;
            }
        }

        return null;
    }

    private new void Awake()
    {        
        base.Awake();
        allTiles = terrainGrid.GetComponentsInChildren<Tile>().ToList();
        InitializeGraph();
        startPositionPerTeam = new Dictionary<Team, int>();
        startPositionPerTeam.Add(Team.Team1, 0);
        startPositionPerTeam.Add(Team.Team2, graph.Nodes.Count - 1);
    }

    private void InitializeGraph()
    {

        graph = new Graph();

        for (int i = 0; i < allTiles.Count; i++)
        {
            
            Vector3 place = allTiles[i].transform.position;
            graph.AddNode(place);
        }

        var allNodes = graph.Nodes;

        foreach(Node from in allNodes)
        {
            foreach(Node to in allNodes)
            {
                if(Vector3.Distance(from.worldPosition, to.worldPosition) <= 1.3f && from != to)
                {
                    graph.AddEdge(from, to);
                }
            }
        }
    }

    public int fromIndex = 0;
    public int toIndex = 0;

    private void OnDrawGizmos()
    {
        if (graph == null)
            return;

        var allEdges = graph.Edges;
        if (allEdges == null)
            return;

        foreach (Edge e in allEdges)
        {
            Debug.DrawLine(e.from.worldPosition, e.to.worldPosition, Color.black, 100);
        }

        var allNodes = graph.Nodes;
        if (allNodes == null)
            return;

        foreach (Node n in allNodes)
        {
            Gizmos.color = n.IsOccupied ? Color.red : Color.green;
            Gizmos.DrawSphere(n.worldPosition, 0.1f);

        }

        if (fromIndex >= allNodes.Count || toIndex >= allNodes.Count)
            return;

        List<Node> path = graph.GetShortestPath(allNodes[fromIndex], allNodes[toIndex]);
        if (path.Count > 1)
        {
            for (int i = 1; i < path.Count; i++)
            {
                Debug.DrawLine(path[i - 1].worldPosition, path[i].worldPosition, Color.blue, 10);
            }
        }
    }
}
