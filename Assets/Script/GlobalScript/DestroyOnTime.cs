using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
	[SerializeField] private string Tag;
	[SerializeField] private float timer = 1;
	
	private void OnEnable()
	{ 
		Invoke("DisableObject", timer);
		GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, 1);
	}

	private void OnDisable()
		=> CancelInvoke("DisableObject");

	private void DisableObject()
	{
		LeanTween.value(1, 0, 1f).setOnUpdate((float x)=> 
				{
					GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", x);
				}).setOnComplete(() => 
				{
				if (Tag == "")
					Destroy(this.gameObject);
				else
					Pool.i.DestroyObject(Tag, this.gameObject);
				});
	}
}
