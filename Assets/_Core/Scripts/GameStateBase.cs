using UnityEngine;

public abstract class GameStateBase<Parent> : MonoBehaviour
	where Parent : MonoBehaviour
{
	public Parent StateHolder
	{
		get; private set;
	}
	public bool IsCurrentState
	{
		get; private set;
	}

	public bool IsInitialized => StateHolder != null;

	public virtual void Initialize(Parent parent)
	{
		StateHolder = parent;
	}

	public virtual void Deinitialize()
	{
		StateHolder = null;
	}

	public void Enter()
	{
		IsCurrentState = true;
		OnEnter();
	}

	public void Exit()
	{
		OnExit();
		IsCurrentState = false;
	}

	protected abstract void OnEnter();
	protected abstract void OnExit();
}