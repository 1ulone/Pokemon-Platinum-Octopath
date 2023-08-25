using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PokemonClass
{
	[SerializeField] PokemonBaseData Data;
	[SerializeField] int Level;

	public PokemonBaseData data { get { return Data; } }
	public MoveClass currentMove;
	public List<MoveClass> moves { get; set; }
	public Dictionary<stat, int> stats { get; private set; }
	public Dictionary<stat, int> statBoosts { get; private set; }
	public ConditionClass status { get; private set; }
	public ConditionClass volatileStatus { get; private set; }
	public Queue<string> statusChange { get; private set; }
	public event System.Action onStatusChanged;

	public int statusTime { get; set; }
	public int volatileStatusTime { get; set; }
	public int level { get { return Level; } }
	public int HP { get; set; }
	public int exp { get; set; }

	public bool HpChanged { get; set; }

	public void Initialize()
	{
		moves = new List<MoveClass>();
		statusChange = new Queue<string>();

		foreach(var m in data.learnables)
		{
			if (m.level <= level)
				moves.Add(new MoveClass(m.move));

			if (moves.Count >= 4)
				break;
		}

		CalculateStats();
		HP = maxHp;
		exp = data.GetEXPforLevel(level);
		Debug.Log(exp);

		ResetStatBoost();
		status = null;
		volatileStatus = null;
	}

	private void ResetStatBoost()
	{
		statBoosts = new Dictionary<stat, int>()
		{
			{stat.attack, 0},
			{stat.defense, 0},
			{stat.spAttack, 0},
			{stat.spDefense, 0},
			{stat.speed, 0},
			{stat.accuracy, 0},
			{stat.evasion, 0}
		};
	}

	private void CalculateStats()
	{
		stats = new Dictionary<stat, int>();
		stats.Add(stat.attack, Mathf.FloorToInt((data.attack* level) / 100f) + 5);
		stats.Add(stat.defense, Mathf.FloorToInt((data.defense* level) / 100f) + 5);
		stats.Add(stat.spAttack, Mathf.FloorToInt((data.specialAttack* level) / 100f) + 5);
		stats.Add(stat.spDefense, Mathf.FloorToInt((data.specialDefense* level) / 100f) + 5);
		stats.Add(stat.speed, Mathf.FloorToInt((data.speed* level) / 100f) + 5);

		maxHp = Mathf.FloorToInt((data.maxHp* level) / 100f) + 10 + level;
	}

	private int GetStat(stat s)
	{
		int val = stats[s];

		int boost = statBoosts[s];
		var boostsVal = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

		if (boost >= 0)
			val = Mathf.FloorToInt(val* boostsVal[boost]);
		else 
			val = Mathf.FloorToInt(val/ boostsVal[-boost]);

		return val;
	}

	public int attack { get { return GetStat(stat.attack); } }
	public int defense { get { return GetStat(stat.defense); } }
	public int spcAttack { get { return GetStat(stat.spAttack); } }
	public int spcDefense { get { return GetStat(stat.spDefense); } }
	public int speed { get { return GetStat(stat.speed); } }

	public int maxHp { get; private set; }

	public void UpdateHP(int damage)
	{
		HP = Mathf.Clamp(HP - damage, 0, maxHp);
		HpChanged = true;
	}

	public void ApplyBoosts(List<statBoosts> boostsL)
	{
		 foreach (var sB in boostsL)
		 {
			 var s = sB.stat;
			 var b = sB.boost;

			 statBoosts[s] = Mathf.Clamp(statBoosts[s] + b, -6, 6);

			 if (b > 0)
				 statusChange.Enqueue($"{data.pname}'s {s} rose!");
			 else
				 statusChange.Enqueue($"{data.pname}'s {s} fell!");
		 }
	}

	public void SetStatus(ConditionID cond)
	{
		if (status != null) 
			return;

		status = ConditionDatabase.conditions[cond];
		status?.onAwakeTurn?.Invoke(this);
		statusChange.Enqueue($"{data.pname} {status.startMessage}");
		onStatusChanged?.Invoke();
	}

	public void RemoveStatus()
	{
		status = null;
		onStatusChanged?.Invoke();
	}

	public void SetVolatileStatus(ConditionID cond)
	{
		if (volatileStatus != null) 
			return;

		volatileStatus = ConditionDatabase.conditions[cond];
		volatileStatus?.onAwakeTurn?.Invoke(this);
		statusChange.Enqueue($"{data.pname} {volatileStatus.startMessage}");
	}

	public void RemoveVolatileStatus()
	{
		volatileStatus = null;
	}

	public DamageDetails TakeDamage(MoveClass move, PokemonClass attackFrom)
	{
		if (move.data.category==MoveCategory.Status)
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

		int a = move.data.category==MoveCategory.Physical?attackFrom.attack:attackFrom.spcAttack;
		int d = move.data.category==MoveCategory.Physical?defense:spcDefense;

		float stab = (move.data.type==attackFrom.data.type1||move.data.type==attackFrom.data.type2)?1.5f:1;
		float modifier = Random.Range(0.85f, 1f) * critical * type * stab;
		float x = (2*attackFrom.level/5)+2;
		float y = (x * move.data.power * (a/d) / 50) + 2;
		int damage = Mathf.RoundToInt(y* modifier);

		UpdateHP(damage);

		return damageDetails;
	}

	public MoveClass GetRandomMove()
	{
		var avaM = moves.Where(x => x.pp > 0).ToList();

		return avaM[Random.Range(0, avaM.Count)];
	}

	public bool OnStartTurn()
	{
		bool canExecuteMove = true;
		if (status?.onStartTurn != null)
			if (!status.onStartTurn(this))
				canExecuteMove = false;

		if (volatileStatus?.onStartTurn != null)
			if (!volatileStatus.onStartTurn(this))
				canExecuteMove = false;

		return canExecuteMove;
	}
	
	public void OnEndTurn()
	{
		status?.onEndTurn?.Invoke(this);
		volatileStatus?.onEndTurn?.Invoke(this);
	}

	public void OnBattleEnd()
	{
		volatileStatus = null;
		ResetStatBoost();
	}

	public bool CheckForLevelup()
	{
		Debug.Log($"{exp}/{data.GetEXPforLevel(Level+1)}");
		if (exp > data.GetEXPforLevel(Level + 1))
		{
			++Level;
			return true;
		}
		return false;
	}
}

public class DamageDetails
{
	public bool fainted { get; set; }
	public float critical { get; set; }
	public float typeEffectiveness { get; set; }
}
