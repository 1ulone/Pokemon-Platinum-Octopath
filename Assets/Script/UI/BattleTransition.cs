using System;
using System.Collections;
using UnityEngine;

public class BattleTransition : MonoBehaviour
{
	[SerializeField] private CanvasGroup whiteFlash; 
	[SerializeField] private bool inBattle = false;
	private SpriteRenderer sRenderer;

	private void Awake()
		=> sRenderer = GetComponent<SpriteRenderer>();

	private void OnEnable()
	{ 
		if (!inBattle)
			sRenderer.material.SetFloat("_CutOff", 0); else 
		if (inBattle)
			sRenderer.material.SetFloat("_CutOff", 1);  
	}

	public IEnumerator StartTransition(Texture2D transitionTex, Action onEndTransition)
	{
		LeanTween.reset();

		int i = 2;
		while(i > 0)
		{
			LeanTween.value(0, 1, 0.25f).setOnUpdate((float x)=> whiteFlash.alpha = x);
			yield return new WaitUntil(()=> whiteFlash.alpha >= 1);
			LeanTween.value(1, 0, 0.25f).setOnUpdate((float x)=> whiteFlash.alpha = x);
			yield return new WaitUntil(()=> whiteFlash.alpha <= 0);
			i--;
		}

		yield return new WaitUntil(()=> i <= 0);
		LeanTween.value(0, 1, 0.45f).setOnStart(()=> sRenderer.material.SetTexture("_TransTex", transitionTex))
								   .setOnUpdate((float x)=> sRenderer.material.SetFloat("_CutOff", x) )
								   .setOnComplete(onEndTransition);
	}

	public void BattleOpeningTransition()
	{
		LeanTween.reset();
		LeanTween.value(1, 0, 2.5f).setOnUpdate((float x)=> sRenderer.material.SetFloat("_CutOff", x));
	}
}								
