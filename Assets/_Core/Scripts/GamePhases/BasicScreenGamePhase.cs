using UnityEngine;
using UnityEngine.UI;

public class BasicScreenGamePhase : GamePhaseBase
{
	// UI
	[SerializeField]
	private GameObject _phaseUI = null;
	[SerializeField]
	private Button _continueButton = null;
	[SerializeField]
	private AudioSource _playSourceOnEnterState = null;
	[SerializeField]
	private GamePhaseBase _gamePhaseAfterContinue = null;

	public override void Initialize(GameManager manager)
	{
		base.Initialize(manager);
		_phaseUI.SetActive(false);
		_playSourceOnEnterState.Stop();
	}

	protected override void OnEnter()
	{
		_phaseUI.SetActive(true);
		_playSourceOnEnterState.Play();
		_continueButton.onClick.AddListener(OnContinueClicked);
	}

	private void OnContinueClicked()
	{
		if (_gamePhaseAfterContinue != null)
		{
			StateHolder.SetGamePhase(_gamePhaseAfterContinue);
		}
		else
		{
			StateHolder.GoToNextPhase();
		}
	}

	protected override void OnExit()
	{
		if(_continueButton != null)
		{
			_continueButton.onClick.RemoveListener(OnContinueClicked);
		}

		if(_playSourceOnEnterState != null)
		{
			_playSourceOnEnterState.Stop();
		}

		if (_phaseUI != null)
		{
			_phaseUI.SetActive(false);
		}
	}
}