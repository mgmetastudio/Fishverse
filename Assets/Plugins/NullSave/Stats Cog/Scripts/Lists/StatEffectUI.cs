using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Stats
{
    public class StatEffectUI : MonoBehaviour
    {

        #region Variables

        public Image icon;
        public TextMeshProUGUI displayName, description, category, remainingTime;

        private StatEffect effect;

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (remainingTime != null && effect != null && effect.hasLifeSpan)
            {
                remainingTime.text = Mathf.CeilToInt(effect.RemainingTime) + "sec";
                if (effect.RemainingTime <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }

        #endregion

        #region Public Methods

        public void SetEffect(StatEffect statEffect)
        {
            effect = statEffect;
            if(icon != null) icon.sprite = effect.sprite;
            if(displayName != null) displayName.text = effect.displayName;
            if(description != null) description.text = effect.description;
            if(category != null) category.text = effect.category;
            if (remainingTime != null)
            {
                if (effect.hasLifeSpan)
                {
                    remainingTime.text = Mathf.CeilToInt(effect.RemainingTime) + "sec";
                }
                else
                {
                    remainingTime.text = string.Empty;
                }
            }

        }

        #endregion

    }
}