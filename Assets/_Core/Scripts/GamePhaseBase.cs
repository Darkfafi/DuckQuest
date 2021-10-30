using UnityEngine;

public abstract class GamePhaseBase : MonoBehaviour
{
	public bool IsCurrentPhase
	{
		get; private set;
	}

	protected GamePhasesManager GamePhasesManager
	{
		get; private set;
	}

	public virtual void Initialize(GamePhasesManager manager)
	{
		GamePhasesManager = manager;
	}

	public virtual void Deinitialize()
	{
		GamePhasesManager = null;
	}

	public void Enter()
	{
		IsCurrentPhase = true;
		OnEnter();
	}

	public void Exit()
	{
		OnExit();
		IsCurrentPhase = false;
	}

	protected abstract void OnEnter();
	protected abstract void OnExit();
}
