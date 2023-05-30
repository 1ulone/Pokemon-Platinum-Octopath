using UnityEngine;
using UnityEngine.UI;

public class BattleMenu : MonoBehaviour
{
	[SerializeField] private RectTransform menu;
	[SerializeField] private RectTransform moveMenu;
	[SerializeField] private RectTransform pkmnMenu;
	[SerializeField] private RectTransform hp;

	private bool onFight, onPkmn;

	private void Start()
	{
		toggleMenu(false);
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

	public void toggleMenu(bool b)
	{
		if (b)
		{
			LeanTween.move(menu, Vector3.zero, 0.5f).setEaseInCubic();
			LeanTween.move(hp, new Vector3(0, 112, 0), 0.5f).setEaseInCubic();
		}
		else 
		{
			LeanTween.move(menu, new Vector3(0, -100, 0), 0.5f).setEaseInCubic();
			LeanTween.move(hp, new Vector3(0, 36, 0), 0.5f).setEaseInCubic();
		}
	}

	public void onFIGHT()
	{
		if (!onFight)
			LeanTween.move(moveMenu, new Vector3(0, 26, 0), 0.5f).setEaseInCubic();
		else 
			LeanTween.move(moveMenu, new Vector3(0, -100, 0), 0.5f).setEaseInCubic();

		onFight = !onFight;
	}

	public void onPOKEMON()
	{
		CanvasGroup a = pkmnMenu.GetComponent<CanvasGroup>();

		if (!onPkmn)
		{
			pkmnMenu.localPosition = new Vector2(0, 0);
			pkmnMenu.GetComponent<PartyUI>().InitializeUI();
			LeanTween.value(pkmnMenu.gameObject, 0, 1, 0.2f).setOnUpdate((float x) => { a.alpha = x; } );
		} else {
			LeanTween.value(pkmnMenu.gameObject, 1, 0, 0.2f).setOnUpdate((float x) => { a.alpha = x; } ).setOnComplete(()=> { pkmnMenu.localPosition = new Vector2(0, 800); pkmnMenu.GetComponent<PartyUI>().ExitUI(); });
		}

		onPkmn = !onPkmn;
	}
}						  
