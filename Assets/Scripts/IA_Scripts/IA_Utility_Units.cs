using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_Utility_Units : MonoBehaviour
{
    // Start is called before the first frame update
    public float availableCountWeight = 0.4f;
    public float benchCountWeight = 0.3f;
    public float costWeight = 0.2f;
    public float unitStrengthWeight = 0.1f;

    // Calculate the utility value for a piece
    public float CalculateUtility(BaseUnit unit, int availableCount, int benchCount)
    {
        float utility = 0f;

        // Calculate available count contribution
        utility += availableCount * availableCountWeight;

        // Calculate bench count contribution
        utility += benchCount * benchCountWeight;

        // Calculate cost contribution
        utility -= unit.cost * costWeight;

        // Calculate unit strength contribution
        utility += (float)(unit.baseDamage * unitStrengthWeight);

        return utility;
    }
}

