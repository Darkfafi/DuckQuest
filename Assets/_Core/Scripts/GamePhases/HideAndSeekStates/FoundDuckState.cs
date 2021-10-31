using DG.Tweening;
using System.Collections;
using UnityEngine;

public class FoundDuckState : HideAndSeekStateBase
{
	[Header("Requirements")]
	[SerializeField]
	private DialogUI _dialogUI = null;
	[SerializeField]
	private OverlayUI _overlayUI = null;

	[Header("Audio")]
	[SerializeField]
	private AudioClip _foundDuckClip = null;
	[SerializeField]
	private AudioClip _flashClip = null;

	private Coroutine _foundDuckRoutine = null;

	protected override void OnEnter()
	{
		_foundDuckRoutine = StartCoroutine(FoundDuckRoutine());
	}

	protected override void OnExit()
	{
		if (_foundDuckRoutine != null)
		{
			StopCoroutine(FoundDuckRoutine());
			_foundDuckRoutine = null;
		}

		if (_dialogUI != null)
		{
			_dialogUI.HideDialog();
		}
	}

	private IEnumerator FoundDuckRoutine()
	{
		StateHolder.StateAudioSource.Stop();
		StateHolder.CreatedDuck.StopRunning();

		StateHolder.StateAudioSource.PlayOneShot(_foundDuckClip);

		foreach (var frog in StateHolder.CreatedFrogs)
		{
			frog.StopRunning();

			Vector3 dirVec = StateHolder.CreatedDuck.transform.position - frog.transform.position;
			if (dirVec.magnitude < 1.5f)
			{
				frog.transform.DOMove(StateHolder.CreatedDuck.transform.position + (-dirVec.normalized * 1.5f), 1f);
			}
		}

		_dialogUI.ShowDialog("...", StateHolder.CreatedDuck.Portrait, null);
		yield return new WaitUntil(() => !_dialogUI.IsBeingShown);
		_dialogUI.ShowDialog("Found by a mere ape...", StateHolder.CreatedDuck.Portrait, null);
		yield return new WaitUntil(() => !_dialogUI.IsBeingShown);
		_dialogUI.ShowDialog("I will make you fear me!", StateHolder.CreatedDuck.Portrait, null);
		yield return new WaitUntil(() => !_dialogUI.IsBeingShown);

		float flashDuration = 2f;

		StateHolder.StateAudioSource.PlayOneShot(_flashClip);

		_overlayUI.FlashOverlay(flashDuration);
		yield return new WaitForSeconds(flashDuration);
		_foundDuckRoutine = null;

		StateHolder.GoToNextState();
	}
}
