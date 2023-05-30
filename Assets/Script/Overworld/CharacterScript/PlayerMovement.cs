using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] public float speed = 5f;
	[SerializeField] public Transform nextPoint;

	private Vector2 dir;
	private Vector2 faceDirection;
	private Animator anim;

	private PlayerInput input;
	private InputAction move;
	
	private bool dontAnimate;

	private void Awake()
		=> input = GetComponent<PlayerInput>();

	private void OnEnable()
	{
		move = input.actions["WASD"];
		move.Enable();
	}

	private void OnDisable()
	{
		move.Disable();
	}

	private void Start()
	{
		anim = GetComponentInChildren<Animator>();
		nextPoint.parent = null;
	}
	
	private void Update()
	{
		dir = move.ReadValue<Vector2>();
		Move();

		Debug.Log($"{dir.normalized.x}, {dir.normalized.y}");

		if (dir.x != 0 && dir.y != 0)
			dontAnimate = true;
		else 
			dontAnimate = false;

		if ((dir.normalized.x != 0 || dir.normalized.y != 0) && !dontAnimate)
		{
			anim.Play("walk"); 
			anim.SetFloat("dirx", (int)dir.normalized.x);
			anim.SetFloat("diry", (int)dir.normalized.y);
		} else 
		if ((dir.normalized.x == 0 && dir.normalized.y == 0) || dontAnimate)
			anim.Play("idle");
	}

	private void Move()
	{
		transform.position = Vector3.MoveTowards(transform.position, nextPoint.position, speed* Time.deltaTime);

		if (Vector3.Distance(transform.position, nextPoint.position) <= .05f)
		{	
			if (Mathf.Abs(dir.x) == 1)
				nextPoint.position += new Vector3(dir.x, 0, 0); else 
			if (Mathf.Abs(dir.y) == 1)
				nextPoint.position += new Vector3(0, 0, dir.y);
		}
	}
}							  
