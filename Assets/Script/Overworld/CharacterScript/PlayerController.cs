using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField] public float speed = 5f;
	[SerializeField] public Transform nextPoint;
	[SerializeField] public LayerMask wildLayer;
	[SerializeField] public LayerMask interactableLayer;
	[SerializeField] public LayerMask colliderL;
	[SerializeField] public LayerMask footable;
	[SerializeField] public LayerMask trainerEye;

	public bool canInteract { get; set; }
	public Action onEncounter { get; set; }
	public PokemonParty party { get; private set; }
	public Vector3 facingDirection { get { return faceDirection; } }
	public Vector2 inputDirection { get { return dir; } }

	private Vector2 dir;
	private Vector3 faceDirection;
	private Animator anim;

	private PlayerInput input;
	private InputAction move;
	private InputAction sprint;
	private InputAction interact;

	private float defaultSpeed, sprintSpeed;
	
	private bool dontAnimate, onCheck, onWall, canMove;

	private void Awake()
		=> input = GetComponent<PlayerInput>();

	private void OnEnable()
	{
		move = input.actions["WASD"];
		sprint = input.actions["Sprint"];
		interact = input.actions["Interact"];

		sprint.Enable();
		move.Enable();
		interact.Enable();
	}

	private void OnDisable()
	{
		sprint.Disable();
		move.Disable();
		interact.Disable();
	}

	private void Start()
	{
		anim = GetComponentInChildren<Animator>();
		party = GetComponent<PokemonParty>();

		nextPoint.parent = null;
		defaultSpeed = speed;
		sprintSpeed = defaultSpeed*2;
		canInteract = true;
		canMove = true;
	}
	
	private void Update()
	{
/////ONDIALOG
		if (interact.WasPressedThisFrame())
		{
			if (canInteract)
				Interact(); 
			else 
				OverworldDialogManager.d.NextDialog();
		}

		if (OverworldDialogManager.onDialog || !canMove || !canInteract)
			return;
/////BASE
		dir = move.ReadValue<Vector2>();
		Move();
		CollisionCheck();
		CheckForTrainer();

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
			//MOVING
			faceDirection = new Vector3(dir.normalized.x, 0, dir.normalized.y);

			anim.Play(speed==defaultSpeed?"walk":"run"); 
			anim.SetFloat("dirx", (int)dir.normalized.x);
			anim.SetFloat("diry", (int)dir.normalized.y);
		} else 
		if ((dir.normalized.x == 0 && dir.normalized.y == 0) || dontAnimate)
			anim.Play("idle");

	}

	private void Move()
	{
		if (onWall)
		{ 
			nextPoint.position = transform.position;
			return;
		}

		if (!onWall)
			transform.position = Vector3.MoveTowards(transform.position, nextPoint.position, speed* Time.deltaTime);

		if (Vector3.Distance(transform.position, nextPoint.position) <= .05f)
		{	
			if (Mathf.Abs(dir.x) == 1)
				nextPoint.position += new Vector3(dir.x, 0, 0); else 
			if (Mathf.Abs(dir.y) == 1)
				nextPoint.position += new Vector3(0, 0, dir.y);
		}
	}

	private void Interact()
	{
		if (!canInteract)
			return;

		anim.Play("idle");
		canInteract = false;

		var faceDir = new Vector3(anim.GetFloat("dirx"), 0, anim.GetFloat("diry"));
		var interactPos = transform.localPosition + (faceDir*2);

		var collider = Physics.OverlapSphere(interactPos, 0.25f, interactableLayer);
		if (collider != null)
			foreach(Collider g in collider)
				g.GetComponent<NPCController>()?.Interact(this.transform.position);
	}

	private void CollisionCheck()
	{
		onWall = Physics.OverlapSphere(transform.position + faceDirection.normalized/2, 0.5f, colliderL).Length > 0;
		//Debug.DrawRay(transform.position, faceDirection.normalized* 1, Color.red);
	}

	private float GetFacingAngle()
	{
		float angle = Mathf.Atan2(faceDirection.x, faceDirection.z)* Mathf.Rad2Deg;
		return angle<0?angle+360:angle;
	}

	public void CheckForEncounter()
	{
		Collider[] wa= Physics.OverlapSphere(transform.position, 0.5f, wildLayer);
		if (wa.Length > 0)
		{
			if (UnityEngine.Random.Range(1, 101) <= 10)
			{
				PokemonParty pp = new PokemonParty();
				pp.WildParty(wa[UnityEngine.Random.Range(0, wa.Length)].GetComponent<MapArea>().GetRandomWildPokemon());

				GameSystemManager.i.SetOpponentPokemon(pp);
				GameSystemManager.i.InitiateBattle(BattleAgainst.wild);
			}
		}
	}

	public void CheckForTrainer()
	{
		if (!canMove)
			return;

		Collider[] te= Physics.OverlapSphere(transform.position, 0.1f, trainerEye);
		if (te.Length > 0)
		{
			foreach(Collider c in te)
			{
				c.GetComponent<TrainerTrigger>().Trigger(this.transform.position, ()=>
				{
					faceDirection = c.transform.position - transform.position;

					anim.SetFloat("dirx", (int)faceDirection.x);
					anim.SetFloat("diry", (int)faceDirection.z);
				});

				anim.Play("idle");
				canInteract = false;
				canMove = false;
			}
		}
	}

	public void CreateFootstep()
	{
		if (Physics.OverlapSphere(new Vector3(transform.position.x, 0, transform.position.z), 0.25f, footable).Length <= 0)
			return;

		Pool.i.CreateObject("snowstep", new Vector3(
					transform.position.x - faceDirection.x/2,
					0.1f,
					transform.position.z - faceDirection.z/2
					), new Vector3(90, GetFacingAngle(), 0));
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(new Vector3(transform.position.x, 0, transform.position.z), 0.25f);
		Gizmos.DrawWireSphere(transform.position + faceDirection.normalized/2, 0.5f);
	}
}							  
