using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEditorInternal;

[CustomEditor(typeof(ExtendedButton), true)]
[CanEditMultipleObjects]
public class ExtendedButtonEditor : Editor {
    Editor defaultEditor;
    ExtendedButton button;

    SerializedProperty extendedButtonEventsProp;
    List<ExtendedButtonEvent> extendedButtonEvents;

    ReorderableList reorderableButtonEvents;

    void OnEnable() {
        //When this inspector is created, also create the built-in inspector
        defaultEditor = CreateEditor(targets, typeof(ButtonEditor));
        button = target as ExtendedButton;

        extendedButtonEventsProp = serializedObject.FindProperty("extendedButtonEvents");
        extendedButtonEvents = button.extendedButtonEvents;

        reorderableButtonEvents = new ReorderableList(serializedObject, extendedButtonEventsProp, true, true, true, true) {
            drawHeaderCallback = DrawEventListHeader,
            drawElementCallback = DrawEventListItems,
            elementHeightCallback = SetElementHeight
        };
    }

    void OnDisable() {
        if (defaultEditor != null) {
            MethodInfo disableMethod = defaultEditor.GetType().GetMethod("OnDisable");
            if (disableMethod != null)
                disableMethod.Invoke(defaultEditor, null);
            DestroyImmediate(defaultEditor);
        }
    }

    public override void OnInspectorGUI() {
        //defaultEditor.OnInspectorGUI();
        serializedObject.Update();

        reorderableButtonEvents.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    void DrawEventListHeader(Rect rect) {
        EditorGUI.LabelField(rect, "Button Events");
    }

    void DrawEventListItems(Rect rect, int index, bool isActive, bool isFocused) {
        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y + 4, EditorGUIUtility.currentViewWidth * 0.25f, rect.height),
            extendedButtonEventsProp.GetArrayElementAtIndex(index).FindPropertyRelative("eventType"),
            GUIContent.none);

        EditorGUI.PropertyField(
          new Rect(rect.x + EditorGUIUtility.currentViewWidth * 0.25f, rect.y + 4, EditorGUIUtility.currentViewWidth * 0.65f, rect.height),
          extendedButtonEventsProp.GetArrayElementAtIndex(index).FindPropertyRelative("buttonEvent"));
    }

    float SetElementHeight(int i) {
        if (i < 0 || i >= extendedButtonEvents.Count || extendedButtonEvents[i] == null || extendedButtonEvents[i].buttonEvent == null) { return 100; }
        return extendedButtonEvents[i].buttonEvent.getListenersCount() + 1 * 100;
    }
}
