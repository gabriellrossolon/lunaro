using System.Collections.Generic;
using System;
using UnityEngine;

// A interface que os estados vão implementar
public interface IState
{
    void OnEnter();
    void OnExit();
    void OnTick();
    void OnFixedTick();
}

// A StateMachine em si, que vai ser responsável por manter uma lista de estados e fazer as transições
public class StateMachine
{
    public Dictionary<string, IState> States = new(); // Dicionário de estados
    public IState CurrentState { get; private set; } // Estado atual
    public IState PreviousState { get; private set; } // Estado anterior (novo)

    public virtual void Tick() => CurrentState?.OnTick();
    public virtual void FixedTick() => CurrentState?.OnFixedTick();

    public void AddState(string key, IState state)
    {
        if (States.ContainsKey(key))
        {
            Debug.LogWarning($"State already found on the dictionary: {key}");
            return;
        }

        States.Add(key, state);
    }

    public void SetState(string key)
    {
        if (States.TryGetValue(key, out IState newState))
        {
            // Salva o estado atual como o estado anterior antes de fazer a transição
            PreviousState = CurrentState;

            // Sai do estado atual
            CurrentState?.OnExit();

            // Define o novo estado e chama o OnEnter
            CurrentState = newState;
            CurrentState.OnEnter();
        }
        else
        {
            throw new ArgumentException($"Invalid state key: {key}");
        }
    }

    public void SetInitialState(string key)
    {
        if (States.TryGetValue(key, out IState initialState))
        {
            CurrentState = initialState;
            PreviousState = null; // Nenhum estado anterior no início
            CurrentState.OnEnter();
        }
        else
        {
            throw new ArgumentException($"Invalid initial state key: {key}");
        }
    }
}

