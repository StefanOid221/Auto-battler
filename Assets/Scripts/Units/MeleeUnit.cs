using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : BaseUnit
{

    protected override void Update()
    {

        base.Update();

        if (GameManager.Instance.gameState == GameState.Fight) 
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
    }
}