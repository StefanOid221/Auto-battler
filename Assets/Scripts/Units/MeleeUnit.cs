using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : BaseUnit
{

    protected override void Update()
    {

        base.Update();

        if (GameManager.Instance.gameState == GameState.Fight && this.isBenched == false)
        {
            if (!hasEnemy)
            {
                FindTarget();
            }

            if (inRange && !moving)
            {
                //In range for attack!
                if (canAttack)
                {
                    Attack();
                    currentTarget.TakeDamage(baseDamage);
                }
            }
            else
            {
                GetInRange();
            }
        }
        else if (GameManager.Instance.gameState == GameState.Decision)
            currentTarget = null;
    }
}