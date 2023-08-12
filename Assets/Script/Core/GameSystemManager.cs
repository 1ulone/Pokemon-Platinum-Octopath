using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum BattleAgainst
{
	wild,
	trainer,
	boss
}

public class GameSystemManager : MonoBehaviour
{
	public static GameSystemManager i;
	public static bool TransitionToBattle;

	[SerializeField] private List<GameObject> importantObject;
	[SerializeField] private PlayerController playerOverworld;

	private PokemonParty opponentParty;
	private PokemonParty playerParty;

	private List<GameObject> allObject;
	private List<opponentData> opponentDatas;

	private void Awake()
	{ 
		i = this;
		ConditionDatabase.Init();
		opponentDatas = new List<opponentData>();
	}

	private void ChangeStateToBattle()
	{
		importantObject.Add(FindObjectOfType<Pool>().gameObject);
		if (allObject == null)
		{
			GameObject[] allObj = FindObjectsOfType<GameObject>();
			allObject = new List<GameObject>(allObj);

			foreach(GameObject oo in importantObject)
				allObject.Remove(oo);
		}

		foreach(GameObject o in allObject)
			o.SetActive(false);
	}

	private void ChangeStateToOverworld()
	{
		foreach(GameObject o in allObject)
			if (o != null)
				o.SetActive(true);
	}

	private void Load()
	{
		foreach(opponentData d in opponentDatas)
			BattleSystem.instances.SetTrainerData(d);
		BattleSystem.instances.StartBattle(playerParty, opponentParty);
		FindObjectOfType<BattleTransition>().BattleOpeningTransition();
	}

	public void SetOpponentPokemon(PokemonParty p)
		=> opponentParty = p;

	public void SetOpponentData(string tag, RuntimeAnimatorController animator)
	{
		opponentData od = new opponentData()
		{
			tname = tag,
			anim = animator
		};

		opponentDatas.Add(od);
	}

	public void InitiateBattle(BattleAgainst ba)
	{
		playerParty = playerOverworld.party;

		SceneManager.LoadScene("Battle-GrassField", LoadSceneMode.Additive);
		SceneManager.LoadScene("BattleCore", LoadSceneMode.Additive);
		ChangeStateToBattle();

		Invoke("Load", 0.5f);
		BattleSystem.against = ba;
	}
	
	public void ExitBattle(bool win)
	{
		opponentDatas.Clear();
		SceneManager.UnloadSceneAsync("BattleCore");
		SceneManager.UnloadSceneAsync("Battle-GrassField");
		TransitionToBattle = false;
		ChangeStateToOverworld();
	}
}								 
