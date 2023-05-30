using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyUI : MonoBehaviour
{
	[SerializeField] private List<PartyMemberUI> member;

	[Header("Description UI")]
	[SerializeField] private TextMeshProUGUI abilityDesc;
	[SerializeField] private Image type1;
	[SerializeField] private Image type2;
	[SerializeField] private MoveUI move;
	[SerializeField] private CanvasGroup info;

	private PokemonParty playerParty;

	public void InitializeUI()
	{
		info.alpha = 0;
		playerParty = BattleSystem.instances.PlayerParty;

		foreach(PartyMemberUI m in member)
			m.gameObject.SetActive(false);

		for (int i = 0; i < playerParty.Party.Count; i++)
		{
			PokemonClass p = playerParty.Party[i];
			member[i].gameObject.SetActive(true);
			member[i].pokemon = p;

			member[i].tname.text = p.data.pname;
			member[i].level.text = p.level.ToString();
			member[i].hptext.text = $"{p.HP.ToString()}/{p.maxHp.ToString()}";

			member[i].status.enabled = false;
			member[i].icon.sprite = p.data.icon;

			member[i].health.fillAmount = (float)p.HP/p.maxHp;
		}
	}

	public void ExitUI()
	{
		info.alpha = 0;
	}

	public void UpdateUI()
	{
		for (int i = 0; i < playerParty.Party.Count; i++)
		{
			//Update Status UI;
			PokemonClass p = playerParty.Party[i];
			member[i].hptext.text = $"{p.HP.ToString()}/{p.maxHp.ToString()}";
			member[i].health.fillAmount = (float)p.HP/p.maxHp;
		}
	}

	public void SetDescriptionUI(PokemonClass pk)
	{
		if (info.alpha == 0)
			LeanTween.value(info.gameObject, 0, 1, 0.25f).setOnUpdate((float x) => { info.alpha = x; } );

		abilityDesc.text = "";
		type1.sprite = GlobalVariable.instances.GetTypeIcon(pk.data.type1);
		if (pk.data.type2 != PokemonType.none) 
		{ 
			type2.enabled = true;
			type2.sprite = GlobalVariable.instances.GetTypeIcon(pk.data.type2); 
		} else { type2.enabled = false; }

		move.Setup(pk.moves);
	}
}
