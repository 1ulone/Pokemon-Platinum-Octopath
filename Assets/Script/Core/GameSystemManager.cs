using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameSystemManager : MonoBehaviour
{
	public static GameSystemManager i;

	[SerializeField] private List<GameObject> importantObject;
	[SerializeField] private PlayerController playerOverworld;

	private PokemonClass opponentPokemon;
	private PokemonParty playerParty;

	private List<GameObject> allObject;

	private void Awake()
		=> i = this;

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
			o.SetActive(true);
	}


	private void Load()
	{
		BattleSystem.instances.StartBattle(playerParty, opponentPokemon);
	}

	public void SetOpponentPokemon(PokemonClass p)
		=> opponentPokemon = p;

	public void InitiateBattle()
	{
		playerParty = playerOverworld.party;

		SceneManager.LoadScene("BattleCore", LoadSceneMode.Additive);
		ChangeStateToBattle();

		Invoke("Load", 0.5f);
	}
	
	public void ExitBattle(bool win)
	{
		SceneManager.UnloadSceneAsync("BattleCore");
		ChangeStateToOverworld();
	}
}								 
