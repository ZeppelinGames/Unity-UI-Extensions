using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TrackedUnityEvent : MonoBehaviour {
    [SerializeField] private int listenersCount;

    [SerializeField] private List<UnityAction> actions = new List<UnityAction>();
    public Object actionObject = null;

    public TrackedUnityEvent() {
        actionObject = null;
    }

    public int getListenersCount() {
        return this.listenersCount;
    }

    public void AddListener(UnityAction call) {
        actions.Add(call);
       // actionObjects.Add((Object)call.Target);

        listenersCount++;
    }

    public void RemoveListener(UnityAction call) {
        actions.Remove(call);
       // actionObjects.Remove((Object)call.Target);

        listenersCount--;
    }

    public void InvokeEvents() {
        for (int i = 0; i < actions.Count; i++) { actions[i].Invoke(); }
    }
}
