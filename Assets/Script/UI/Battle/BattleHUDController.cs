using UnityEngine;
using TMPro;

public class BattleHUDController : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI tname;
	[SerializeField] private TextMeshProUGUI lvl;
	[SerializeField] private HPUIController hpui;

	[SerializeField] private bool isPlayerPokemon;

	[SerializeField] private TextMeshProUGUI maxHPt;
	[SerializeField] private TextMeshProUGUI HPt;

	private PokemonClass pokemonm;

	public void SetData(PokemonClass pokemon)
	{
		pokemonm = pokemon;
		tname.text = pokemon.data.pname.ToUpper();
		lvl.text = pokemon.level.ToString();
		hpui.SetHP((float)pokemon.HP/pokemon.maxHp);

		if (isPlayerPokemon)
		{
			maxHPt.text = pokemon.maxHp.ToString();
			HPt.text = pokemon.HP.ToString();
		}
	}

	public void UpdateHP()
	{ 
		hpui.LerpHP((float)pokemonm.HP/pokemonm.maxHp);
		if (isPlayerPokemon) { HPt.text = pokemonm.HP.ToString(); }
	}
}								   								   
