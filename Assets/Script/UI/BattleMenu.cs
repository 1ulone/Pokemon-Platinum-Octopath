using UnityEngine;
using UnityEngine.InputSystem;

public class BattleMenu : MonoBehaviour
{
	[SerializeField] private RectTransform menu;
	[SerializeField] private RectTransform moveMenu;
	[SerializeField] private RectTransform pkmnMenu;
	[SerializeField] private RectTransform php;
	[SerializeField] private RectTransform ohp;

	private bool onFight, onPkmn;
	private MoveUI mui;

	private void Start()
	{
		toggleMenu(false);
		mui = GetComponent<MoveUI>();
	}

	private void OnEnable()
	{
		LeanTween.move(php, new Vector3(300, 36, 0), 0);
		LeanTween.move(ohp, new Vector3(-300, -40, 0), 0);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
		{
			if (onFight)
				onFIGHT();

			if (onPkmn)
				onPOKEMON();
		}
	}

	public void InitiatePlayerUI()
		=> LeanTween.move(php, new Vector3(0, 36, 0), 0.5f).setEaseInCubic();

	public void InitiateOpponentUI()
		=> LeanTween.move(ohp, new Vector3(0, -40, 0), 0.5f).setEaseInCubic();

	public void toggleMenu(bool b)
	{
		if (b)
		{
			LeanTween.move(menu, Vector3.zero, 0.5f).setEaseInCubic();
			LeanTween.move(php, new Vector3(0, 112, 0), 0.5f).setEaseInCubic();
		}
		else 
		{
			LeanTween.move(menu, new Vector3(0, -100, 0), 0.5f).setEaseInCubic();
			LeanTween.move(php, new Vector3(0, 36, 0), 0.5f).setEaseInCubic();
		}
	}

	public void onFIGHT()
	{

		if (!onFight)
			{ LeanTween.move(moveMenu, new Vector3(0, 26, 0), 0.5f).setEaseInCubic(); mui.UpdateUI(); }
		else 
			LeanTween.move(moveMenu, new Vector3(0, -100, 0), 0.5f).setEaseInCubic();

		onFight = !onFight;
	}

	public void onPOKEMON()
	{
		CanvasGroup a = pkmnMenu.GetComponent<CanvasGroup>();

		if (!onPkmn)
		{
			StartCoroutine(FindObjectOfType<BattleDialog>().TypeDialog("Which pokemon will you switch to ?"));
			pkmnMenu.localPosition = new Vector2(0, 0);

			pkmnMenu.GetComponent<PartyUI>().InitializeUI();
			LeanTween.value(pkmnMenu.gameObject, 0, 1, 0.2f).setOnUpdate((float x) => { a.alpha = x; } );
		} else {
			LeanTween.value(pkmnMenu.gameObject, 1, 0, 0.2f).setOnUpdate((float x) => { a.alpha = x; } )
															.setOnComplete(()=> { pkmnMenu.GetComponent<PartyUI>().ExitUI(); });
		}

		onPkmn = !onPkmn;
	}

	public void onRUN()
	{
		toggleMenu(false);
		BattleSystem.instances.RUNstate();
	}

	public void InputFight(InputAction.CallbackContext context)
		=> onFIGHT();

	public void InputPokemon(InputAction.CallbackContext context)
		=> onPOKEMON();

	public void InputBag(InputAction.CallbackContext context) {}

	public void InputRun(InputAction.CallbackContext context) {}
}						  
