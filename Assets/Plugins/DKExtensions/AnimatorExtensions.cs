using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimatorExtensions
{
    public static bool IsParameter(this Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }

    public static AnimatorControllerParameter HasParameter(this Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
            if (param.name == paramName) 
                return param;

        return null;
    }

    public static AnimatorControllerParameterType? ParameterType(this Animator animator, string paramName)
    {
        var param = animator.HasParameter(paramName);
        if(param != null)
            return param.type;
        return null;
    }
}
