using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour 
{
	[SerializeField] private string trainerName;
	[SerializeField] private string trainerClass;
	[SerializeField] private RuntimeAnimatorController animator;

	[SerializeField] private GameObject exclamationMark;
	[SerializeField] private Texture2D transitionTexture;
	[SerializeField] private List<Dialog> dialog;
	[SerializeField] private NPCController con;
	
	private Animator anim;
	private Vector3 faceDirection;
	public string tname { get { return trainerClass + " " + trainerName; } }

	private void Start()
	{
		exclamationMark.SetActive(false);
		anim = GetComponentInChildren<Animator>();
		con.battleReady = true;
	}

	public IEnumerator TriggerTrainerBattle(Vector3 tPos, Action playerAction = null)
	{
		GameSystemManager.TransitionToBattle = true;

		exclamationMark.SetActive(true);
		yield return new WaitForSeconds(1f);

		if (tPos != this.transform.position)
		{
			con.canMove = false;
			con.MoveTo(tPos);
			playerAction?.Invoke();

			yield return new WaitUntil(()=> con.isMoving == false);
		}

		exclamationMark.SetActive(false);
		OverworldDialogManager.d.ShowDialog(dialog, transform.position, ()=> 
		{
			StartCoroutine(FindObjectOfType<BattleTransition>().StartTransition(transitionTexture, ()=> 
			{
				GameSystemManager.i.SetOpponentData(tname, animator);
				GameSystemManager.i.SetOpponentPokemon(GetComponent<PokemonParty>());
				GameSystemManager.i.InitiateBattle(BattleAgainst.trainer);
				con.battleReady = false;
			}));
		});
	}
}								 
