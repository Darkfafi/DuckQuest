using DG.Tweening;
using UnityEngine;

public class BossFightBattleState : BossFightStateBase
{
	[Header("Options")]
	[SerializeField, Range(0.1f, 0.9f)]
	private float _battleProgressStart = 0.5f;
	[SerializeField]
	private float _depletionRate = 1f;
	[SerializeField]
	private float _progressRate = 1f;

	[Header("Requirements")]
	[SerializeField]
	private AudioClip _hitClip = null;
	[SerializeField]
	private GameObject _fillContainer = null;
	[SerializeField]
	private Transform _fillUI = null;

	private float _battleProgress;

	public override void Initialize(BossFightGamePhase parent)
	{
		base.Initialize(parent);


		if (_fillContainer != null)
		{
			_fillContainer.SetActive(false);
		}
	}

	protected override void OnEnter()
	{
		StateHolder.StateAudioSource.Play(); 

		SetBattleProgress(_battleProgressStart);
		StateHolder.DuckBossInstance.EntityClickedEvent += OnBossClicked;
		
		_fillContainer.SetActive(true);
	}

	protected void Update()
	{
		if (IsCurrentState)
		{
			SetBattleProgress(_battleProgress - Time.deltaTime * _depletionRate);

			if (Mathf.Approximately(_battleProgress, 0f))
			{
				StateHolder.LosePhase();
			}
		}
	}

	private void OnBossClicked(BounceEntity boss)
	{
		SetBattleProgress(_battleProgress + _progressRate);
		StateHolder.StateAudioSource.PlayOneShot(_hitClip);
		StateHolder.DuckBossInstance.transform.DOComplete();
		StateHolder.DuckBossInstance.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 5);
		if (Mathf.Approximately(_battleProgress, 1f))
		{
			StateHolder.GoToNextState();
		}
	}

	private void SetBattleProgress(float progress)
	{
		_battleProgress = Mathf.Clamp01(progress);
		_fillUI.localScale = new Vector3(_fillUI.localScale.x, _battleProgress, _fillUI.localScale.z);
	}

	protected override void OnExit()
	{
		if (_fillContainer != null)
		{
			_fillContainer.SetActive(false);
		}

		if (StateHolder.DuckBossInstance != null)
		{
			StateHolder.DuckBossInstance.transform.DOKill();
		}

		if(StateHolder.StateAudioSource != null)
		{
			StateHolder.StateAudioSource.Stop();
		}

		StateHolder.DuckBossInstance.EntityClickedEvent -= OnBossClicked;
	}
}
