using System;
using UnityEngine;

public class BattleTransition : MonoBehaviour
{
	[SerializeField] CanvasGroup cg;
	private SpriteRenderer sRenderer;

	private void Awake()
		=> sRenderer = GetComponent<SpriteRenderer>();

	public void StartTransition(Texture2D transitionTex, Action onEndTransition)
	{
		LeanTween.value(0, 1, 2f).setOnStart(()=> sRenderer.material.SetTexture("_TransTex", transitionTex))
								 .setOnUpdate((float x)=> sRenderer.material.SetFloat("_CutOff", x) )
								 .setOnComplete(onEndTransition);
	}

	public void BattleOpeningTransition()
	{
		LeanTween.reset();
		LeanTween.value(1, 0, 1.5f).setOnStart(()=> cg.alpha = 0)
								 .setOnUpdate((float x)=> 
										 {
											sRenderer.material.SetFloat("_CutOff", x);
										 })
								 .setOnComplete(()=> cg.alpha = 1);

	}
}								
