using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TrackedUnityEvent {
    [SerializeField] private int listenersCount = 1;

    [SerializeField] private List<Action> actions = new List<Action>();
    [SerializeReference, SerializeField] private List<object> actionObjects = new List<object>();

    [SerializeField] private List<int> selectedCompIndex = new List<int>();
    [SerializeField] private List<int> selectedFuncIndex = new List<int>();

    public TrackedUnityEvent() {
        listenersCount = 1;

        actions.Add(null);
        actionObjects.Add(null);

        selectedCompIndex.Add(0);
        selectedFuncIndex.Add(0);
    }

    public int getListenersCount() {
        return this.listenersCount;
    }

    public void AddListener(Action call) {
        actions.Add(call);
        actionObjects.Add(call.Target);

        selectedCompIndex.Add(0);
        selectedFuncIndex.Add(0);

        listenersCount++;
    }

    public void RemoveListener(Action call) {
        int removeIndex = actions.IndexOf(call);

        actions.RemoveAt(removeIndex);
        actionObjects.RemoveAt(removeIndex);

        selectedCompIndex.RemoveAt(removeIndex);
        selectedFuncIndex.RemoveAt(removeIndex);

        listenersCount--;
    }

    public void InvokeEvents() {
        for (int i = 0; i < actions.Count; i++) { actions[i].Invoke(); }
    }
}
