using System;
using System.Collections.Generic;
using UnityEngine;

public class GamePhasesManager : MonoBehaviour
{
	private GamePhaseBase[] _gamePhases;
	private GamePhaseBase _currentPhase = null;

	protected void Awake()
	{
		_gamePhases = GetComponentsInChildren<GamePhaseBase>();

		for (int i = 0; i < _gamePhases.Length; i++)
		{
			_gamePhases[i].Initialize(this);
		}
	}

	protected void Start()
	{
		SetPhase(0);
	}

	protected void OnDestroy()
	{
		EndCurrentPhase();
		for (int i = 0; i < _gamePhases.Length; i++)
		{
			_gamePhases[i].Deinitialize();
		}
	}

	public int GetCurrentPhaseIndex()
	{
		return Array.IndexOf(_gamePhases, _currentPhase);
	}

	public int GetPhasesCount()
	{
		return _gamePhases.Length;
	}

	public void SetToNextPhase()
	{
		int index = GetCurrentPhaseIndex();
		SetPhase((index + 1) % _gamePhases.Length);
	}

	public void SetPhase(string name)
	{
		for (int i = 0; i < _gamePhases.Length; i++)
		{
			GamePhaseBase phase = _gamePhases[i];
			if (phase.name == name)
			{
				SetPhase(phase);
				return;
			}
		}

		throw new Exception($"No Phase found with name {name}");
	}

	public void SetPhase(int index)
	{
		SetPhase(_gamePhases[index]);
	}

	public void SetPhase(GamePhaseBase gamePhase)
	{
		EndCurrentPhase();
		_currentPhase = gamePhase;
		if(_currentPhase != null)
		{
			_currentPhase.Enter();
		}
	}

	private void EndCurrentPhase()
	{
		if(_currentPhase != null)
		{
			_currentPhase.Exit();
			_currentPhase = null;
		}
	}
}