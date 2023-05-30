using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
	public List<PokemonClass> wildPokemon;

	public PokemonClass GetRandomWildPokemon()
	{
		var wp = wildPokemon[Random.Range(0, wildPokemon.Count)];
		wp.Initialize();
		return wp;
	}
}					   
