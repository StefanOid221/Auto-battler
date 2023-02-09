using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    private List<Node> nodes;
    private List<Edge> edges;

    public Graph(){

        nodes = new List<Node>();
        edges = new List<Edge>();

    }

    public void AddNode(Vector3 worldPosition){

        nodes.Add(new Node(nodes.Count, worldPosition));
    }

    public void AddEdge(Node from, Node to){

        edges.Add(new Edge(from, to, 1));
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
