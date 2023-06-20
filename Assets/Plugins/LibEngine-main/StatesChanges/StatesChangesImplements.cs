using LibEngine.Extensions;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.TMP_InputField;

namespace LibsEngine.States.Implements
{
    [System.Serializable]
    public class ImageChangeSprites<T> : BaseStatesChange<T, Sprite>
    {
        [SerializeField] private Image currentImage;

        protected override void StateChangeImplement(Sprite typeSet)
        {
            base.StateChangeImplement(typeSet);
            currentImage.sprite = typeSet;
        }
    }

    [System.Serializable]
    public class GameObjectChangeActive<Tstate> : BaseStatesChange<Tstate, GameObject>
    {
        [SerializeField]
        private bool isActiveState = true; //for optional revert in serialize

        protected override void StateChangeImplement(GameObject typeSet)
        {
            base.StateChangeImplement(typeSet);
            if (typeSet != null)
                typeSet.SetActive(isActiveState);
        }

        public override void ResetState(GameObject type)
        {
            base.ResetState(type);
            if(type!=null)
                type.SetActive(!isActiveState);
        }

        public override void ResetAllStates()
        {
            base.ResetAllStates();
            lastState = default;
            //lastType = (object)Convert.ToInt32(-1) as Tstate; //costil //Todo
        }
    }

    [System.Serializable]
    public class ImageChangeColors<T> : BaseStatesChange<T, Color>
    {
        [SerializeField] private Image currentImage;

        protected override void StateChangeImplement(Color typeSet)
        {
            base.StateChangeImplement(typeSet);
            currentImage.color = typeSet;
        }
    }

    [System.Serializable]
    public class ImageChangeMaterials<T> : BaseStatesChange<T, Material>
    {
        [SerializeField] private Image currentImage;

        protected override void StateChangeImplement(Material typeSet)
        {
            base.StateChangeImplement(typeSet);
            currentImage.material = typeSet;
        }
    }

    [System.Serializable]
    public class GroupsHideStates : ChangeStatesImplementation<bool>
    {
        [SerializeField] private Button[] _buttonsToHideModeView;

        public override void SetState(bool setState, bool isForcibly = false)
        {
            base.SetState(setState, isForcibly);
            _buttonsToHideModeView.ForEach(item => item.gameObject.SetActive(setState));
        }
    }

    [System.Serializable]
    public class ButtonChangeInteraction<T> : BaseStatesChange<T, bool>
    {
        [SerializeField] private Button currentButton;

        protected override void StateChangeImplement(bool typeSet)
        {
            base.StateChangeImplement(typeSet);
            currentButton.interactable = typeSet;
        }
    }

    [System.Serializable]
    public class GameObjectsTurningSingle<T> : BaseStatesChange<T, bool>
    {
        [SerializeField] private GameObject currentButton;

        protected override void StateChangeImplement(bool typeSet)
        {
            base.StateChangeImplement(typeSet);
            currentButton.SetActive(typeSet);
        }
    }

    [System.Serializable]
    public class CanvasGroupAlpaChangeState<T> : BaseStatesChange<T, float>
    {
        [SerializeField] private CanvasGroup canvasGroup;

        protected override void StateChangeImplement(float typeSet)
        {
            base.StateChangeImplement(typeSet);
            canvasGroup.alpha = typeSet;
        }
    }

    [System.Serializable]
    public class ChangeFontMaterialPresset<T> : BaseStatesChange<T, Material>
    {
        [SerializeField]
        private TMP_Text currentText;

        protected override void StateChangeImplement(Material typeSet)
        {
            base.StateChangeImplement(typeSet);
            currentText.fontSharedMaterial = typeSet;
        }
    }

    [System.Serializable]
    public class ChangeFontTMProColor<T> : BaseStatesChange<T, Color>
    {
        [SerializeField]
        private TMP_Text currentText;

