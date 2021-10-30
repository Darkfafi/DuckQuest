using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DialogUI : MonoBehaviour, IPointerClickHandler
{
	[SerializeField]
	private GameObject _container = null;
	[SerializeField]
	private Image _portraitImage = null;
	[SerializeField]
	private Text _textLabel = null;

	[SerializeField]
	private AudioSource _audioSource = null;
	[SerializeField]
	private AudioClip _charSound = null;

	private Action _callback = null;
	private Coroutine _revealTextRoutine = null;
	private Coroutine _durationRoutine = null;
	private bool _clickable = false;
	private string _textToDisplay = null;
	private float _duration = 1f;

	public bool IsBeingShown
	{
		get; private set;
	}

	protected void Awake()
	{
		HideDialog();
	}

	public void ShowDialog(string text, Sprite portrait, Action continueCallback, float? duration = null, bool clickable = true)
	{
		HideDialog();

		IsBeingShown = true;
		_container.SetActive(true);
		_textToDisplay = text;
		_clickable = clickable;

		_portraitImage.sprite = portrait;
		_portraitImage.gameObject.SetActive(portrait != null);

		_textLabel.text = FormatDisplayText(0f, out _);
		_callback = continueCallback;

		if(duration.HasValue)
		{
			_durationRoutine = StartCoroutine(DurationRoutine(duration.Value));
			_duration = Mathf.Min(duration.Value * 0.5f, 1f);
		}

		_revealTextRoutine = StartCoroutine(RevealTextRoutine());
	}

	public void ContinueDialog()
	{
		HideDialog();
		Action callback = _callback;
		_callback = null;
		callback?.Invoke();
	}

	public void HideDialog()
	{
		_container.SetActive(false);

		if(_revealTextRoutine != null)
		{
			StopCoroutine(_revealTextRoutine);
			_revealTextRoutine = null;
		}

		if(_durationRoutine != null)
		{
			StopCoroutine(_durationRoutine);
			_durationRoutine = null;
		}
		IsBeingShown = false;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (_clickable)
		{
			if (_revealTextRoutine != null)
			{
				StopCoroutine(_revealTextRoutine);
				_revealTextRoutine = null;
				_textLabel.text = FormatDisplayText(1f, out _);
			}
			else
			{
				ContinueDialog();
			}
		}
	}

	private IEnumerator RevealTextRoutine()
	{
		float progress = 0f;
		int previousRevealedCount = 0;
		float step = 1f / _duration;
		float timeSinceSound = 1f;

		while (progress < 1f)
		{
			progress += Time.deltaTime * step;
			timeSinceSound += Time.deltaTime;
			if (progress >= 1f)
			{
				progress = 1f;
			}
			_textLabel.text = FormatDisplayText(progress, out int revealedCount);

			if(previousRevealedCount != revealedCount)
			{
				previousRevealedCount = revealedCount;
				if (timeSinceSound > 0.1f)
				{
					_audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
					_audioSource.PlayOneShot(_charSound, 0.5f);
					timeSinceSound = 0f;
				}
			}

			yield return null;
		}
		_revealTextRoutine = null;
	}

	private IEnumerator DurationRoutine(float duration)
	{
		yield return new WaitForSeconds(duration);
		ContinueDialog();
		_durationRoutine = null;
	}

	private string FormatDisplayText(float progress, out int revealedCount)
	{
		string hiddenText = _textToDisplay;
		revealedCount = Mathf.FloorToInt(hiddenText.Length * progress);
		string revealedText = hiddenText.Substring(0, revealedCount);
		hiddenText = hiddenText.Remove(0, revealedCount);

		return string.Format("{0}<color=#0000>{1}</color>", revealedText, hiddenText);
	}
}
