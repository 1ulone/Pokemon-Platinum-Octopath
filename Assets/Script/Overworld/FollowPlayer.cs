using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
	[SerializeField] private bool permanentFollow;
	[SerializeField] private bool permanentAnimate;
	[SerializeField] private int steps;
	[SerializeField] private float targetDist;
	public bool doFollow { get; set; }

	private PlayerController player;
	private Animator anim;
	private List<Vector3> record = new List<Vector3>();
	private Vector3 npos;

	private void Start()
	{
		player = FindObjectOfType<PlayerController>();
		anim = GetComponent<Animator>();

		doFollow = permanentFollow;
	}

	private void Update()
	{
		if (!doFollow)
			return;
		
		if (player.inputDirection != Vector2.zero)
		{
			record.Add(player.transform.position);
		} else {
			if (!permanentAnimate)
				anim.speed = 0;
		}

		if (record.Count > steps)
		{
			npos = new Vector3(record[record.Count-steps].x, 0, record[record.Count-steps].z);
			Vector3 dir = (npos - transform.position).normalized;
			if (Vector3.Distance(npos, transform.position) > targetDist)
			{
				anim.speed = 1;
				anim.SetFloat("moveX", dir.x);
				anim.SetFloat("moveY", dir.z);

				if (this.transform.position != record[0])
				{	
					LeanTween.move(this.gameObject, npos, 0.1f);
				} else {
					record.Remove(record[0]);
					LeanTween.move(this.gameObject, npos, 0.1f);
				}

			}
			
//				this.transform.position = Vector3.MoveTowards(transform.position, npos, player.speed);
			/*
			var maxDist = player.speed * Time.deltaTime;
			var oldPos  = transform.localPosition;
			var newPos  = Vector3.MoveTowards(transform.localPosition, npos, maxDist*2.5f);
			var actDist = Vector3.Distance(newPos, oldPos);
			var distRemainder = maxDist - actDist;

			if (distRemainder > 0.0001f)
				newPos = Vector3.MoveTowards(newPos, npos, distRemainder);
			
			this.transform.position = newPos;
				*/
		}

	}

	/*
	private void Update()
	{
		if (!doFollow)
			return;

		faceDir = player.facingDirection;
		if (faceDir == Vector3.zero)
			faceDir = new Vector3(0, 0, 1);

		transform.position = new Vector3(player.transform.position.x + (faceDir.x*(-1.5f)), 0,
										 player.transform.position.z + (faceDir.z*(-1.5f)));
		anim.SetFloat("moveX", faceDir.x);
		anim.SetFloat("moveY", faceDir.y);
	}
	*/
}							
