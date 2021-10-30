using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class OverlayUI : MonoBehaviour
{
	[SerializeField]
	private Image _overlay = null;

	private Sequence _sequence = null;

	public void FlashOverlay(float duration, float fadeInDuration = 0.3f, float fadeOutDuration = 0.3f)
	{
		if(_sequence != null)
		{
			_sequence.Kill();
		}

		_sequence = DOTween.Sequence();
		_sequence.Append(_overlay.DOFade(1f, fadeInDuration));
		_sequence.Append(_overlay.DOFade(0f, fadeOutDuration).SetDelay(duration));
	}
}
