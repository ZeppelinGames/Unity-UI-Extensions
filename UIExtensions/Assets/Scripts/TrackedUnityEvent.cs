using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TrackedUnityEvent : MonoBehaviour {
    public UnityEvent trackedEvent;
    private int listenersCount;

    private List<UnityAction> actions = new List<UnityAction>();

    public int getListenersCount() {
        return this.listenersCount;
    }

    public void AddListener(UnityAction call) {
        Debug.Log("Added");
        actions.Add(call);
        trackedEvent.AddListener(call);
        listenersCount++;
    }

    public void RemoveListener(UnityAction call) {
        Debug.Log("removed");
        actions.Remove(call);
        trackedEvent.RemoveListener(call);
        listenersCount--;
    }
}
