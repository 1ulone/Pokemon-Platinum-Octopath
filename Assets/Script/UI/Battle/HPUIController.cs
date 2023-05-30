using UnityEngine;
using UnityEngine.UI;

public class HPUIController : MonoBehaviour
{
	[SerializeField] public Image bar;

	public void SetHP(float normHP)
		=> bar.fillAmount = normHP;

	public void LerpHP(float normHP)
		=> LeanTween.value(gameObject, bar.fillAmount, normHP, 1f).setOnUpdate((float x)=> { bar.fillAmount = x; });
}							  
