using System.Collections;
using UnityEngine;

public enum state
{
	action,
	onturn,
	busy,
	exit
}

public enum BattleAction 
{
	fight,
	switchP,
	useItem,
	run
}

public class BattleSystem : MonoBehaviour
{
	public static BattleSystem instances;
	public static BattleAgainst against;

	[Header("Main Component")]
	[SerializeField] BattleUnit playerUnit;
	[SerializeField] BattleUnit opponentUnit;

	[Header("UI Component")]
	[SerializeField] BattleDialog dialog;

	[Header("Player UI Component")]
	[SerializeField] BattleMenu menu;
	[SerializeField] MoveUI moveOptions;
	[SerializeField] PartyUI partyMemberUI; 

	[Header("Other")]
	[SerializeField] GameObject opponentTrainer;

	public PokemonParty PlayerParty { get { return playerParty; } }
	public PokemonClass PlayerCurrentPokemon { get { return playerUnit.pokemon; } }

	private state state;
	private state? prevState;
	private MoveClass playerMove;
	private MoveClass opponentMove;

	private PokemonParty playerParty;
	private PokemonParty opponentParty;
//	private PokemonClass opponentPokemon;

	private PokemonClass selectedPokemon;

	private void Awake()
	{
		instances = this;
	}

	public void StartBattle(PokemonParty pparty, PokemonParty oppoP)
	{
		this.playerParty = pparty;
		this.opponentParty = oppoP;

		partyMemberUI.InitializeUI();

		StartCoroutine(SetupBattle());
	}

	public IEnumerator SetupBattle()
	{
		var pplayer = playerParty.GetPokemon();
		var poppone = opponentParty.GetPokemon();

		switch(against)
		{
			case BattleAgainst.wild:////WILD BATTLE
			{
				opponentTrainer.SetActive(false);
				BattleCamera.cam.SetBattleCamera(pplayer.data.inGameSize, poppone.data.inGameSize);

				opponentUnit.Setup(poppone);
				yield return dialog.TypeDialog($"a wild {opponentUnit.pname} appeared!");
				yield return new WaitForSeconds(1f);

			} break;

			case BattleAgainst.trainer:////TRAINER BATTLE
			{
				opponentTrainer.SetActive(true);
				BattleCamera.cam.SetBattleCamera(pplayer.data.inGameSize, poppone.data.inGameSize);
				yield return dialog.TypeDialog($"PKMN Trainer Dawn wants to battle!");

				opponentUnit.Setup(poppone);
				yield return dialog.TypeDialog($"Dawn sent out {opponentUnit.pname}!");
				yield return new WaitForSeconds(1f);

			} break;
		}
		
		playerUnit.Setup(playerParty.GetPokemon());
		moveOptions.Setup(playerUnit.pokemon.moves);
		yield return dialog.TypeDialog($"{playerUnit.pname} go!");
		ACTIONstate();
	}

	public void ACTIONstate()
	{
		state = state.action;
		StartCoroutine(dialog.TypeDialog("What will you do ?"));
		StartCoroutine(BattleCamera.cam.ChangeState(camState.onIdle));
		menu.toggleMenu(true);
	}

	public void BATTLEstate(MoveClass selectedMove)
	{
		playerMove = selectedMove;
		StartCoroutine(ExecuteTurn(BattleAction.fight));
	}	

	public void SWITCHPOKEMONstate(PokemonClass p)
	{ 
		prevState = state;
		menu.toggleMenu(false);

		if (prevState == state.action)
		{
			prevState = null;
			selectedPokemon = p;
			StartCoroutine(ExecuteTurn(BattleAction.switchP));
		}
		else 
		{
			state = state.busy;
			StartCoroutine(SwitchPokemon(p));
		}
	}

	private void EXITstate(bool w)
	{
		state = state.exit;
		playerParty.Party.ForEach(p => p.OnBattleEnd());
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
		BattleCamera.cam.SetBattleCamera(playerUnit.pokemon.data.inGameSize, opponentUnit.pokemon.data.inGameSize);
		yield return dialog.TypeDialog($"Go {playerUnit.pname}!");
 
		state = state.onturn;
	}


