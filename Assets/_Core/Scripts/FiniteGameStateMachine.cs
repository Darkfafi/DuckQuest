using System;
using UnityEngine;

public class FiniteGameStateMachine<Parent> : IDisposable
	where Parent : MonoBehaviour
{
	public readonly Parent StatesHolder;
	public readonly GameStateBase<Parent>[] States;

	public GameStateBase<Parent> CurrentState
	{
		get; private set;
	}

	public bool IsRunning => CurrentState != null;

	public bool IsTempState
	{
		get; private set;
	}

	private GameStateBase<Parent> _nextState = null;

	public FiniteGameStateMachine(Parent parent, GameStateBase<Parent>[] states, bool startStateMachine = true)
	{
		StatesHolder = parent;
		States = states;

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

	public void Dispose()
	{
		EndCurrentState();
		for (int i = 0; i < States.Length; i++)
		{
			States[i].Deinitialize();
		}
	}

	public int GetCurrentPhaseIndex()
	{
		return Array.IndexOf(States, CurrentState);
	}

	public int GetPhasesCount()
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
			int index = GetCurrentPhaseIndex();
			SetState((index + 1) % States.Length);
		}
	}

	public void SetState(string name)
	{
		for (int i = 0; i < States.Length; i++)
		{
			GameStateBase<Parent> state = States[i];
			if (state.name == name)
			{
				SetState(state);
				return;
			}
		}

		throw new Exception($"No Phase found with name {name}");
	}

	public void SetState(int index)
	{
		SetState(States[index]);
	}

	public void SetState(GameStateBase<Parent> state, GameStateBase<Parent> nextState = null)
	{
		if(state.StateHolder != StatesHolder)
		{
			throw new Exception($"State {state} belongs to {state.StateHolder}, and can't be used by {StatesHolder}");
		}

		EndCurrentState();

		IsTempState = false;
		_nextState = nextState;

		if (!state.IsInitialized)
		{
			IsTempState = true;
			state.Initialize(StatesHolder);
		}

		CurrentState = state;
		if (CurrentState != null)
		{
			CurrentState.Enter();
		}
	}

	private void EndCurrentState()
	{
		if (CurrentState != null)
		{
			CurrentState.Exit();

			if (IsTempState)
			{
				CurrentState.Deinitialize();
				IsTempState = false;
			}

			CurrentState = null;
			_nextState = null;
		}
	}
}