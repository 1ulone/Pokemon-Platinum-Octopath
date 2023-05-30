using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
	public static HitStop h;

	private Coroutine coroutine;

	private void Awake()
		=> h = this;

	public void StartHitStop(float t, float scale = 0)
	{
		if (coroutine == null)
			coroutine = StartCoroutine(StopTime(t, scale));
		else 
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
			coroutine = StartCoroutine(StopTime(t, scale));
		}
	}

	public IEnumerator StopTime(float t, float scale)
	{
		Time.timeScale = scale;
		yield return new WaitForSecondsRealtime(t);
		Time.timeScale = 1;
	}
}					   
