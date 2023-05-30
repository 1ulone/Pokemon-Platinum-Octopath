using System.Collections;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
	[SerializeField] private int level;
	[SerializeField] private bool isPlayerPokemon;

	public PokemonClass pokemon { get; set; }
	public string pname { get; private set; }
	private Animator anim;
	private SpriteRenderer sprite;
	private Light emmision;

	private void Start()
	{
		anim = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer>();
		emmision = GetComponentInChildren<Light>();

		emmision.enabled = false;
	}

	public void Setup(PokemonClass pokemon)
	{
		this.pokemon = pokemon;
		anim.runtimeAnimatorController = pokemon.data.animator;
		pname = pokemon.data.pname;

		if (isPlayerPokemon)
			anim.Play("back");
		else 
			anim.Play("front");

		//Check for Emmision
		if (pokemon.data.emmitsLight)
		{
			sprite.material = GlobalVariable.instances.GetMaterial(materialType.Glow);
			emmision.enabled = true;
			emmision.intensity = pokemon.data.lightIntensity;
			emmision.color = pokemon.data.lightColor;
			emmision.transform.localPosition = new Vector3(pokemon.data.lightPosition.x*(isPlayerPokemon?1:-1), pokemon.data.lightPosition.y, pokemon.data.lightPosition.z*(isPlayerPokemon?1:-1));
			sprite.material.color = pokemon.data.lightColor;
		} 
		else 
		{ 
			sprite.material = GlobalVariable.instances.GetMaterial(materialType.Default);
			emmision.enabled = false; 
		}
	}

	public IEnumerator hitEffect()
	{
		Vector3 origPos = this.gameObject.transform.position;
		Vector3 knockPos = origPos + new Vector3(isPlayerPokemon?-0.25f:0.25f, 0, isPlayerPokemon?-0.5f:0.5f);
		LeanTween.move(this.gameObject, knockPos, 0.25f).setEaseOutQuad();
		HitStop.h.StartHitStop(0.5f, 0.1f);

		int i = 0;
		while(i < 5)
		{
			sprite.enabled = false;
			yield return new WaitForSecondsRealtime(0.15f);
			sprite.enabled = true;
			yield return new WaitForSecondsRealtime(0.15f);
			i++;
		}	

		yield return new WaitForSeconds(0.5f);
		LeanTween.move(this.gameObject, origPos, 0.5f).setEaseInQuad();

	}
}						  
