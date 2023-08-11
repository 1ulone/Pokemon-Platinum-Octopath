using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
	[SerializeField] private bool permanentFollow;
	public bool doFollow { get; set; }

	private PlayerController player;
	private Animator anim;
	private Vector3 faceDir;

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

		faceDir = player.facingDirection;
		if (faceDir == Vector3.zero)
			faceDir = new Vector3(0, 0, 1);

		transform.position = player.transform.position + (faceDir*(-1.5f));
	}
}							
