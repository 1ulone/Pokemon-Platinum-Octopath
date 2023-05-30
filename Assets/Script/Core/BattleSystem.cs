using System;
using System.Collections;
using UnityEngine;

public enum state
{
	action,
	battle,
	exit
}

public class BattleSystem : MonoBehaviour
{
	public static BattleSystem instances;

	[Header("Main Component")]
	[SerializeField] BattleUnit playerUnit;
	[SerializeField] BattleUnit opponentUnit;
	[SerializeField] BattleHUDController hud;
	[SerializeField] BattleHUDController opphud;

	[Header("UI Component")]
	[SerializeField] BattleDialog dialog;

	[Header("Player UI Component")]
	[SerializeField] BattleMenu menu;
	[SerializeField] MoveUI moveOptions;
	[SerializeField] PartyUI partyMemberUI; 

	public PokemonParty PlayerParty { get { return playerParty; } }

	private state state;
	private MoveClass playerMove;
	private MoveClass opponentMove;

	private PokemonParty playerParty;
	private PokemonParty opponentParty;
	private PokemonClass opponentPokemon;

	private void Awake()
	{
		instances = this;
	}

	public void StartBattle(PokemonParty pparty, PokemonClass oppoP)
	{
		this.playerParty = pparty;
		this.opponentPokemon = oppoP;

		partyMemberUI.InitializeUI();

		StartCoroutine(SetupBattle());
	}

	public IEnumerator SetupBattle()
	{
		playerUnit.Setup(playerParty.GetPokemon());
		hud.SetData(playerUnit.pokemon);
		moveOptions.Setup(playerUnit.pokemon.moves);

		opponentUnit.Setup(opponentPokemon);
		opphud.SetData(opponentUnit.pokemon);

		BattleCamera.cam.SetBattleCamera(playerUnit.pokemon.data.inGameSize, opponentUnit.pokemon.data.inGameSize);

		yield return dialog.TypeDialog($"a wild {opponentUnit.pname} appeared!");
		yield return new WaitForSeconds(1f);
		ACTIONstate();
	}

	public void ACTIONstate()
	{
		StartCoroutine(dialog.TypeDialog("What will you do ?"));
		StartCoroutine(BattleCamera.cam.ChangeState(camState.onIdle));
		state = state.action;
		menu.toggleMenu(true);
	}

	public IEnumerator BATTLEstate()
	{
		state = state.battle;
		yield return BattleCamera.cam.ChangeState(camState.onAttack);
		yield return PlayerTurn();
	}	

	public void SetPlayerMove(MoveClass move)
		=> playerMove = move;

	private IEnumerator PlayerTurn()
	{
		playerMove.pp--;
		moveOptions.UpdateUI();

		yield return dialog.TypeDialog($"{playerUnit.pname} use {playerMove.data.mname}");
		yield return BattleCamera.cam.ChangeState(camState.onZoomEnemy);

		DamageDetails details = opponentUnit.pokemon.TakeDamage(playerMove, playerUnit.pokemon);
		opphud.UpdateHP();
		StartCoroutine(opponentUnit.hitEffect());
		yield return ShowDamageDetails(details);

		if (details.fainted)
			{ yield return dialog.TypeDialog($"{opponentUnit.pname} Fainted"); GameSystemManager.i.ExitBattle(true); }
		else
			StartCoroutine(OpponentTurn());

		yield return BattleCamera.cam.ChangeState(camState.onAttack);
	}

	private IEnumerator OpponentTurn()
	{
		opponentMove = opponentUnit.pokemon.GetRandomMove();
		opponentMove.pp--;

		yield return dialog.TypeDialog($"{opponentUnit.pname} use {opponentMove.data.mname}");
		yield return BattleCamera.cam.ChangeState(camState.onZoomPlayer);

		DamageDetails details = playerUnit.pokemon.TakeDamage(opponentMove, opponentUnit.pokemon);
		hud.UpdateHP();
		StartCoroutine(playerUnit.hitEffect());
		yield return ShowDamageDetails(details);

		if (details.fainted)
		{ 
			yield return dialog.TypeDialog($"{playerUnit.pname} Fainted"); 
			var nextPokemon = playerParty.GetPokemon();
			if (nextPokemon != null)
			{
				yield return dialog.TypeDialog($"You did well {playerUnit.pname}");

				playerUnit.Setup(nextPokemon);
				hud.SetData(nextPokemon);
				moveOptions.Setup(nextPokemon.moves);

				yield return dialog.TypeDialog($"Go {nextPokemon.data.pname}!");

				ACTIONstate();
			} else {
				GameSystemManager.i.ExitBattle(false); 
			}
		}
		else
			ACTIONstate();

	}

	private IEnumerator ShowDamageDetails(DamageDetails details)
	{
		if (details.critical > 1f)
			yield return dialog.TypeDialog("A critical hit!");

		if (details.typeEffectiveness > 1f)
			yield return dialog.TypeDialog("It's super effective!"); else
		if (details.typeEffectiveness < 1f)
			yield return dialog.TypeDialog("It's not very effective");
	}
}							
