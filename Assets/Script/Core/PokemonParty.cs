using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PokemonParty : MonoBehaviour
{
	[SerializeField] List<PokemonClass> party;
	public List<PokemonClass> Party { get { return party; } }

	private void Start()
	{
		foreach(var p in party)
			p.Initialize();
	}

	public PokemonClass GetPokemon()
		{ return party.Where(x => x.HP > 0).FirstOrDefault(); }

	public void AddPokemon(PokemonClass pokemon)
	{
		if (party.Count < 6)
		{
			party.Add(pokemon);
		}
		else 
		{
			//add to PC;
		}
	}

	public void WildParty(PokemonClass pokemon)
	{
		if (party.Count > 0)
			party.Clear();

		party = new List<PokemonClass>();
		party.Add(pokemon);
	}
}							
