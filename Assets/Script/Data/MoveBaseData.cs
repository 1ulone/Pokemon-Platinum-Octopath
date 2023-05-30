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
	[SerializeField] public moveType moveclass;
}							

public enum moveType
{
	Status,
	Physical,
	Special
}
