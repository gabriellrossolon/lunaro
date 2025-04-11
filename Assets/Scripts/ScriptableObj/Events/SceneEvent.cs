using UnityEngine;
using System;

[CreateAssetMenu(menuName = "SceneEvent/Event")]
public class SceneEvent : ScriptableObject
{
    public Action OnEventTriggered;

    public void TriggerEvent()
    {
        OnEventTriggered?.Invoke();
        Debug.Log("Triggou um Evento");
    }
}