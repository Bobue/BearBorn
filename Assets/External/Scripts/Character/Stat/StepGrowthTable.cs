using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Growth/Step Growth Table")]
public class StepGrowthTable : ScriptableObject
{
    public List<GrowthStep> steps;

    public GrowthStep GetStep(float level)
    {
        foreach (var step in steps)
        {
            if (level >= step.startLevel && level <= step.endLevel)
            {
                return step;
            }
            
        }
        return null;
    }
}
