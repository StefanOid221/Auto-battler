using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : Manager<GridManager>
{

    public Tilemap grid;

    Graph graph;
    protected Dictionary<Team, int> startPositionPerTeam;

    public Node GetFreeNode(Team forTeam)
    {
        int startIndex = startPositionPerTeam[forTeam];
        int currentIndex = startIndex;

        tile_type type;
        if (forTeam is Team.Team1)
            type = tile_type.player_board;
        else type = tile_type.ia_board;

        while (graph.Nodes[currentIndex].IsOccupied)
        {
           
            if (startIndex == 0)
            {
                currentIndex++;
                if (currentIndex == graph.Nodes.Count)
                    return null;
            }
            else
            {
                currentIndex--;
                if (currentIndex == -1)
                    return null;
            }
        }
        return graph.Nodes[currentIndex];
    }

    public List<Node> GetNodesCloseTo(Node to)
    {
        return graph.Neighbors(to);
    }

    public List<Node> GetPath(Node from, Node to)
    {
        return graph.GetShortestPath(from, to);
    }

    private new void Awake()
    {
        base.Awake();
        InitializeGraph();
        startPositionPerTeam = new Dictionary<Team, int>();
        startPositionPerTeam.Add(Team.Team1, 0);
        startPositionPerTeam.Add(Team.Team2, graph.Nodes.Count - 1);
    }

    private void InitializeGraph()
    {

        graph = new Graph();
        for(int x = grid.cellBounds.xMin; x < grid.cellBounds.xMax; x++)
        {
            for(int y = grid.cellBounds.yMin; y < grid.cellBounds.yMax; y++)
            {
                Vector3Int localPosition = new Vector3Int(x, y, 0);
                if (grid.HasTile(localPosition))
                {
                    Vector3 worldPosition = grid.CellToWorld(localPosition);
                    tile_type type;
                    if (grid.GetColor(localPosition) == Color.red)
                        type = tile_type.ia_board;
                    else type = tile_type.player_board;
                    graph.AddNode(worldPosition,type);
                }
            }
        }

        var allNodes = graph.Nodes;

        foreach(Node from in allNodes)
        {
            foreach(Node to in allNodes)
            {
                if(Vector3.Distance(from.worldPosition, to.worldPosition) <= 1f && from != to)
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
