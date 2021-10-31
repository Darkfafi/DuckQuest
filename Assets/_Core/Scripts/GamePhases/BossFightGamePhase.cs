using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BossFightGamePhase : GamePhaseBase
{
	[Header("Options")]
	[SerializeField]
	private float _depletionRate = 1f;
	[SerializeField]
	private float _progressRate = 1f;

	[SerializeField, Range(0.1f, 0.9f)]
	private float _battleProgressStart = 0.5f;

	[Header("Requirements")]
	[SerializeField]
	private AudioSource _bgMusicSource = null;
	[SerializeField]
	private AudioClip _hitClip = null;
	[SerializeField]
	private AudioClip _killClip = null;

	[SerializeField]
	private DialogUI _dialogUI = null;
	[SerializeField]
	private BounceEntity _duckBossPrefab = null;
	[SerializeField]
	private GameObject _phaseUI = null;
	[SerializeField]
	private Transform _fillUI = null;
	[SerializeField]
	private OverlayUI _overlayUI = null;

	private float _battleProgress;
	private BounceEntity _duckBossInstance = null;
	private Coroutine _cinematicRoutine = null;

	public override void Initialize(GameManager manager)
	{
		base.Initialize(manager);
		_phaseUI.SetActive(false);
		_bgMusicSource.Stop();
	}

	protected override void OnEnter()
	{
		_battleProgress = _battleProgressStart;
		_duckBossInstance = Instantiate(_duckBossPrefab);
		_duckBossInstance.EntityClickedEvent += OnBossClicked;
		_phaseUI.SetActive(true);

		UpdateProgressBar();
		_cinematicRoutine = StartCoroutine(IntroRoutine());
	}

	protected void Update()
	{
		if(IsCurrentState && _cinematicRoutine == null)
		{
			_battleProgress -= Time.deltaTime * _depletionRate;

			if(_battleProgress <= 0f)
			{
				_battleProgress = 0f;
				StateHolder.GoToLosePhase();
			}
			UpdateProgressBar();
		}
	}

	private void OnBossClicked(BounceEntity boss)
	{
		if (_cinematicRoutine == null)
		{
			_battleProgress += Time.deltaTime * _progressRate;
			_bgMusicSource.PlayOneShot(_hitClip);
			_duckBossInstance.transform.DOComplete();
			_duckBossInstance.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 5);
			if (_battleProgress >= 1f)
			{
				_battleProgress = 1f;
				_cinematicRoutine = StartCoroutine(OutroRoutine());
			}
			UpdateProgressBar();
		}
	}

	protected override void OnExit()
	{
		if (_phaseUI != null)
		{
			_phaseUI.SetActive(false);
		}

		if(_bgMusicSource != null)
		{
			_bgMusicSource.Stop();
		}

		if(_cinematicRoutine != null)
		{
			StopCoroutine(_cinematicRoutine);
			_cinematicRoutine = null;
		}

		if (_duckBossInstance != null)
		{
			_duckBossInstance.transform.DOKill();
			_duckBossInstance.EntityClickedEvent -= OnBossClicked;
			Destroy(_duckBossInstance.gameObject);
		}
	}

	private void UpdateProgressBar()
	{
		_fillUI.localScale = new Vector3(_fillUI.localScale.x, _battleProgress, _fillUI.localScale.z);
	}

	private IEnumerator IntroRoutine()
	{
		_duckBossInstance.transform.DOShakeScale(2f, 0.5f, 4);
		yield return new WaitForSeconds(2f);
		_dialogUI.ShowDialog("Nobody has seen my chunky form and has lived to tell it.", _duckBossInstance.Portrait, null);
		yield return new WaitUntil(() => !_dialogUI.IsBeingShown);

		_bgMusicSource.PlayOneShot(_hitClip);
		_duckBossInstance.transform.DOShakePosition(1f, 0.4f, 4);
		_dialogUI.ShowDialog("I WILL PECK YOU INTO OBLIVION!!", _duckBossInstance.Portrait, null);
		yield return new WaitUntil(() => !_dialogUI.IsBeingShown);

		_bgMusicSource.Play();

		_cinematicRoutine = null;
	}

	private IEnumerator OutroRoutine()
	{
		_bgMusicSource.Stop();
		yield return new WaitForSeconds(0.2f);
		_bgMusicSource.PlayOneShot(_killClip);
		_overlayUI.FlashOverlay(0.5f, 0.1f, 0.1f);
		yield return new WaitForSeconds(1f);
		_dialogUI.ShowDialog("I...", _duckBossInstance.Portrait, null);
		yield return new WaitUntil(() => !_dialogUI.IsBeingShown);
		_dialogUI.ShowDialog("Is this how it ends?....", _duckBossInstance.Portrait, null);
		yield return new WaitUntil(() => !_dialogUI.IsBeingShown);
		_dialogUI.ShowDialog("qᵤₐcₖ", _duckBossInstance.Portrait, null, 1f);
		yield return new WaitUntil(() => !_dialogUI.IsBeingShown);
		_dialogUI.ShowDialog("Farewell..", _duckBossInstance.Portrait, null);
		_duckBossInstance.SpriteRenderer.DOFade(0f, 1f);
		yield return new WaitUntil(() => !_dialogUI.IsBeingShown && _duckBossInstance.SpriteRenderer.color.a < 0.01f);
		yield return new WaitForSeconds(1f);
		_cinematicRoutine = null;

		StateHolder.GoToNextPhase();
	}
}
