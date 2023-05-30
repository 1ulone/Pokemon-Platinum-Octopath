using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField] public float speed = 5f;
	[SerializeField] public Transform nextPoint;
	[SerializeField] public LayerMask wildLayer;

	public Action onEncounter { get; set; }
	public PokemonParty party { get; private set; }

	private Vector2 dir;
	private Vector2 faceDirection;
	private Animator anim;

	private PlayerInput input;
	private InputAction move;
	private InputAction sprint;

	private float defaultSpeed, sprintSpeed;
	
	private bool dontAnimate, onCheck;

	private void Awake()
		=> input = GetComponent<PlayerInput>();

	private void OnEnable()
	{
		move = input.actions["WASD"];
		sprint = input.actions["Sprint"];

		sprint.Enable();
		move.Enable();
	}

	private void OnDisable()
	{
		sprint.Disable();
		move.Disable();
	}

	private void Start()
	{
		anim = GetComponentInChildren<Animator>();
		party = GetComponent<PokemonParty>();
		nextPoint.parent = null;
		defaultSpeed = speed;
		sprintSpeed = defaultSpeed*2;
	}
	
	private void Update()
	{
		dir = move.ReadValue<Vector2>();
		Move();

	//	Debug.Log($"{dir.normalized.x}, {dir.normalized.y}");
	
		if (sprint.WasPerformedThisFrame())
			speed = sprintSpeed; else 
		if (sprint.WasReleasedThisFrame())
			speed = defaultSpeed;

		if (dir.x != 0 && dir.y != 0)
			dontAnimate = true;
		else 
			dontAnimate = false;

		if ((dir.normalized.x != 0 || dir.normalized.y != 0) && !dontAnimate)
		{
			anim.Play(speed==defaultSpeed?"walk":"run"); 
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

	public void CheckForEncounter()
	{
		Collider[] wa= Physics.OverlapSphere(transform.position, 0.5f, wildLayer);
		if (wa.Length > 0)
		{
			if (UnityEngine.Random.Range(1, 101) <= 10)
			{
				GameSystemManager.i.SetOpponentPokemon(wa[UnityEngine.Random.Range(0, wa.Length)].GetComponent<MapArea>().GetRandomWildPokemon());
				GameSystemManager.i.InitiateBattle();
			}
		}
	}
}							  
