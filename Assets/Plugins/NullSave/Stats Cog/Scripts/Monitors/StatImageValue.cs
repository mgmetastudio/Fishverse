using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Stats
{
    [RequireComponent(typeof(Image))]
    public class StatImageValue : MonoBehaviour
    {

        #region Variables

        public StatsCog statCog;
        public string valueName;
        public string minName;
        public string maxName;

        public bool includeEffects = false;

        private Image image;
        private bool isLocked;
        private StatValue value, min, max;

        #endregion

        #region Unity Methods

        public void Start()
        {
            image = GetComponent<Image>();
            Rebind();
        }

        #endregion

        #region Public Methods

        public void Rebind()
        {
            if (statCog == null) return;

            value = statCog.FindStat(valueName);
            if (value == null)
            {
                Debug.LogError(name + ".StatSlider could not find stat '" + valueName + "'");
                enabled = false;
                return;
            }
            value.onValueChanged.AddListener(UpdateStat);

            min = statCog.FindStat(minName);
            if (min)
            {
                min.onMinValueChanged.AddListener(UpdateStat);
            }

            max = statCog.FindStat(maxName);
            if (max)
            {
                max.onMaxValueChanged.AddListener(UpdateStat);
            }

            UpdateStat(0, 0);
        }

        #endregion

        #region Private Methods

        private void UpdateStat(float oldVal, float newVal)
        {
            if (value == null || image == null) return;
            if (isLocked) return;

            float minVal, maxVal;
            isLocked = true;
            if (includeEffects)
            {
                minVal = min != null ? min.CurrentValue : value.CurrentMinimum;
                maxVal = min != null ? max.CurrentValue : value.CurrentMaximum;
                image.fillAmount = (value.CurrentValue - minVal) / (maxVal - minVal);
            }
            else
            {
                minVal = min != null ? min.CurrentBaseValue : value.CurrentBaseMinimum;
                maxVal = min != null ? max.CurrentBaseValue : value.CurrentBaseMaximum;
                image.fillAmount = (value.CurrentBaseValue - minVal) / (maxVal - minVal);
            }
            isLocked = false;
        }

        #endregion

    }
}