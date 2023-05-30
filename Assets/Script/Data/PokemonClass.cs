using System.Collections.Generic;
using UnityEngine;

public class PokemonClass
{
	[SerializeField] PokemonBaseData Data;
	[SerializeField] int Level;

	public PokemonBaseData data { get { return Data; } }
	public List<MoveClass> moves { get; set; }

	public int level { get { return Level; } }
	public int HP { get; set; }

	public void Initialize()
	{
		HP = maxHp;

		moves = new List<MoveClass>();
		foreach(var m in data.learnables)
		{
			if (m.level <= level)
				moves.Add(new MoveClass(m.move));

			if (moves.Count >= 4)
				break;
		}
	}

	public int attack { get { return Mathf.FloorToInt((data.attack* level) / 100f) + 5; } }
	public int defense { get { return Mathf.FloorToInt((data.defense* level) / 100f) + 5; } }
	public int spcAttack { get { return Mathf.FloorToInt((data.specialAttack* level) / 100f) + 5; } }
	public int spcDefense { get { return Mathf.FloorToInt((data.specialDefense* level) / 100f) + 5; } }
	public int speed { get { return Mathf.FloorToInt((data.speed* level) / 100f) + 5; } }

	public int maxHp { get { return Mathf.FloorToInt((data.maxHp* level) / 100f) + 10; } }

	public DamageDetails TakeDamage(MoveClass move, PokemonClass attackFrom)
	{
		if (move.data.moveclass==moveType.Status)
			return null;

		float critical = 1f;
		if (Random.value* 100f <= 6.25f)
			critical = 2f;
			
		float type = TypeChart.GetEffectiveness(move.data.type, this.data.type1)* TypeChart.GetEffectiveness(move.data.type, this.data.type2);

		var damageDetails = new DamageDetails()
		{
			typeEffectiveness = type,
			critical = critical,
			fainted = false
		};

		/*
		float modifier = Random.Range(0.85f, 1f)* type* critical;
		float a = (2 * attackFrom.level + 10) / 250f;
		float d = a * move.data.power* ((float)attackFrom.attack/defense) + 2;
		int damage = Mathf.RoundToInt(d * modifier);
		*/
		int a = move.data.moveclass==moveType.Physical?attackFrom.attack:attackFrom.spcAttack;
		int d = move.data.moveclass==moveType.Physical?defense:spcDefense;

		float stab = (move.data.type==attackFrom.data.type1||move.data.type==attackFrom.data.type2)?1.5f:1;
		float modifier = Random.Range(0.85f, 1f) * critical * type * stab;
		float x = (2*attackFrom.level/5)+2;
		float y = (x * move.data.power * (a/d) / 50) + 2;
		int damage = Mathf.RoundToInt(y* modifier);

		HP -= damage;
		Debug.Log((float)HP/maxHp);

		if (HP <= 0.01f)
		{
			HP = 0;
			damageDetails.fainted = true;
		}

		return damageDetails;
	}

	public MoveClass GetRandomMove()
		=> moves[Random.Range(0, moves.Count)];
}

public class DamageDetails
{
	public bool fainted { get; set; }
	public float critical { get; set; }
	public float typeEffectiveness { get; set; }
}
