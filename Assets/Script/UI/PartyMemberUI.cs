using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyMemberUI : MonoBehaviour
{	
	[HideInInspector] public PokemonClass pokemon;

	[SerializeField] public TextMeshProUGUI tname;
	[SerializeField] public TextMeshProUGUI level;
	[SerializeField] public TextMeshProUGUI hptext;
	
	[SerializeField] public Image status;
	[SerializeField] public Image icon;
	[SerializeField] public Image health;

	public void OnSelected()
	{
		FindObjectOfType<PartyUI>().SetDescriptionUI(pokemon);
	}
}							 
