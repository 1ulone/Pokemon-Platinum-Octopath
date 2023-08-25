using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHUDController : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI tname;
	[SerializeField] private TextMeshProUGUI lvl;
	[SerializeField] private HPUIController hpui;
	[SerializeField] private HPUIController expui;
	[SerializeField] private Image statusIcon;

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
		statusIcon.enabled = false;

		if (isPlayerPokemon)
		{
			maxHPt.text = pokemon.maxHp.ToString();
			HPt.text = pokemon.HP.ToString();
		}

		UpdateStatus();
		pokemonm.onStatusChanged += UpdateStatus;
	}

	public void UpdateStatus()
	{
		if (pokemonm.status == null && statusIcon.enabled)
		{
			statusIcon.enabled = false;
		} else
		if (pokemonm.status != null && !statusIcon.enabled)
		{
			statusIcon.enabled = true;
			statusIcon.sprite = GlobalVariable.instances.GetStatusIcon(pokemonm.status.id);
		}
	}

	public void SetLevel()
	{
		lvl.text = $"{pokemonm.level.ToString()}";
	}

	public void UpdateHP()
	{ 
		if (pokemonm.HpChanged)
		{
			hpui.LerpHP((float)pokemonm.HP/pokemonm.maxHp);
			if (isPlayerPokemon) { HPt.text = pokemonm.HP.ToString(); }

			pokemonm.HpChanged = false;
		}
	}

	public IEnumerator SetEXP(bool doLerp)
	{
		if (expui == null)
			yield break;

		if (doLerp)
			expui.LerpHPUntil(GetNormalizedEXP());
		else 
			expui.SetHP(GetNormalizedEXP());

		yield return new WaitUntil(()=> expui.doneLerp==true || doLerp==false);
		expui.doneLerp = false;
	}

	//ADD EXP IMAGE;
	public float GetNormalizedEXP()
	{
		int currentLevelExp = pokemonm.data.GetEXPforLevel(pokemonm.level);
		int nextLevelExp = pokemonm.data.GetEXPforLevel(pokemonm.level+1);

		float pp =(float)(pokemonm.exp-currentLevelExp)/(nextLevelExp-currentLevelExp);
		Debug.Log(pp);

		return Mathf.Clamp01(pp);
	}

}								   								   
