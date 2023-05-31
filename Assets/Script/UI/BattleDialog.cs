using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleDialog : MonoBehaviour
{
	public static BattleDialog d;

	[SerializeField] private TextMeshProUGUI t;
	[SerializeField] private int typeSpeed = 30;

	private float expireTimer;
	private bool isCoroutine;

	private void Start()
	{ 
		d = this;
		expireTimer = 0f;
	}

	public void SetDialog(string t)
	{
		this.t.text = t;
	}

	public IEnumerator TypeDialog(string t)
	{
		while (isCoroutine)
		{
			expireTimer += Time.deltaTime;
			if (expireTimer >= 2)
				yield break;

			yield return new WaitForSeconds(0);
		}

		expireTimer = 0;
		yield return TypeEffect(t);  
	
		yield return new WaitForSeconds(1f);
		isCoroutine = false;
	}

	public IEnumerator TypeEffect(string t)
	{
		isCoroutine = true;
		this.t.text = "";
		foreach(var l in t.ToCharArray())
		{
			this.t.text += l;
			yield return new WaitForSeconds(1f/typeSpeed);
		}

	}
}							
