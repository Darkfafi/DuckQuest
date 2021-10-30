using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPhase : GamePhaseBase
{
	// UI
	[SerializeField]
	private GameObject _phaseUI = null;
	[SerializeField]
	private Button _continueButton = null;
	[SerializeField]
	private GamePhaseBase _continuePhase = null;
	[SerializeField]
	private Transform _label = null;

	public override void Initialize(GamePhasesManager manager)
	{
		base.Initialize(manager);
		_phaseUI.SetActive(false);
	}

	protected override void OnEnter()
	{
		_phaseUI.SetActive(true);
		_continueButton.onClick.AddListener(OnContinueClicked);
		_label.DOPunchScale(Vector3.one, 0.5f, 2);
	}

	private void OnContinueClicked()
	{
		GamePhasesManager.SetPhase(_continuePhase);
	}

	protected override void OnExit()
	{
		if (_continueButton != null)
		{
			_continueButton.onClick.RemoveListener(OnContinueClicked);
		}

		if (_phaseUI != null)
		{
			_phaseUI.SetActive(false);
		}
	}
}
