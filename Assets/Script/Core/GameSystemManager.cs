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

	[SerializeField] private List<GameObject> importantObject;
	[SerializeField] private PlayerController playerOverworld;

	private PokemonParty opponentParty;
	private PokemonParty playerParty;

	private List<GameObject> allObject;

	private void Awake()
	{ 
		i = this;
		ConditionDatabase.Init();
	}

	private void ChangeStateToBattle()
	{
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
		BattleSystem.instances.StartBattle(playerParty, opponentParty);
		FindObjectOfType<BattleTransition>().BattleOpeningTransition();
	}

	public void SetOpponentPokemon(PokemonParty p)
		=> opponentParty = p;

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
		SceneManager.UnloadSceneAsync("BattleCore");
		SceneManager.UnloadSceneAsync("Battle-GrassField");
		ChangeStateToOverworld();
	}
}								 
