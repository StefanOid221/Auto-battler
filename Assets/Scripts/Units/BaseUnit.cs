using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseUnit : MonoBehaviour
{
    public GameObject gameObject;
    public Animator animator;

    public int baseDamage = 1;
    public int baseHealth = 3;
    [Range(1, 5)]
    public int range = 1;
    public float attackSpeed = 1f; //Attacks per second
    public float movementSpeed = 1f; //Attacks per second

    protected Team myTeam;
    protected Node currentNode;

    public void Setup(Team team, Node spawnNode)
    {
        myTeam = team;
        this.currentNode = spawnNode;
        transform.position = currentNode.worldPosition;
        currentNode.SetOccupied(true);
    }
}
