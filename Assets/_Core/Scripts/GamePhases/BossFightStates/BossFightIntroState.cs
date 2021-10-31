using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BossFightIntroState : BossFightStateBase
{
	[SerializeField]
	private DialogUI _dialogUI = null;
	[SerializeField]
	private AudioClip _hitClip = null;

	private Coroutine _cinematicRoutine = null;

	protected override void OnEnter()
	{
		_cinematicRoutine = StartCoroutine(IntroRoutine());
	}

	protected override void OnExit()
	{
		if(_dialogUI != null)
		{
			_dialogUI.HideDialog();
		}

		if(_cinematicRoutine != null)
		{
			StopCoroutine(_cinematicRoutine);
			_cinematicRoutine = null;
		}
	}


	private IEnumerator IntroRoutine()
	{
		StateHolder.DuckBossInstance.transform.DOShakeScale(2f, 0.5f, 4);
		yield return new WaitForSeconds(2f);
		_dialogUI.ShowDialog("Nobody has seen my chunky form and has lived to tell it.", StateHolder.DuckBossInstance.Portrait, null);
		yield return new WaitUntil(() => !_dialogUI.IsBeingShown);

		StateHolder.StateAudioSource.PlayOneShot(_hitClip);
		StateHolder.DuckBossInstance.transform.DOShakePosition(1f, 0.4f, 4);
		_dialogUI.ShowDialog("I WILL PECK YOU INTO OBLIVION!!", StateHolder.DuckBossInstance.Portrait, null);
		yield return new WaitUntil(() => !_dialogUI.IsBeingShown);

		_cinematicRoutine = null;

		StateHolder.GoToNextState();
	}
}
