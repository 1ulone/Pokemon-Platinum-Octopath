using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CameraShake;

public class OverworldDialogManager : MonoBehaviour
{
	public static OverworldDialogManager d;
	public static bool onDialog;

	[SerializeField] private GameObject box;
	[SerializeField] private TextMeshPro txt;
	[SerializeField] private float lps = 30;

	private Coroutine coroutine;
	private int currentLine;

	private void Awake()
	{ 
		d = this;
		box.SetActive(false);
	}

	public void ShowDialog(List<Dialog> d, Vector3 pos)
	{
		box.SetActive(true);
		box.transform.position = new Vector3(pos.x, pos.y + 1.75f, pos.z);
		
		if (coroutine == null)
		{
			onDialog = true;
			currentLine = 0;
			box.GetComponent<SpriteRenderer>().sprite = DialogBoxSprite.b.GetBoxStyle("Default", d[currentLine].Type);
			coroutine = StartCoroutine(TypeDialog(d[currentLine].Lines)); 
			if (d[currentLine].Type == dialogType.exclamatation)
				CameraShaker.Presets.ShortShake3D();
		} else 
		if (coroutine != null)
			NextDialog(d);
	}

	public void NextDialog(List<Dialog> d)
	{
		if (currentLine >= d.Count)
		{
			CloseDialog();
			return;
		}

		box.GetComponent<SpriteRenderer>().sprite = DialogBoxSprite.b.GetBoxStyle("Default", d[currentLine].Type);
		StopCoroutine(coroutine);
		txt.text ="";

		if (d[currentLine].Type == dialogType.exclamatation)
			CameraShaker.Presets.ShortShake2D();

		StartCoroutine(TypeDialog(d[currentLine].Lines));
	}

	private void CloseDialog()
	{
		coroutine = null;
		box.SetActive(false);
		onDialog = false;
	}

	private IEnumerator TypeDialog(string str)
	{
		txt.text = "";
		foreach(var letter in str.ToCharArray())
		{
			txt.text += letter;
			yield return new WaitForSeconds(1f / lps);
		}

		currentLine++;
	}
}									  
