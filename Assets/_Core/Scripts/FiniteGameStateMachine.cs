using System;
using UnityEngine;

public class FiniteGameStateMachine<Parent> : IDisposable
	where Parent : MonoBehaviour
{
	public delegate void StateSwitchHandler(GameStateBase<Parent> newState, GameStateBase<Parent> prevState);
	public event StateSwitchHandler StateSwitchedEvent;

	public readonly Parent StatesHolder;
	public readonly GameStateBase<Parent>[] States;

	public GameStateBase<Parent> CurrentState
	{
		get; private set;
	}

	public bool IsRunning => CurrentState != null;

	public bool IsLooping
	{
		get; private set;
	}

	private bool _isTempState = false;
	private GameStateBase<Parent> _nextState = null;

	public FiniteGameStateMachine(Parent parent, GameStateBase<Parent>[] states, bool isLoopingStateMachine, bool startStateMachine = true)
	{
		StatesHolder = parent;
		States = states;
		IsLooping = isLoopingStateMachine;

		for (int i = 0; i < states.Length; i++)
		{
			states[i].Initialize(StatesHolder);
		}

		if(startStateMachine)
		{
			StartStateMachine();
		}
	}

	public void StartStateMachine()
	{
		if(!IsRunning)
		{
			SetState(0);
		}
	}

	public void StopStateMachine()
	{
		if(IsRunning)
		{
			SetState(null);
		}
	}

	public void Dispose()
	{
		EndCurrentState();
		for (int i = 0; i < States.Length; i++)
		{
			States[i].Deinitialize();
		}
	}

	public int GetCurrentStateIndex()
	{
		return Array.IndexOf(States, CurrentState);
	}

	public int GetStatesCount()
	{
		return States.Length;
	}

	public void GoToNextState()
	{
		if (_nextState != null)
		{
			GameStateBase<Parent> nextState = _nextState;
			_nextState = null;
			SetState(nextState);
		}
		else
		{
			int index = GetCurrentStateIndex();
			if (IsLooping)
			{
				SetState((index + 1) % States.Length);
			}
			else
			{
				if (index == States.Length - 1)
				{
					StopStateMachine();
				}
				else
				{
					SetState(index + 1);
				}
			}
		}
	}

	public void SetState(int index)
	{
		SetState(States[index]);
	}

	public void SetState(GameStateBase<Parent> state, GameStateBase<Parent> nextState = null)
	{
		GameStateBase<Parent> preState = CurrentState;

		EndCurrentState();

		_isTempState = false;
		_nextState = nextState;

		if (state != null)
		{
			if (state.StateHolder != StatesHolder)
			{
				throw new Exception($"State {state} belongs to {state.StateHolder}, and can't be used by {StatesHolder}");
			}

			if (!state.IsInitialized)
			{
				_isTempState = true;
				state.Initialize(StatesHolder);
			}
		}

		CurrentState = state;

		if (CurrentState != null)
		{
			CurrentState.Enter();
		}

		StateSwitchedEvent?.Invoke(CurrentState, preState);
	}

	private void EndCurrentState()
	{
		if (CurrentState != null)
		{
			CurrentState.Exit();

			if (_isTempState)
			{
				CurrentState.Deinitialize();
				_isTempState = false;
			}

			CurrentState = null;
			_nextState = null;
		}
	}
}