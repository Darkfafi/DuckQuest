using UnityEngine;

public class BossFightGamePhase : GamePhaseBase
{
	[Header("Requirements")]
	[SerializeField]
	private AudioSource _bgMusicSource = null;
	[SerializeField]
	private BounceEntity _duckBossPrefab = null;
	[SerializeField]
	private GameObject _phaseUI = null;

	#region Variables

	private BounceEntity _duckBossInstance = null;
	private FiniteGameStateMachine<BossFightGamePhase> _stateMachine = null;

	#endregion

	public BounceEntity DuckBossInstance => _duckBossInstance;
	public AudioSource StateAudioSource => _bgMusicSource;

	public override void Initialize(GameManager manager)
	{
		base.Initialize(manager);
		_phaseUI.SetActive(false);
		_bgMusicSource.Stop();

		_stateMachine = new FiniteGameStateMachine<BossFightGamePhase>(this, GetComponentsInChildren<BossFightStateBase>(), false, false);
	}

	public override void Deinitialize()
	{
		_stateMachine.Dispose();
		base.Deinitialize();
	}

	public void GoToNextState()
	{
		_stateMachine.GoToNextState();

		if (_stateMachine.CurrentState == null)
		{
			StateHolder.GoToNextPhase();
		}
	}

	public void LosePhase()
	{
		StateHolder.GoToLosePhase();
	}

	protected override void OnEnter()
	{
		_duckBossInstance = Instantiate(_duckBossPrefab);
		_phaseUI.SetActive(true);
		_stateMachine.StartStateMachine();
	}

	protected override void OnExit()
	{
		_stateMachine.StopStateMachine();

		if (_phaseUI != null)
		{
			_phaseUI.SetActive(false);
		}

		if(_bgMusicSource != null)
		{
			_bgMusicSource.Stop();
		}

		if (_duckBossInstance != null)
		{
			Destroy(_duckBossInstance.gameObject);
		}
	}
}
