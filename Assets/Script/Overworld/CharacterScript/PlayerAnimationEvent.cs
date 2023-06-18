using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
	private PlayerController con;

	private void Start()
		=> con = GetComponentInParent<PlayerController>();

	private void doEvents()
	{
		con.CheckForEncounter();
	}

	private void FootstepMark()
	{
		con.CreateFootstep();
	}
}									
