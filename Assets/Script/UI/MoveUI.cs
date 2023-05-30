using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoveUI : MonoBehaviour
{
	[SerializeField] private List<MoveComponent> move;
	private List<MoveClass> movec;
	
	private void ResetMoveUI()
	{	
		foreach(MoveComponent m in move)
		{
			m.img.sprite = GlobalVariable.instances.GetMoveSprite(PokemonType.none);
			m.tag.text = "";
			m.pp.text = "";
			m.ppmax.text = "";
		}

	}

	public void Setup(List<MoveClass> m)
	{
		ResetMoveUI();

		movec = m;
		for (int i=0; i<m.Count; i++)
		{
			move[i].img.sprite = GlobalVariable.instances.GetMoveSprite(m[i].data.type);
			move[i].tag.text = m[i].data.mname;
			move[i].pp.text = m[i].pp.ToString();
			move[i].ppmax.text = m[i].data.pp.ToString();
			move[i].move = m[i];
		}
	}

	public void UpdateUI()
	{
		for (int i=0; i<movec.Count; i++)
			move[i].pp.text = movec[i].pp.ToString();
	}

	public void OnPressed(int i)
	{
		GetComponent<BattleMenu>().onFIGHT();
		GetComponent<BattleMenu>().toggleMenu(false);
		FindObjectOfType<BattleSystem>().SetPlayerMove(move[i].move);
		StartCoroutine(FindObjectOfType<BattleSystem>().BATTLEstate());
	}
}					  

[System.Serializable]
public class MoveComponent
{
	[SerializeField] public Image img;
	[SerializeField] public TextMeshProUGUI tag;
	[SerializeField] public TextMeshProUGUI pp;
	[SerializeField] public TextMeshProUGUI ppmax;
	[HideInInspector] public MoveClass move;
}
