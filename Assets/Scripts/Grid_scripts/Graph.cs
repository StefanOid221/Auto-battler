using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Graph
{
    private List<Node> nodes;
    private List<Edge> edges;

    public List<Node> Nodes => nodes;
    public List<Edge> Edges => edges;

    public Graph()
    {

        nodes = new List<Node>();
        edges = new List<Edge>();

    }

    public void AddNode(Vector3 worldPosition){

        nodes.Add(new Node(nodes.Count, worldPosition));
    }

    public void AddEdge(Node from, Node to){

        edges.Add(new Edge(from, to, 1));
    }

    public bool Adjacent(Node from, Node to)
    {
        foreach (Edge e in edges)
        {
            if (e.from == from && e.to == to)
                return true;
        }
        return false;
    }

    public List<Node> Neighbors(Node from)
    {
        List<Node> result = new List<Node>();

        foreach (Edge e in edges)
        {
            if (e.from == from)
                result.Add(e.to);
        }
        return result;
    }

    public float Distance(Node from, Node to)
    {
        foreach (Edge e in edges)
        {
            if (e.from == from && e.to == to)
                return e.GetWeight();
        }

        return Mathf.Infinity;
    }

}
    public class Node{

        public int index;
        public Vector3 worldPosition;

        private bool occupied = false;

        public Node(int index, Vector3 worldPosition){

            this.index = index;
            this.worldPosition = worldPosition;
            occupied = false;
        }

        public void SetOccupied(bool val){

            occupied = val;
        }

        public bool IsOccupied => occupied;
    }
    public class Edge{
        
        public Node from;
        public Node to;

        private float weight;

        public Edge(Node from, Node to, float weight){

            this.from = from;
            this.to = to;
            this.weight = weight;
        }

        public float GetWeight(){

            if (to.IsOccupied)
                return Mathf.Infinity;

            return weight;
        }
    }
