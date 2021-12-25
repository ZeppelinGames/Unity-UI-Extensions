using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using System;

public enum ExtendedButtonEventTypes { OnClick, OnDeselect, OnHover, OnExitHover, OnToggleOn, OnToggleOff }

public class ExtendedButton : Button, IPointerDownHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

    [HideInInspector] public List<ExtendedButtonEvent> extendedButtonEvents = new List<ExtendedButtonEvent>();
    private bool toggleActive = false;

    [MenuItem("GameObject/UI/Extended Button", false, 0)]
    public static void CreateExtendedButton() {
        GameObject newExtendedButton = new GameObject("Extended button"); //Create new GameObject

        //Add needed components for UI item
        newExtendedButton.AddComponent<RectTransform>();
        newExtendedButton.AddComponent<CanvasRenderer>();
        newExtendedButton.AddComponent<Image>();
        newExtendedButton.AddComponent<ExtendedButton>();

        newExtendedButton.transform.SetParent(Selection.activeTransform, false); //Set parent to current selected object
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);
        RunEvent(ExtendedButtonEventTypes.OnHover);
    }

    public override void OnPointerExit(PointerEventData eventData) {
        base.OnPointerExit(eventData);
        RunEvent(ExtendedButtonEventTypes.OnExitHover);
    }

    public override void OnPointerClick(PointerEventData eventData) {
        base.OnPointerClick(eventData);

        if (EventSystem.current.IsPointerOverGameObject()) {
            if (EventSystem.current.currentSelectedGameObject == null) {
                RunEvent(ExtendedButtonEventTypes.OnDeselect);
            } else {
                RunEvent(ExtendedButtonEventTypes.OnClick);

                toggleActive = !toggleActive; //Invert toggle
                RunEvent(toggleActive ? ExtendedButtonEventTypes.OnToggleOn : ExtendedButtonEventTypes.OnToggleOff);
            }
        }
    }

    void RunEvent(ExtendedButtonEventTypes eventType) {
        for (int i = 0; i < extendedButtonEvents.Count; i++) {
            if (extendedButtonEvents[i].eventType == eventType) {
                extendedButtonEvents[i].RunEvent();
            }
        }
    }

    public void TestVoid() { }
    public void TestStringParam(string s) { }
    public void TestIntParam(int i) { }
    public void TestFloatParam(float f) { }
    public void TestBoolParam(bool b) { }
    public void TestStringIntParam(string s, int i) { }
}

[System.Serializable]
public class ExtendedButtonEvent {
    public ExtendedButtonEventTypes eventType = ExtendedButtonEventTypes.OnClick;
    public TrackedUnityEvent buttonEvent;

    public ExtendedButtonEvent(ExtendedButtonEventTypes eventType, TrackedUnityEvent buttonEvent) {
        this.eventType = eventType;
        this.buttonEvent = buttonEvent;
    }

    public void RunEvent() {
        if (buttonEvent != null) {
            buttonEvent.InvokeEvents();
        } else {
            Debug.LogError("Button event doesnt exist");
        }
    }
}
