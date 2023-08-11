using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
	[SerializeField] private GameObject exclamationMark;
	[SerializeField] private Texture2D transitionTexture;
	[SerializeField] private List<Dialog> dialog;
	[SerializeField] private NPCController con;

	private void Start()
	{
		exclamationMark.SetActive(false);
	}

	public IEnumerator TriggerTrainerBattle(Vector3 tPos, Action playerAction)
	{
		exclamationMark.SetActive(true);
		yield return new WaitForSeconds(1f);

		con.canMove = false;
		con.MoveTo(tPos);
		playerAction.Invoke();


		yield return new WaitUntil(()=> con.isMoving == false);
		exclamationMark.SetActive(false);
		OverworldDialogManager.d.ShowDialog(dialog, transform.position, ()=> 
				{
					StartCoroutine(FindObjectOfType<BattleTransition>().StartTransition(transitionTexture, ()=> 
							{
								GameSystemManager.i.SetOpponentPokemon(GetComponent<PokemonParty>());
								GameSystemManager.i.InitiateBattle(BattleAgainst.trainer);
							}));
				});
	}


}								 