        protected override void StateChangeImplement(Color typeSet)
        {
            base.StateChangeImplement(typeSet);
            currentText.color = typeSet;
        }
    }

    [System.Serializable]
    public class ChangeStatesImplementation<T> : BaseStatesChange<T, bool>
    {
        [Space]
        [SerializeField] private bool isChangeImageSprites = false;
        [ShowIf("isChangeImageSprites")]
        [SerializeField] private ImageChangeSprites<bool> imageChangesSprites;

        [Space]
        [SerializeField] private bool isChangeImageColors = false;
        [SerializeField] [ShowIf("isChangeImageColors")] private ImageChangeColors<bool> imageChangesColors;

        [Space]
        [SerializeField] private bool isChangeButtonInteraction;
        [SerializeField] [ShowIf("isChangeButtonInteraction")] private ButtonChangeInteraction<bool> buttonChange;

        [Space]
        [SerializeField] private bool isGameobjectTurnSingle = false;
        [SerializeField] [ShowIf("isGameobjectTurnSingle")] private GameObjectsTurningSingle<bool> gameObjectTurnSingle;

        protected override void StateChangeImplement(bool typeSet)
        {
            base.StateChangeImplement(typeSet);

            if (isChangeImageSprites)
                imageChangesSprites.SetState(typeSet);

            if (isChangeImageColors)
                imageChangesColors.SetState(typeSet);

            if (isChangeButtonInteraction)
                buttonChange.SetState(typeSet);

            if (isGameobjectTurnSingle)
                gameObjectTurnSingle.SetState(typeSet);
        }
    }

    [System.Serializable]
    public class ChangeStatesImplementatCollection<T> : BaseStatesChange<T, T>
    {
        [Space]
        [SerializeField] private bool isChangeImageSprites = false;
        [ShowIf("isChangeImageSprites")]
        [SerializeField] private List<ImageChangeSprites<T>> imageChangesSprites;

        [Space]
        [SerializeField] private bool isChangeImageColors = false;
        [SerializeField]
        [ShowIf("isChangeImageColors")]
        private List<ImageChangeColors<T>> imageChangesColors;

        [Space]
        [SerializeField] private bool isChangeButtonInteraction;
        [SerializeField]
        [ShowIf("isChangeButtonInteraction")]
        private List<ButtonChangeInteraction<T>> buttonChange;

        [Space]
        [SerializeField] private bool isGameobjectTurnSingle = false;
        [SerializeField]
        [ShowIf("isGameobjectTurnSingle")]
        private List<GameObjectsTurningSingle<T>> gameObjectTurnSingle;

        protected override void StateChangeImplement(T typeSet)
        {
            base.StateChangeImplement(typeSet);

            if (isChangeImageSprites)
                foreach (var item in imageChangesSprites)
                    item.SetState(typeSet);

            if (isChangeImageColors)
                foreach (var item in imageChangesColors)
                    item.SetState(typeSet);

            if (isChangeButtonInteraction)
                foreach (var item in buttonChange)
                    item.SetState(typeSet);

            if (isGameobjectTurnSingle)
                foreach (var item in gameObjectTurnSingle)
                    item.SetState(typeSet);
        }
    }

    [System.Serializable]
    public class ChangeInputFieldHiden : BaseStatesChange<bool, bool>
    {
        [Space]
        [SerializeField] private TMP_InputField _inputField;

        protected override void StateChangeImplement(bool typeSet)
        {
            base.StateChangeImplement(typeSet);
            ContentType setContentType = ContentType.Standard;
            if (typeSet)
                setContentType = ContentType.Password;

            _inputField.contentType = setContentType;

            _inputField.ForceLabelUpdate();
        }
    }

    public static class ChangeStatesExtensions
    {
        public static void SetInvertedState(this IStateables<bool> stateablesBool)
        {
            var state = stateablesBool.GetState();
            stateablesBool.SetState(!state);
        }
    }
}