using System;
using UnityEngine;

public class TrainerTrigger : MonoBehaviour
{
	[SerializeField] private TrainerController con;	

	public void Trigger(Vector3 pos, Action playerAction)
	{
		StartCoroutine(con.TriggerTrainerBattle(pos, playerAction));
	}
}							  
