using UnityEngine;
using UnityEngine.UI;

public class BattleMenu : MonoBehaviour
{
	[SerializeField] private RectTransform menu;
	[SerializeField] private RectTransform moveMenu;
	[SerializeField] private RectTransform hp;
	private bool onFight;

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
}						  
