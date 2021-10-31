using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BossFightOutroState : BossFightStateBase
{
	[Header("Requirements")]
	[SerializeField]
	private AudioClip _killClip = null;
	[SerializeField]
	private OverlayUI _overlayUI = null;
	[SerializeField]
	private DialogUI _dialogUI = null;

	private Coroutine _cinematicRoutine = null;

	protected override void OnEnter()
	{
		_cinematicRoutine = StartCoroutine(OutroRoutine());
	}

	protected override void OnExit()
	{
		if (_cinematicRoutine != null)
		{
			StopCoroutine(_cinematicRoutine);
			_cinematicRoutine = null;
		}
	}

	private IEnumerator OutroRoutine()
	{
		yield return new WaitForSeconds(0.2f);
		StateHolder.StateAudioSource.PlayOneShot(_killClip);
		_overlayUI.FlashOverlay(0.5f, 0.1f, 0.1f);
		yield return new WaitForSeconds(1f);
		_dialogUI.ShowDialog("I...", StateHolder.DuckBossInstance.Portrait, null);
		yield return new WaitUntil(() => !_dialogUI.IsBeingShown);
		_dialogUI.ShowDialog("Is this how it ends?....", StateHolder.DuckBossInstance.Portrait, null);
		yield return new WaitUntil(() => !_dialogUI.IsBeingShown);
		_dialogUI.ShowDialog("qᵤₐcₖ", StateHolder.DuckBossInstance.Portrait, null, 1f);
		yield return new WaitUntil(() => !_dialogUI.IsBeingShown);
		_dialogUI.ShowDialog("Farewell..", StateHolder.DuckBossInstance.Portrait, null);
		StateHolder.DuckBossInstance.SpriteRenderer.DOFade(0f, 1f);
		yield return new WaitUntil(() => !_dialogUI.IsBeingShown && StateHolder.DuckBossInstance.SpriteRenderer.color.a < 0.01f);
		yield return new WaitForSeconds(1f);
		_cinematicRoutine = null;

		StateHolder.GoToNextState();
	}
}
