using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEventListener : MonoBehaviour
{
    [SerializeField] JUFootPlacement footIK;

    public void Air()
    {
        footIK.SmoothBodyPlacementTransition = false;
        footIK.SmoothIKTransition = false;
    }

    public void Land()
    {
        footIK.SmoothBodyPlacementTransition = true;
        footIK.SmoothIKTransition = true;
    }

    public void Sprint()
    {
        footIK.EnableDynamicBodyPlacing = false;
    }

    public void SprintStop()
    {
        footIK.EnableDynamicBodyPlacing = true;
    }
}
