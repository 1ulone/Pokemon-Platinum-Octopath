using System.Collections;
using UnityEngine;
using TMPro;

public class BattleDialog : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI t;
	[SerializeField] private int typeSpeed = 30;

	public void SetDialog(string t)
	{
		this.t.text = t;
	}

	public IEnumerator TypeDialog(string t)
	{
		this.t.text = "";
		foreach(var l in t.ToCharArray())
		{
			this.t.text += l;
			yield return new WaitForSeconds(1f/typeSpeed);
		}

		yield return new WaitForSeconds(1f);
	}
}							
