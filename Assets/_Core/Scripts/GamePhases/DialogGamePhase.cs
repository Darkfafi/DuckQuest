using UnityEngine;

public class DialogGamePhase : GamePhaseBase
{
	[SerializeField]
	private Sprite _portrait = null;

	[SerializeField, TextArea]
	private string[] _dialog;

	[SerializeField]
	private DialogUI _dialogUI = null;

	private int _currentIndex = 0;

	protected override void OnEnter()
	{
		_currentIndex = 0;
		ShowDialog();
	}

	protected override void OnExit()
	{
		if(_dialogUI != null)
		{
			_dialogUI.HideDialog();
		}
	}

	private void ShowDialog()
	{
		_dialogUI.ShowDialog(_dialog[_currentIndex], _portrait, OnContinueDialog);
	}

	private void OnContinueDialog()
	{
		_currentIndex++;
		if(_currentIndex >= _dialog.Length)
		{
			StateHolder.GoToNextPhase();
		}
		else
		{
			ShowDialog();
		}
	}
}
