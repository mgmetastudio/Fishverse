using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

public enum GraphicType
{
    OnlyTarget,
    OnlyChildren,
    BaseAndChildren
}

public class ButtonXL : Button
{
    public GraphicType targetGraphicType;

    public UnityEvent onDown = new UnityEvent();
    public UnityEvent onUp = new UnityEvent();
    public UnityEvent onEnter = new UnityEvent();
    public UnityEvent onExit = new UnityEvent();

    protected List<Graphic> graphics;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        onDown.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        onUp.Invoke();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        onEnter.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        onExit.Invoke();
    }

    protected override void Awake()
    {
        base.Awake();

        if (targetGraphicType == GraphicType.OnlyTarget)
            return;

        graphics = GetComponentsInChildren<Graphic>().ToList();

        if (targetGraphicType == GraphicType.OnlyChildren)
        {
            var graphic = GetComponent<Graphic>();
            if (graphic)
                graphics.Remove(graphic);
        }
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        if (targetGraphicType == GraphicType.OnlyTarget || transition != Transition.ColorTint)
        {
            base.DoStateTransition(state, instant);
            return;
        }

        var targetColor =
            state == SelectionState.Disabled ? colors.disabledColor :
            state == SelectionState.Highlighted ? colors.highlightedColor :
            state == SelectionState.Normal ? colors.normalColor :
            state == SelectionState.Pressed ? colors.pressedColor :
            state == SelectionState.Selected ? colors.selectedColor : Color.white;

        foreach (var graphic in graphics)
            graphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ButtonXL))]
public class ButtonXLEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        SerializedProperty targetGraphicTypeProperty = serializedObject.FindProperty("targetGraphicType");
        EditorGUILayout.PropertyField(targetGraphicTypeProperty);

        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();

        SerializedProperty onDownProperty = serializedObject.FindProperty("onDown");
        SerializedProperty onUpProperty = serializedObject.FindProperty("onUp");
        SerializedProperty onEnterProperty = serializedObject.FindProperty("onEnter");
        SerializedProperty onExitProperty = serializedObject.FindProperty("onExit");
        EditorGUILayout.PropertyField(onDownProperty);
        EditorGUILayout.PropertyField(onUpProperty);
        EditorGUILayout.PropertyField(onEnterProperty);
        EditorGUILayout.PropertyField(onExitProperty);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif