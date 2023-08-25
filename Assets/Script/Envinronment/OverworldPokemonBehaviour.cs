using UnityEngine;

public class OverworldPokemonBehaviour : MonoBehaviour
{
	[SerializeField] private Transform triggerTransform;
	[SerializeField] private Transform transformTo;
	[SerializeField] private float radius = 2;
	[SerializeField] private bool flip;
	[SerializeField] private LayerMask triggerTo;

	private bool triggered;

	private void Start()
	{
		triggered = false;
		GetComponent<Animator>().SetBool("goRight", flip);
	}

	private void Update()
	{
		if (triggered)
			return;

		if (Physics.OverlapSphere(triggerTransform.position, radius, triggerTo).Length > 0)
		{
			GetComponent<Animator>().SetTrigger("t");
			Invoke("MovePosition", 0.25f);
			triggered = true;
		}
	}

	private void MovePosition()
	{
		LeanTween.move(this.gameObject, transformTo, 4f).setEaseOutCubic();
	}
}							 
