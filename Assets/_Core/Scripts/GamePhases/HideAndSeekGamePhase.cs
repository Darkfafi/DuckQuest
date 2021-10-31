using System.Collections.Generic;
using UnityEngine;

public class HideAndSeekGamePhase : GamePhaseBase
{
	[Header("Options")]
	[SerializeField]
	private int _frogAmount = 10;

	[Header("Requirements")]
	[SerializeField]
	private AudioSource _bgmSource = null;

	// World
	[SerializeField]
	private BounceEntity _frogPrefab = null;
	[SerializeField]
	private BounceEntity _duckPrefab = null;

	// UI
	[SerializeField]
	private GameObject _phaseUI = null;

	#region Variables

	private List<BounceEntity> _createdFrogs = new List<BounceEntity>();
	private BounceEntity _createdDuck = null;
	private FiniteGameStateMachine<HideAndSeekGamePhase> _stateMachine = null;

	#endregion

	#region Properties

	public BounceEntity CreatedDuck => _createdDuck;
	public IReadOnlyList<BounceEntity> CreatedFrogs => _createdFrogs;
	public AudioSource StateAudioSource => _bgmSource;

	#endregion

	public override void Initialize(GameManager manager)
	{
		base.Initialize(manager);
		_stateMachine = new FiniteGameStateMachine<HideAndSeekGamePhase>(this, GetComponentsInChildren<HideAndSeekStateBase>(), false, false);
		_phaseUI.SetActive(false);
		_bgmSource.Stop();
	}

	public override void Deinitialize()
	{
		_stateMachine.Dispose();
		base.Deinitialize();
	}

	public void GoToNextState()
	{
		_stateMachine.GoToNextState();

		if(_stateMachine.CurrentState == null)
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
		_phaseUI.SetActive(true);
		_bgmSource.Play();

		_createdDuck = Instantiate(_duckPrefab);

		for(int i = 0; i < _frogAmount; i++)
		{
			BounceEntity frog = Instantiate
			(
				_frogPrefab, 
				new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f), 
				Quaternion.identity
			);

			_createdFrogs.Add(frog);
		}

		_stateMachine.StartStateMachine();
	}

	protected override void OnExit()
	{
		_stateMachine.StopStateMachine();

		if (_phaseUI != null)
		{
			_phaseUI.SetActive(false);
		}

		if(_bgmSource != null)
		{
			_bgmSource.Stop();
		}

		for (int i = _createdFrogs.Count - 1; i >= 0; i--)
		{
			BounceEntity frog = _createdFrogs[i];
			if (frog != null)
			{
				Destroy(frog.gameObject);
			}
		}
		_createdFrogs.Clear();
		
		if (_createdDuck != null)
		{
			Destroy(_createdDuck.gameObject);
			_createdDuck = null;
		}
	}
}
