using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HideAndSeekGamePhase : GamePhaseBase
{
	[Header("Options")]
	[SerializeField]
	private int _phaseTime = 10;
	[SerializeField]
	private int _frogAmount = 10;

	[Header("Redirects")]
	[SerializeField]
	private GamePhaseBase _losePhase;
	[SerializeField]
	private GamePhaseBase _successPhase;

	[Header("Requirements")]
	[SerializeField]
	private AudioSource _bgmSource = null;
	[SerializeField]
	private AudioClip _foundDuckClip = null;
	[SerializeField]
	private AudioClip _flashClip = null;

	// World
	[SerializeField]
	private BounceEntity _frogPrefab = null;
	[SerializeField]
	private BounceEntity _duckPrefab = null;

	// UI
	[SerializeField]
	private DialogUI _dialogUI = null;
	[SerializeField]
	private OverlayUI _overlayUI = null;
	[SerializeField]
	private GameObject _phaseUI = null;
	[SerializeField]
	private Text _clockLabel = null;

	private List<BounceEntity> _createdFrogs = new List<BounceEntity>();
	private BounceEntity _createdDuck = null;

	private Coroutine _phaseTimerRoutine = null;
	private Coroutine _foundDuckRoutine = null;

	public override void Initialize(GamePhasesManager manager)
	{
		base.Initialize(manager);
		_phaseUI.SetActive(false);
		_bgmSource.Stop();
	}

	protected override void OnEnter()
	{
		_phaseUI.SetActive(true);
		_clockLabel.text = "";
		_bgmSource.Play();

		_createdDuck = Instantiate(_duckPrefab);
		_createdDuck.EntityClickedEvent += OnDuckClickedEvent;

		for(int i = 0; i < _frogAmount; i++)
		{
			BounceEntity frog = Instantiate
			(
				_frogPrefab, 
				new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f), 
				Quaternion.identity
			);

			_createdFrogs.Add(frog);

			frog.EntityClickedEvent += OnFrogClickedEvent;
		}

		_phaseTimerRoutine = StartCoroutine(PhaseTimerRoutine());
	}

	private void OnDuckClickedEvent(BounceEntity duck)
	{
		if (_foundDuckRoutine == null)
		{
			if (_phaseTimerRoutine != null)
			{
				StopCoroutine(_phaseTimerRoutine);
				_phaseTimerRoutine = null;
			}

			_foundDuckRoutine = StartCoroutine(FoundDuckRoutine());
		}
	}

	private void OnFrogClickedEvent(BounceEntity frog)
	{
		if (_foundDuckRoutine == null)
		{
			_dialogUI.ShowDialog("Rabbit", frog.Portrait, null, 1f, clickable: false);
		}
	}

	private IEnumerator FoundDuckRoutine()
	{
		_bgmSource.Stop();
		_createdDuck.StopRunning();

		_bgmSource.PlayOneShot(_foundDuckClip);

		foreach (var frog in _createdFrogs)
		{
			frog.StopRunning();

			Vector3 dirVec = _createdDuck.transform.position - frog.transform.position;
			if (dirVec.magnitude < 1.5f)
			{
				frog.transform.DOMove(_createdDuck.transform.position + (-dirVec.normalized * 1.5f), 1f);
			}
		}

		_dialogUI.ShowDialog("...", _createdDuck.Portrait, null);
		yield return new WaitUntil(()=> !_dialogUI.IsBeingShown);
		_dialogUI.ShowDialog("Found by a mere ape...", _createdDuck.Portrait, null);
		yield return new WaitUntil(()=> !_dialogUI.IsBeingShown);
		_dialogUI.ShowDialog("I will make you fear me!", _createdDuck.Portrait, null);
		yield return new WaitUntil(()=> !_dialogUI.IsBeingShown);

		float flashDuration = 2f;

		_bgmSource.PlayOneShot(_flashClip);

		_overlayUI.FlashOverlay(flashDuration);
		yield return new WaitForSeconds(flashDuration);
		GamePhasesManager.SetPhase(_successPhase);
		_foundDuckRoutine = null;
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

		GamePhasesManager.SetPhase(_losePhase);
	}

	protected override void OnExit()
	{
		if (_dialogUI != null)
		{
			_dialogUI.HideDialog();
		}

		if (_phaseUI != null)
		{
			_phaseUI.SetActive(false);
		}

		if(_bgmSource != null)
		{
			_bgmSource.Stop();
		}

		if (_phaseTimerRoutine != null)
		{
			StopCoroutine(_phaseTimerRoutine);
			_phaseTimerRoutine = null;
		}

		if (_foundDuckRoutine != null)
		{
			StopCoroutine(_foundDuckRoutine);
			_foundDuckRoutine = null;
		}

		for (int i = _createdFrogs.Count - 1; i >= 0; i--)
		{
			BounceEntity frog = _createdFrogs[i];
			if (frog != null)
			{
				frog.EntityClickedEvent -= OnFrogClickedEvent;
				Destroy(frog.gameObject);
			}
		}
		_createdFrogs.Clear();
		
		if (_createdDuck != null)
		{
			_createdDuck.EntityClickedEvent -= OnDuckClickedEvent;
			Destroy(_createdDuck.gameObject);
			_createdDuck = null;
		}
	}
}
