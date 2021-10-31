using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	private GamePhaseBase _winPhase = null;

	[SerializeField]
	private GamePhaseBase _losePhase = null;

	private FiniteGameStateMachine<GameManager> _gamePhaseStateMachine = null;

	protected void Awake()
	{
		_gamePhaseStateMachine = new FiniteGameStateMachine<GameManager>(this, GetComponentsInChildren<GamePhaseBase>(), true, false);
		_winPhase.Initialize(this);
		_losePhase.Initialize(this);
	}

	protected void Start()
	{
		_gamePhaseStateMachine.StartStateMachine();
	}

	protected void OnDestroy()
	{
		_winPhase.Deinitialize();
		_losePhase.Deinitialize();
		_gamePhaseStateMachine.Dispose();
	}

	public void SetGamePhase(GamePhaseBase gamePhase)
	{
		_gamePhaseStateMachine.SetState(gamePhase);
	}

	public void GoToNextPhase()
	{
		_gamePhaseStateMachine.GoToNextState();
	}

	public void RestartPhases()
	{
		_gamePhaseStateMachine.SetState(0);
	}

	public void GoToWinPhase()
	{
		_gamePhaseStateMachine.SetState(_winPhase, _gamePhaseStateMachine.States[0]);
	}

	public void GoToLosePhase()
	{
		_gamePhaseStateMachine.SetState(_losePhase, _gamePhaseStateMachine.States[0]);
	}
}
