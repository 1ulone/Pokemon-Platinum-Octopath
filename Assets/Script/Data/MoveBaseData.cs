using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "[000]Move", menuName = "Data/Moves")]
public class MoveBaseData : ScriptableObject
{
	[SerializeField] public string mname;
	[TextArea][SerializeField] public string Description;

	[SerializeField] public PokemonType type;
	[SerializeField] public int pp;
	[SerializeField] public int accuracy;
	[SerializeField] public int power;
	[SerializeField] public int priority;
	[SerializeField] public MoveCategory category;
	[SerializeField] public MoveTarget target;
	[SerializeField] public MoveEffects effects;
}							

[System.Serializable]
public class MoveEffects
{
	[SerializeField] public List<statBoosts> boosts;
	[SerializeField] public ConditionID status;
	[SerializeField] public ConditionID volatileStatus;
}

[System.Serializable]
public class statBoosts
{
	public stat stat;
	public int boost;
}

public enum MoveCategory 
{
	Status,
	Physical,
	Special
}

public enum MoveTarget
{
	Foe,
	Self
}
