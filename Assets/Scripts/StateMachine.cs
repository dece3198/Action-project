using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T1,T2> where T2 : MonoBehaviour
{
    private T2 player;
    private BaseState<T2> curState;
    private Dictionary<T1, BaseState<T2>> states;

    public void Reset(T2 player)
    {
        this.player = player;
        curState = null;
        states = new Dictionary<T1, BaseState<T2>>();
    }

    public void Update()
    {
        curState.Update(player);
    }

    public void AddState(T1 state, BaseState<T2> baseState)
    {
        states.Add(state, baseState);
    }

    public void ChangeState(T1 state)
    {
        if(curState != null)
        {
            curState.Exit(player);
        }
        curState = states[state];
        curState.Enter(player);
    }
}
