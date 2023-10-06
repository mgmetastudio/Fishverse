#if GDTK
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class StatModifierUI : MonoBehaviour
    {

        #region Fields

        [Tooltip("Label used to display modifier details")] public Label details;
        [Tooltip("Format to use for modifier details")] public string detailsFormat;
        [Tooltip("Show '+' on positive numbers")] public bool showPlusSymbol;

        [Tooltip("Label used to display remaining life")] public Label remainingLife;
        [Tooltip("Format to use for remaining life")] public string remainingLifeFormat;

        private GDTKStatModifier target;

        #endregion

        #region Unity Methods

        private void OnDestroy()
        {
            if (target != null)
            {
                target.onLifeChanged -= UpdateUI;
                target.onDeactivated -= RemoveEffect;
            }
        }

        #endregion

        #region Public Methods

        public virtual void LoadStatusEffect(GDTKStatModifier modifier)
        {
            target = modifier;

            target.onLifeChanged += UpdateUI;
            target.onDeactivated += RemoveEffect;
        }

        public virtual void UpdateUI()
        {
            if (details)
            {
                details.text = detailsFormat
                    .Replace("{statId}", target.affectsStatId)
                    .Replace("{statTitle}", target.GetStat().info.title)
                    .Replace("{requirements}", target.requirements)
                    .Replace("{value}", target.GetDisplayValue(showPlusSymbol))
                    .Replace("{target}", target.target.ToString())
                    ;
            }

            if (remainingLife)
            {
                if (target.applies != ModifierApplication.RecurringOverSeconds && target.applies != ModifierApplication.RecurringOverTurns)
                {
                    remainingLife.text = string.Empty;
                }
                else
                {
                    remainingLife.text = remainingLifeFormat.Replace("{0}", Mathf.Round((float)target.lifeRemaining).ToString());
                }
            }

        }

        #endregion

        #region Private Methods

        private void RemoveEffect(GDTKStatModifier modifier)
        {
            Destroy(gameObject);
        }

        #endregion

    }
}
#endif