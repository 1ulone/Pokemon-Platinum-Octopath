using System;
using UnityEngine;

public class ConditionClass
{
	public string cname { get; set; }
	public string description { get; set; }
	public string startMessage { get; set; }
	public ConditionID id { get; set; }

	public Func<PokemonClass, bool> onStartTurn { get; set; }
	public Action<PokemonClass> onEndTurn { get; set; }
	public Action<PokemonClass> onAwakeTurn { get; set; }
}
