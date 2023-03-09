using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : BaseUnit
{
   
    public void Update()
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