using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [System.Serializable]
    public class AnimatorMod
    {

        #region Variables

        public string keyName;
        public AnimatorParamType paramType;
        public bool boolVal;
        public int intVal;
        public float floatVal;
        public AnimatorTriggerType triggerVal;

        #endregion

        #region Public Methods

        public void ApplyMod(Animator animator)
        {
            if (animator == null) return;

            switch (paramType)
            {
                case AnimatorParamType.Bool:
                    animator.SetBool(keyName, boolVal);
                    break;
                case AnimatorParamType.Float:
                    animator.SetFloat(keyName, floatVal);
                    break;
                case AnimatorParamType.Int:
                    animator.SetInteger(keyName, intVal);
                    break;
                case AnimatorParamType.Trigger:
                    if (triggerVal == AnimatorTriggerType.Reset)
                    {
                        animator.ResetTrigger(keyName);
                    }
                    else
                    {
                        animator.SetTrigger(keyName);
                    }
                    break;
            }
        }

        public bool CheckMod(Animator animator)
        {
            if (animator == null) return false;

            switch (paramType)
            {
                case AnimatorParamType.Bool:
                    return animator.GetBool(keyName) == boolVal;
                case AnimatorParamType.Float:
                    return animator.GetFloat(keyName) == floatVal;
                case AnimatorParamType.Int:
                    return animator.GetInteger(keyName) == intVal;
            }

            return false;
        }

        #endregion

    }
}