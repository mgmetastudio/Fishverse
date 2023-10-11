using UnityEngine;

namespace NullSave.GDTK
{
    public class LabelParentResizer : MonoBehaviour
    {

        #region Fields

        public Label label;
        public Vector2 padding;

        private RectTransform rt, rtLabel;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            rt = GetComponent<RectTransform>();
            if (label == null) enabled = false;
            rtLabel = label.GetComponent<RectTransform>();
            label.onResized.AddListener(Resize);
        }

        #endregion

        #region Private Methods

        private void Resize()
        {
            float width = Mathf.Max(rtLabel.rect.width, rtLabel.sizeDelta.x);
            float height = Mathf.Max(rtLabel.rect.height, rtLabel.sizeDelta.y);
            Vector2 sizeTo = new Vector2(width + padding.x, height + padding.y);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeTo.y);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeTo.x);
        }

        #endregion

    }
}
