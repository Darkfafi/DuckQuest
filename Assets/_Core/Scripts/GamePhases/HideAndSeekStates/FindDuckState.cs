using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FindDuckState : HideAndSeekStateBase
{
	[Header("Options")]
	[SerializeField]
	private int _phaseTime = 10;

	[Header("Requirements")]
	[SerializeField]
	private DialogUI _dialogUI = null;
	[SerializeField]
	private Text _clockLabel = null;

	private Coroutine _phaseTimerRoutine = null;

	protected override void OnEnter()
	{
		_clockLabel.text = "";
		StateHolder.CreatedDuck.EntityClickedEvent += OnDuckClickedEvent;

		StateHolder.CreatedDuck.StartRunning();

		foreach(BounceEntity frog in StateHolder.CreatedFrogs)
		{ 
			frog.EntityClickedEvent += OnFrogClickedEvent;
			frog.StartRunning();
		}

		_phaseTimerRoutine = StartCoroutine(PhaseTimerRoutine());
	}
	private void OnDuckClickedEvent(BounceEntity duck)
	{
		StateHolder.GoToNextState();
	}

	private void OnFrogClickedEvent(BounceEntity frog)
	{
		_dialogUI.ShowDialog("Rabbit", frog.Portrait, null, 1f, clickable: false);
	}

	private IEnumerator PhaseTimerRoutine()
	{
		float timeLeft = _phaseTime;
		while (timeLeft > 0)
		{
			timeLeft -= Time.deltaTime;
			_clockLabel.text = Mathf.CeilToInt(timeLeft).ToString();
			yield return null;
		}
		_phaseTimerRoutine = null;
		StateHolder.LosePhase();
	}

	protected override void OnExit()
	{
		if (_phaseTimerRoutine != null)
		{
			StopCoroutine(_phaseTimerRoutine);
			_phaseTimerRoutine = null;
		}

		foreach (BounceEntity frog in StateHolder.CreatedFrogs)
		{
			frog.EntityClickedEvent -= OnFrogClickedEvent;
		}
		StateHolder.CreatedDuck.EntityClickedEvent -= OnDuckClickedEvent;

		if (_dialogUI != null)
		{
			_dialogUI.HideDialog();
		}
	}
}
