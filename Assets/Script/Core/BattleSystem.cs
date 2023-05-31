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

	[Header("UI Component")]
	[SerializeField] BattleDialog dialog;

	[Header("Player UI Component")]
	[SerializeField] BattleMenu menu;
	[SerializeField] MoveUI moveOptions;
	[SerializeField] PartyUI partyMemberUI; 

	public PokemonParty PlayerParty { get { return playerParty; } }
	public PokemonClass PlayerCurrentPokemon { get { return playerUnit.pokemon; } }

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
		moveOptions.Setup(playerUnit.pokemon.moves);

		opponentUnit.Setup(opponentPokemon);

		BattleCamera.cam.SetBattleCamera(playerUnit.pokemon.data.inGameSize, opponentUnit.pokemon.data.inGameSize);

		yield return dialog.TypeDialog($"a wild {opponentUnit.pname} appeared!");
		ACTIONstate();
	}

	public void ACTIONstate()
	{
		state = state.action;
		StartCoroutine(dialog.TypeDialog("What will you do ?"));
		StartCoroutine(BattleCamera.cam.ChangeState(camState.onIdle));
		menu.toggleMenu(true);
	}

	public IEnumerator BATTLEstate()
	{
		state = state.battle;
		yield return BattleCamera.cam.ChangeState(camState.onAttack);
		yield return PlayerTurn();
	}	

	private void EXITstate(bool w)
	{
		state = state.exit;
		GameSystemManager.i.ExitBattle(w);
	}

	public IEnumerator SwitchPokemon(PokemonClass newPokemon)
	{
		yield return BattleCamera.cam.ChangeState(camState.onAttack);

		if (playerUnit.pokemon.HP > 0)
		{
			yield return dialog.TypeDialog($"Come back {playerUnit.pname}");
			playerUnit.FaintAnimation();
			yield return new WaitForSeconds(1f);
		}

		playerUnit.Setup(newPokemon);
		moveOptions.Setup(newPokemon.moves);
		yield return dialog.TypeDialog($"Go {playerUnit.pname}!");

		StartCoroutine(OpponentTurn());
	}

	public void SetPlayerMove(MoveClass move)
		=> playerMove = move;

	private IEnumerator PlayerTurn()
	{
		state = state.battle;

		playerMove.pp--;
		moveOptions.UpdateUI();

		yield return ExecuteMove(playerUnit, opponentUnit, playerMove);

		if (state == state.battle)
			StartCoroutine(OpponentTurn());
	}

	private IEnumerator OpponentTurn()
	{
		opponentMove = opponentUnit.pokemon.GetRandomMove();
		opponentMove.pp--;

		yield return ExecuteMove(opponentUnit, playerUnit, opponentMove); 

		if (state == state.battle)
			ACTIONstate();
	}

	private IEnumerator ExecuteMove(BattleUnit source, BattleUnit target, MoveClass move)
	{
		yield return dialog.TypeDialog($"{source.pname} use {move.data.mname}");
		yield return BattleCamera.cam.ChangeState(camState.onZoomEnemy);

		DamageDetails details = target.pokemon.TakeDamage(move, source.pokemon);
		target.HUD.UpdateHP();
		StartCoroutine(target.hitEffect());
		yield return ShowDamageDetails(details);

		if (details.fainted)
		{ 
			yield return dialog.TypeDialog($"{target.pname} Fainted"); 
			target.FaintAnimation();
			yield return BattleCamera.cam.ChangeState(camState.onAttack);
			yield return new WaitForSeconds(1f);

			CheckBattleEnd(target);
		}

		yield return BattleCamera.cam.ChangeState(camState.onAttack);
	}

	private void CheckBattleEnd(BattleUnit faintedP)
	{
		if (faintedP.isPlayer)
		{
			var nextPokemon = playerParty.GetPokemon();
			if (nextPokemon != null)
				menu.onPOKEMON();
			else 
				EXITstate(false);
		} 
		else 
			EXITstate(true);
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
