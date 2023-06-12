﻿using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(AttachmentItemGrid))]
    public class AttachmentItemGridEditor : TOCKEditorV2
    {

        #region Enumerations

        private enum DisplayFlags
        {
            None = 0,
            Behaviour = 1,
            Details = 2,
            Category = 4,
            Tag = 8,
            Navigation = 16,
            Events = 32,
            UI = 64,
        }

        #endregion

        #region Variables

        private DisplayFlags displayFlags;
        private Texture2D behaviourIcon, detailsIcon, categoryIcon, tagIcon, navIcon, eventsIcon, uiIcon;

        #endregion

        #region Properties

        private Texture2D BehaviourIcon
        {
            get
            {
                if (behaviourIcon == null)
                {
                    behaviourIcon = (Texture2D)Resources.Load("Icons/tock-behaviour", typeof(Texture2D));
                }

                return behaviourIcon;
            }
        }

        private Texture2D DetailsIcon
        {
            get
            {
                if (detailsIcon == null)
                {
                    detailsIcon = (Texture2D)Resources.Load("Icons/detailsUI", typeof(Texture2D));
                }

                return detailsIcon;
            }
        }

        private Texture2D CategoryIcon
        {
            get
            {
                if (categoryIcon == null)
                {
                    categoryIcon = (Texture2D)Resources.Load("Icons/category", typeof(Texture2D));
                }

                return categoryIcon;
            }
        }

        private Texture2D TagIcon
        {
            get
            {
                if (tagIcon == null)
                {
                    tagIcon = (Texture2D)Resources.Load("Icons/tock-tag", typeof(Texture2D));
                }

                return tagIcon;
            }
        }

        private Texture2D NavIcon
        {
            get
            {
                if (navIcon == null)
                {
                    navIcon = (Texture2D)Resources.Load("Icons/tock-navigation", typeof(Texture2D));
                }

                return navIcon;
            }
        }

        private Texture2D EventsIcon
        {
            get
            {
                if (eventsIcon == null)
                {
                    eventsIcon = (Texture2D)Resources.Load("Icons/tock-event", typeof(Texture2D));
                }

                return eventsIcon;
            }
        }

        private Texture2D UIIcon
        {
            get
            {
                if (uiIcon == null)
                {
                    uiIcon = (Texture2D)Resources.Load("Icons/tock-ui", typeof(Texture2D));
                }

                return uiIcon;
            }
        }

        #endregion

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            displayFlags = (DisplayFlags)serializedObject.FindProperty("z_display_flags").intValue;
            MainContainerBegin("Attachment Item Grid", "Icons/item-grid");

            if (SectionToggle(DisplayFlags.Behaviour, "Behaviour", BehaviourIcon))
            {
                SimpleProperty("itemUIPrefab");
            }

            if (SectionToggle(DisplayFlags.UI, "UI", UIIcon))
            {
                SimpleProperty("fillContainer");

                SectionHeader("When Input is Locked");
                SimpleProperty("hideSelectionWhenLocked", "Hide Select Indicator");
            }

            if (SectionToggle(DisplayFlags.Navigation, "Navigation", NavIcon))
            {
                SimpleProperty("lockInput");
                SimpleProperty("navigationMode");
                switch ((NavigationType)serializedObject.FindProperty("navigationMode").intValue)
                {
                    case NavigationType.ByButton:
                        SimpleProperty("inputHorizontal", "Horizontal");
                        SimpleProperty("inputVertical", "Vertical");
                        break;
                    case NavigationType.ByKey:
                        SimpleProperty("keyLeft", "Left");
                        SimpleProperty("keyRight", "Right");
                        SimpleProperty("keyUp", "Up");
                        SimpleProperty("keyDown", "Down");
                        break;
                }

                SimpleProperty("autoRepeat");
                if (serializedObject.FindProperty("autoRepeat").boolValue)
                {
                    SimpleProperty("repeatDelay");
                }

                SimpleProperty("selectionMode");
                switch ((NavigationType)serializedObject.FindProperty("selectionMode").intValue)
                {
                    case NavigationType.ByButton:
                        SimpleProperty("buttonSubmit");
                        break;
                    case NavigationType.ByKey:
                        SimpleProperty("keySubmit");
                        break;
                }
            }

            if (SectionToggle(DisplayFlags.Events, "Events", EventsIcon))
            {
                SimpleProperty("onSelectionChanged");
                SimpleProperty("onInputLocked");
                SimpleProperty("onInputUnlocked");
            }

            MainContainerEnd();
        }

        #endregion

        #region Private Methods

        private bool SectionToggle(DisplayFlags flag, string title, Texture2D icon)
        {
            bool hasFlag = (displayFlags & flag) == flag;
            bool result = SectionGroup(title, icon, hasFlag);

            if (result != hasFlag)
            {
                displayFlags = result ? displayFlags | flag : displayFlags & ~flag;
                serializedObject.FindProperty("z_display_flags").intValue = (int)displayFlags;
            }

            return hasFlag;
        }

        #endregion

    }
}