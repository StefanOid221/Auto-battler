using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseUnit : MonoBehaviour
{
    public GameObject gameObject;
    public Animator animator;
    protected HealthBar healthbar;
    public HealthBar barPrefab;

    public int baseDamage = 1;
    public int baseHealth = 3;
    [Range(1, 5)]
    public int range = 1;
    public float attackSpeed = 1f; //Attacks per second
    public float movementSpeed = 1f; //Attacks per second

    public bool moving;
    protected Node destination;

    public Team myTeam;
    protected Node currentNode;
    protected BaseUnit currentTarget;
    protected bool inRange => currentTarget != null && Vector3.Distance(this.transform.position, currentTarget.transform.position) <= range;

    protected bool hasEnemy => currentTarget != null;

    protected bool canAttack = true;
    protected bool dead = false;
    protected float waitBetweenAttack;

    public void Setup(Team team, Node spawnNode)
    {
        myTeam = team;
        this.currentNode = spawnNode;
        transform.position = currentNode.worldPosition;
        currentNode.SetOccupied(true);

        healthbar = Instantiate(barPrefab, this.transform);
        healthbar.Setup(this.transform, baseHealth);
    }
    protected void FindTarget()
    {
        var allEnemies = GameManager.Instance.GetUnitsAgainst(myTeam);
        float minDistance = Mathf.Infinity;
        BaseUnit entity = null;
        foreach (BaseUnit e in allEnemies)
        {
            if (Vector3.Distance(e.transform.position, this.transform.position) <= minDistance)
            {
                minDistance = Vector3.Distance(e.transform.position, this.transform.position);
                entity = e;
            }
        }

        currentTarget = entity;
    }

    protected void GetInRange()
    {
        if (currentTarget == null)
            return;

        if (!moving)
        {
            destination = null;
            List<Node> candidates = GridManager.Instance.GetNodesCloseTo(currentTarget.currentNode);
            candidates = candidates.OrderBy(x => Vector3.Distance(x.worldPosition, this.transform.position)).ToList();
            for (int i = 0; i < candidates.Count; i++)
            {
                if (!candidates[i].IsOccupied)
                {
                    destination = candidates[i];
                    break;
                }
            }
            if (destination == null)
                return;

            var path = GridManager.Instance.GetPath(currentNode, destination);
            if (path == null && path.Count >= 1)
                return;

            if (path[1].IsOccupied)
                return;

            path[1].SetOccupied(true);
            destination = path[1];
        }

        moving = !MoveTowards();

        if (!moving)
        {
            currentNode.SetOccupied(false);
            currentNode = destination;
        }
    }

    public void TakeDamage(int amount)
    {
        baseHealth -= amount;
        healthbar.UpdateBar(baseHealth);

        if (baseHealth <= 0 && !dead)
        {
            dead = true;
            currentNode.SetOccupied(false);
            GameManager.Instance.UnitDead(this);
        }
    }
    protected virtual void Attack()
    {
        if (!canAttack)
            return;

        animator.SetTrigger("attack");

        waitBetweenAttack = 1 / attackSpeed;
        StartCoroutine(WaitCoroutine());
    }

    IEnumerator WaitCoroutine()
    {
        canAttack = false;
        yield return null;
        animator.ResetTrigger("attack");
        yield return new WaitForSeconds(waitBetweenAttack);
        canAttack = true;
    }



    protected bool MoveTowards()
    {
        Vector3 direction = destination.worldPosition - this.transform.position;
        if (direction.sqrMagnitude <= 0.005f)
        {
            transform.position = destination.worldPosition;
            return true;
        }
        this.transform.position += direction.normalized * movementSpeed * Time.deltaTime;
        return false;
    }
}