	private IEnumerator ExecuteTurn(BattleAction playerAction)
	{
		state = state.onturn; 

		//PLAYER ACTION TO BATTTLE!!!
		if (playerAction == BattleAction.fight)
		{
			playerUnit.pokemon.currentMove = playerMove;
			opponentUnit.pokemon.currentMove = opponentUnit.pokemon.GetRandomMove();

			int ppriority = playerUnit.pokemon.currentMove.data.priority;
			int opriority = opponentUnit.pokemon.currentMove.data.priority;

			//CHECK WHO GOES FIRST
			bool playerFirst = true;
			if (opriority > ppriority)
				playerFirst = false;
			else if (opriority == ppriority)
				playerFirst = playerUnit.pokemon.speed >= opponentUnit.pokemon.speed;

			var firstUnit = playerFirst?playerUnit:opponentUnit;
			var secondUnit = playerFirst?opponentUnit:playerUnit;

			var secondPokemon = secondUnit.pokemon;

			yield return BattleCamera.cam.ChangeState(camState.onAttack);

	
			//FIRST ATTACK
			yield return ExecuteMove(firstUnit, secondUnit, firstUnit.pokemon.currentMove);
			yield return ExecuteOnEnd(firstUnit);
			if (state == state.exit)
				yield break;

			if (secondPokemon.HP > 0)
			{
				//SECOND ATTACK
				yield return ExecuteMove(secondUnit, firstUnit, secondUnit.pokemon.currentMove);
				yield return ExecuteOnEnd(secondUnit);
				if (state == state.exit) yield break;
			}
		} 
		else  
		{		
			//PLAYER ACTION TO SWITCHING POKEMON
			if (playerAction == BattleAction.switchP)
			{
				state = state.busy;
				var np = selectedPokemon;
				yield return SwitchPokemon(np);
			}

			opponentUnit.pokemon.currentMove = opponentUnit.pokemon.GetRandomMove();
			yield return ExecuteMove(opponentUnit, playerUnit, opponentUnit.pokemon.currentMove);
			yield return ExecuteOnEnd(opponentUnit);
			if (state == state.exit) yield break;

		}

		if (state != state.exit)
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
		
		if (CheckIfMoveHit(move, source.pokemon, target.pokemon))
		{

			yield return BattleCamera.cam.ChangeState(camState.onAttack);

			move.pp--;
			if (move.data.category == MoveCategory.Status)
			{
				yield return ExecuteMoveEffects(move.data.effects, source.pokemon, target.pokemon, move.data.target, source.isPlayer?true:false);
			}
			else
			{
				yield return BattleCamera.cam.ChangeState(source.isPlayer?camState.onZoomEnemy:camState.onZoomPlayer);
				DamageDetails details = target.pokemon.TakeDamage(move, source.pokemon);
				target.HUD.UpdateHP();
				StartCoroutine(target.hitEffect());
				yield return ShowDamageDetails(details);
			}
	
			if (move.data.secondaryEffects != null && move.data.secondaryEffects.Count > 0 && target.pokemon.HP > 0)
			{
				foreach (var s in move.data.secondaryEffects)
				{
					var rnd = UnityEngine.Random.Range(1, 101);
					if (rnd <= s.chance)
						yield return ExecuteMoveEffects(s, source.pokemon, target.pokemon, s.target, source.isPlayer?true:false);
				}
			}

			if (target.pokemon.HP <= 0)
			{ 
				yield return dialog.TypeDialog($"{target.pname} Fainted"); 
				target.FaintAnimation();
				yield return BattleCamera.cam.ChangeState(camState.onAttack);
				yield return new WaitForSeconds(1f);
	
				CheckBattleEnd(target);
			}

		}
		else 
		{
			yield return dialog.TypeDialog($"{source.pname} attacks missed!");
		}
	}

	private IEnumerator ExecuteMoveEffects(MoveEffects move, PokemonClass source, PokemonClass target, MoveTarget mt, bool isP)
	{
		var effects = move;
		if (effects.boosts != null) //BOOST STAT
		{
			if (mt == MoveTarget.Self)
			{
				source.ApplyBoosts(effects.boosts); 
				yield return BattleCamera.cam.ChangeState(isP?camState.onZoomPlayer:camState.onZoomEnemy);
			} else
			if (mt == MoveTarget.Foe)
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

	private IEnumerator ExecuteOnEnd(BattleUnit source)
	{
		if (state == state.exit)
			yield break;

		yield return new WaitUntil(() => state == state.onturn);

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

	private bool CheckIfMoveHit(MoveClass move, PokemonClass source, PokemonClass target)
	{
		if (move.data.alwaysHit)
			return true;

		float moveAcc = move.data.accuracy;

		int accuracy = target.statBoosts[stat.accuracy];
		int evasion = target.statBoosts[stat.accuracy];

		var boostsVal = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

		if (accuracy > 0)
			moveAcc *= boostsVal[accuracy]; 
		else 
			moveAcc /= boostsVal[-accuracy];

		if (evasion > 0)
			moveAcc /= boostsVal[evasion]; 
		else 
			moveAcc *= boostsVal[-evasion];

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
