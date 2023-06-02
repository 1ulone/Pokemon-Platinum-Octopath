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
		ChooseFirstTurn();
	}	

	private void EXITstate(bool w)
	{
		state = state.exit;
		playerParty.Party.ForEach(p => p.OnBattleEnd());
		GameSystemManager.i.ExitBattle(w);
	}

	private void ChooseFirstTurn()
	{
		if (playerUnit.pokemon.speed >= opponentUnit.pokemon.speed)
			StartCoroutine(PlayerTurn());
		else
			StartCoroutine(OpponentTurn());
	}

	public IEnumerator SwitchPokemon(PokemonClass newPokemon)
	{
		yield return BattleCamera.cam.ChangeState(camState.onAttack);

		bool currentPokemonFainted = true;
		if (playerUnit.pokemon.HP > 0)
		{
			currentPokemonFainted = false;
			yield return dialog.TypeDialog($"Come back {playerUnit.pname}");
			playerUnit.FaintAnimation();
			yield return new WaitForSeconds(1f);
		}

		playerUnit.Setup(newPokemon);
		moveOptions.Setup(newPokemon.moves);
		yield return dialog.TypeDialog($"Go {playerUnit.pname}!");

		if (currentPokemonFainted)
			ChooseFirstTurn();
		else
			StartCoroutine(OpponentTurn());
	}

	public void SetPlayerMove(MoveClass move)
		=> playerMove = move;

	private IEnumerator PlayerTurn()
	{
		state = state.battle;

		moveOptions.UpdateUI();

		yield return ExecuteMove(playerUnit, opponentUnit, playerMove);

		if (state == state.battle)
			StartCoroutine(OpponentTurn());
	}

	private IEnumerator OpponentTurn()
	{
		opponentMove = opponentUnit.pokemon.GetRandomMove();

		yield return ExecuteMove(opponentUnit, playerUnit, opponentMove); 

		if (state == state.battle)
			ACTIONstate();
	}

	private IEnumerator ExecuteMove(BattleUnit source, BattleUnit target, MoveClass move)
	{
		bool canExecute = source.pokemon.OnStartTurn();
		if (!canExecute)
		{
			yield return StatusChangeDetails(source.pokemon);
			source.HUD.UpdateHP();
			yield break;
		}
		yield return StatusChangeDetails(source.pokemon);

		yield return dialog.TypeDialog($"{source.pname} use {move.data.mname}");
		yield return BattleCamera.cam.ChangeState(camState.onAttack);

		move.pp--;
		if (move.data.category == MoveCategory.Status)
		{
			yield return ExecuteMoveEffects(move, source.pokemon, target.pokemon, source.isPlayer?true:false);
		}
		else
		{
			yield return BattleCamera.cam.ChangeState(source.isPlayer?camState.onZoomEnemy:camState.onZoomPlayer);
			DamageDetails details = target.pokemon.TakeDamage(move, source.pokemon);
			target.HUD.UpdateHP();
			StartCoroutine(target.hitEffect());
			yield return ShowDamageDetails(details);
		}

		if (target.pokemon.HP <= 0)
		{ 
			yield return dialog.TypeDialog($"{target.pname} Fainted"); 
			target.FaintAnimation();
			yield return BattleCamera.cam.ChangeState(camState.onAttack);
			yield return new WaitForSeconds(1f);

			CheckBattleEnd(target);
		}

		source.pokemon.OnEndTurn();
		yield return StatusChangeDetails(source.pokemon);
		source.HUD.UpdateHP();

		if (source.pokemon.HP <= 0)
		{ 
			yield return dialog.TypeDialog($"{source.pname} Fainted"); 
			source.FaintAnimation();
			yield return BattleCamera.cam.ChangeState(camState.onAttack);
			yield return new WaitForSeconds(1f);

			CheckBattleEnd(source);
		}

		yield return BattleCamera.cam.ChangeState(camState.onAttack);
	}

	private IEnumerator ExecuteMoveEffects(MoveClass move, PokemonClass source, PokemonClass target, bool isP)
	{
		var effects = move.data.effects;
		if (effects.boosts != null) //BOOST STAT
		{
			if (move.data.target == MoveTarget.Self)
			{
				source.ApplyBoosts(effects.boosts); 
				yield return BattleCamera.cam.ChangeState(isP?camState.onZoomPlayer:camState.onZoomEnemy);
			} else
			if (move.data.target == MoveTarget.Foe)
			{
				target.ApplyBoosts(effects.boosts);
				yield return BattleCamera.cam.ChangeState(isP?camState.onZoomEnemy:camState.onZoomPlayer);
			}
		}

		if (effects.status != ConditionID.none) //STATUS EFFECT
		{
			target.SetStatus(effects.status);
		}

		if (effects.volatileStatus != ConditionID.none) //STATUS EFFECT
		{
			target.SetVolatileStatus(effects.volatileStatus);
		}

		yield return StatusChangeDetails(source);
		yield return StatusChangeDetails(target);

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

	private bool CheckIfMoveHit(MoveClass move, PokemonClass source, PokemonClass target)
	{
		float moveAcc = move.data.accuracy;

		int accuracy = target.statBoosts[stat.accuracy];
		int evasion = target.statBoosts[stat.accuracy];

		return UnityEngine.Random.Range(1, 101) <= moveAcc;
	}

	private IEnumerator StatusChangeDetails(PokemonClass p)
	{
		while (p.statusChange.Count > 0)
		{
			var message = p.statusChange.Dequeue();
			yield return dialog.TypeDialog(message);
		}
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
