using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
	[SerializeField] private float speed = 10;
	[SerializeField] private float wanderCooldown = 5;
	[SerializeField] private int wanderRange = 5;
	[SerializeField] private List<Dialog> dialog;
	[SerializeField] private List<Vector3> movePattern;
	[SerializeField] private LayerMask colliderL;

	public bool canMove { get; set; }
	public bool battleReady  { get; set; }
	public bool isMoving { get { return onMove; } }

	private bool onWall;
	private bool onMove;
	private bool onDialog;

	private float timer;
	private int currentPattern;

	private Vector3 faceDirection;
	private Animator anim;
	private Coroutine coroutine;

	public void Interact(Vector3 otherPos)
	{
		onDialog = true;
		faceDirection = (otherPos - transform.position).normalized;
		anim.SetFloat("moveX", faceDirection.x);
		anim.SetFloat("moveY", faceDirection.z);

		if (battleReady)
			StartCoroutine(GetComponent<TrainerController>().TriggerTrainerBattle(this.transform.position)); else 
		if (!battleReady)
			OverworldDialogManager.d.ShowDialog(dialog, transform.position, ()=> { onDialog = false; });	
	}

	private void Start()
	{
		anim = GetComponentInChildren<Animator>();
		onMove = false;
		canMove = true;
		timer = wanderCooldown;
		currentPattern = 0;
	}

	private void OnEnable()
	{
		onDialog = false;
		coroutine = null;
	}

	public void MoveTo(Vector3 pos)
		=> coroutine = StartCoroutine(Move(pos));
	
	private void Update()
	{
		if (onDialog)
		{
			if (coroutine != null)
				StopCoroutine(coroutine); 
			return; 
		}

		CollisionCheck();
		if (onMove || !canMove)
			return;
		
		if (movePattern.Count <= 0)
			return;

		if (timer > 0)
			timer -= Time.deltaTime;

		if (timer <= 0)
		{
			if (coroutine == null)
				coroutine = StartCoroutine(Move(transform.position + movePattern[currentPattern]));
		}
	}

	private IEnumerator Move(Vector3 tPos)
	{
		if (tPos == Vector3.zero)
			{ ResetValue(); yield break; }
	
		faceDirection = (tPos - transform.position).normalized;

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
		currentPattern++;

		if (currentPattern >= movePattern.Count)
			currentPattern = 0;

		onMove = false;
		anim.Play("idle");
		coroutine = null;
		timer = wanderCooldown;
	}

	private void CollisionCheck()
	{
		onWall = Physics.OverlapSphere(transform.position + faceDirection.normalized/2f, 0.5f, colliderL).Length > 0;

		if (onWall)
		{
			if (coroutine != null)
			{ 
				StopCoroutine(coroutine);
				ResetValue();
			}
		}
		//Debug.DrawRay(transform.position, faceDirection.normalized* 1, Color.red);
	}
}							 
