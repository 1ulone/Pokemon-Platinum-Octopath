using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
	[SerializeField] private float speed = 10;
	[SerializeField] private float wanderCooldown = 5;
	[SerializeField] private int wanderRange = 5;
	[SerializeField] private List<Dialog> dialog;
	[SerializeField] private LayerMask colliderL;

	private bool onWall;
	private bool onMove;
	private float timer;

	private Vector3 faceDirection;
	private Animator anim;
	private Coroutine coroutine;

	public void Interact()
	{
		OverworldDialogManager.d.ShowDialog(dialog, transform.position);
	}

	private void Start()
	{
		anim = GetComponentInChildren<Animator>();
		coroutine = null;
		onMove = false;
		timer = wanderCooldown;
	}
	
	private void Update()
	{
		if (onMove)
			return;
		
		if (timer > 0)
			timer -= Time.deltaTime;

		if (timer <= 0)
		{
			int xTo = Random.Range(-wanderRange, wanderRange);
			int yTo = Random.Range(-wanderRange, wanderRange);
			Vector3 posTo = new Vector3(xTo>yTo?xTo:0, 0, yTo>xTo?yTo:0);
			if (coroutine == null)
				coroutine = StartCoroutine(Move(transform.position + posTo));
		}
	}

	private IEnumerator Move(Vector3 tPos)
	{
		if (tPos == Vector3.zero)
			{ ResetValue(); yield break; }
	
		faceDirection = (tPos - transform.position).normalized;
		CollisionCheck();

		if (onWall)
			{ ResetValue(); yield break; }

		onMove = true;
		anim.Play("walk");
		anim.SetFloat("moveX", faceDirection.x);
		anim.SetFloat("moveY", faceDirection.z);

		while ((tPos - transform.position).sqrMagnitude > Mathf.Epsilon)
		{
			transform.position = Vector3.MoveTowards(transform.position, tPos, speed* Time.deltaTime);
			yield return null;
		}

		transform.position = tPos;
		ResetValue();
	}

	private void ResetValue()
	{
		onMove = false;
		anim.Play("idle");
		coroutine = null;
		timer = wanderCooldown;

	}

	private void CollisionCheck()
	{
		onWall = Physics.OverlapSphere(transform.position + faceDirection.normalized/2, 0.5f, colliderL).Length > 0;
		//Debug.DrawRay(transform.position, faceDirection.normalized* 1, Color.red);
	}
}							 
