using System.Collections.Generic;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
	[SerializeField] List<PokemonClass> party;

	private void Start()
	{
		foreach(var p in party)
			p.Initialize();
	}

	public PokemonClass GetPokemon()
	{
		return party[0];
	}
}							
