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
}							
