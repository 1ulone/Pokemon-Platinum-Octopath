using System;
using UnityEngine;

public class ConfirmButton : MonoBehaviour
{
	[SerializeField] private RectTransform yesB;
	[SerializeField] private RectTransform noB;

	public Action confirmAction, cancelAction;
	
	private void Start()
	{
		ExitButton();
	}

	public void ShowButton()
	{
		LeanTween.move(yesB, new Vector3(362, 226, 0), 0.25f).setEaseOutBack();
		LeanTween.move(noB, new Vector3(362, 174, 0), 0.35f).setEaseOutBack();
	}

	public void ExitButton()
	{
		LeanTween.move(yesB, new Vector3(362, 66, 0), 0.25f).setEaseInBack();
		LeanTween.move(noB, new Vector3(362, 12, 0), 0.35f).setEaseInBack();

		confirmAction = null;
		cancelAction = null;
	}

	public void Accept()
	{
		confirmAction.Invoke();
	}

	public void Cancel()
	{
		cancelAction.Invoke();
		ExitButton();
	}
}														 
