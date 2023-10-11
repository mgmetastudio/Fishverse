#if GDTK
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NullSave.GDTK.Stats
{
    public class statsInfoUI : MonoBehaviour, IPointerClickHandler
    {

        #region Fields

        [Tooltip("Image used to display Sprite")] public Image image;
        [Tooltip("Apply color to image")] public bool colorize;
        [Tooltip("List of labels and formats for displaying information")] public List<TemplatedLabel> labels;
        [SerializeField] [Tooltip("Show/Hide help in editor")] private bool showHelp;

        public UnityEvent onClick;

        #endregion

        #region Properties

        public BasicInfo info { get; set; }

        #endregion

        #region Unity Methods

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }

        #endregion

        #region Private Methods

        internal void ApplyImage()
        {
            if (image)
            {
                if(info == null)
                {
                    image.sprite = null;
                    image.enabled = false;
                    return;
                }

                image.sprite = info.image.GetImage();
                image.enabled = image.sprite != null;
                if (colorize)
                {
                    image.color = info.color;
                }
            }
        }

        internal string FormatInfo(string format)
        {
            if (info == null)
            {
                return format.Replace("{id}", string.Empty)
                    .Replace("{abbr}", string.Empty)
                    .Replace("{description}", string.Empty)
                    .Replace("{groupName}", string.Empty)
                    .Replace("{title}", string.Empty)
                    ;
            }

            return format.Replace("{id}", info.id)
                .Replace("{abbr}", info.abbr)
                .Replace("{description}", info.description)
                .Replace("{groupName}", info.groupName)
                .Replace("{title}", info.title)
                ;
        }

        #endregion

    }
}
#endif