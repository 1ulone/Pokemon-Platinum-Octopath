using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
	[SerializeField] private List<Dialog> dialog;
	
	public void Interact()
	{
		OverworldDialogManager.d.ShowDialog(dialog, transform.position);
	}
}							 
