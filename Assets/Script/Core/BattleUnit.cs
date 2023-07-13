using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using CameraShake;

public class BattleUnit : MonoBehaviour
{
	[SerializeField] BounceShake.Params shakeParams;

	[SerializeField] private int level;
	[SerializeField] private bool isPlayerPokemon;
	[SerializeField] private BattleHUDController hud;
	[SerializeField] private VisualEffect vfx;

	public bool isPlayer { get { return isPlayerPokemon; } }
	public string pname { get; private set; }
	public PokemonClass pokemon { get; set; }
	public BattleHUDController HUD { get { return hud; } }

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

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
			CameraShaker.Shake(new BounceShake(shakeParams));
	}

	public void Setup(PokemonClass p)
	{
		this.pokemon = p;
		anim.runtimeAnimatorController = pokemon.data.animator;
		pname = pokemon.data.pname;

		if (isPlayerPokemon)
			anim.Play("back");
		else 
			anim.Play("front");

		hud.SetData(pokemon);
		shakeParams.freq = pokemon.data.weight; 
		shakeParams.numBounces = (int)shakeParams.freq/2;

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

		StartCoroutine(EnterAnimation());
	}

	public IEnumerator EnterAnimation()
	{
		float vfxSize = pokemon.data.inGameSize==32?0.1f:(pokemon.data.inGameSize==24?0.15f:0.35f);

		vfx.transform.position = transform.position + new Vector3(0, pokemon.data.baseSprite.bounds.size.y/2, 0);
		vfx.visualEffectAsset = GlobalVariable.instances.GetVisualAsset("PokeballOUT");
		vfx.SetVector3("Cpos", new Vector3(0, -vfx.transform.position.y, 0));
		vfx.SetFloat("Size", vfxSize);
		vfx.Play();

		transform.localScale = Vector3.zero;
		transform.position = new Vector3(transform.position.x, 1.25f, transform.position.z);
		LeanTween.scale(this.gameObject, new Vector3(1, 1, 1), 0.4f).setEaseOutQuart();

		yield return new WaitUntil(() => this.gameObject.transform.localScale == new Vector3(1, 1, 1));

		LeanTween.moveY(this.gameObject, 0, 0.5f).setEaseInQuart();
		yield return new WaitUntil(() => this.gameObject.transform.position.y <= 0.5f);
		if (pokemon.data.weight <= 50)
			yield break;

		CameraShaker.Shake(new BounceShake(shakeParams));
	}

	public void FaintAnimation()
	{
		LeanTween.scale(this.gameObject, new Vector3(0, 0, 0), 0.4f).setEaseInQuart();
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
