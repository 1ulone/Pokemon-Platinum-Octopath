using System.Collections.Generic;
using UnityEngine;

public class ConditionDatabase
{
	public static void Init()
	{
		foreach (var kvp in conditions)
		{
			var cID = kvp.Key;
			var c = kvp.Value;

			c.id = cID;
		}
	}

	public static Dictionary<ConditionID, ConditionClass> conditions { get; set; } = new Dictionary<ConditionID, ConditionClass>()
	{
		{
			ConditionID.psn, 
			new ConditionClass()
			{
				cname = "Poison",
				startMessage = "has been poisoned",
				onEndTurn = (PokemonClass pokemon) =>
				{
					pokemon.UpdateHP(pokemon.maxHp / 8);
					pokemon.statusChange.Enqueue($"{pokemon.data.pname} hurt itself due to poison");
				}
			}
		},
		{
			ConditionID.brn,
			new ConditionClass()
			{
				cname = "Burn",
				startMessage = "has been burned",
				onEndTurn = (PokemonClass pokemon) =>
				{
					pokemon.UpdateHP(pokemon.maxHp / 8);
					pokemon.statusChange.Enqueue($"{pokemon.data.pname} hurt itself due to burn");
				}
			}
		},
		{
			ConditionID.par,
			new ConditionClass()
			{
				cname = "Paralyzed",
				startMessage = "has been paralyzed",
				onStartTurn = (PokemonClass pokemon) =>
				{
					if (Random.Range(1, 5) == 1)
					{
						pokemon.statusChange.Enqueue($"{pokemon.data.pname}'s paralyzed and can't move");
						return false;
					}

					return true;
				}
			}
		},
		{
			ConditionID.frz,
			new ConditionClass()
			{
				cname = "Freeze",
				startMessage = "is frozen solid",
				onStartTurn = (PokemonClass pokemon) =>
				{
					if (Random.Range(1, 5) == 1)
					{
						pokemon.RemoveStatus();
						pokemon.statusChange.Enqueue($"{pokemon.data.pname}'s is frozen solid");
						return true;
					}

					return false;
				}
			}
		},
		{
			ConditionID.slp,
			new ConditionClass()
			{
				cname = "Sleep",
				startMessage = "fell asleep",
				onAwakeTurn = (PokemonClass pokemon) =>
				{
					pokemon.statusTime = Random.Range(1, 4);
				},
				onStartTurn = (PokemonClass pokemon) =>
				{
					if (pokemon.statusTime <= 0)
					{
						pokemon.RemoveStatus();
						pokemon.statusChange.Enqueue($"{pokemon.data.pname} woke up!");
						return true;
					}

					pokemon.statusTime--;
					pokemon.statusChange.Enqueue($"{pokemon.data.pname}'s is fast asleep");
					return false;
				}
			}
		},
		//Volatile status
		{
			ConditionID.confused,
			new ConditionClass()
			{
				cname = "Confused",
				startMessage = "is confused",
				onAwakeTurn = (PokemonClass pokemon) =>
				{
					pokemon.volatileStatusTime = Random.Range(1, 4);
				},
				onStartTurn = (PokemonClass pokemon) =>
				{
					if (pokemon.volatileStatusTime <= 0)
					{
						pokemon.RemoveStatus();
						pokemon.statusChange.Enqueue($"{pokemon.data.pname} snap out of it's confusion!");
						return true;
					}
					pokemon.volatileStatusTime--;
					
					if (Random.Range(1, 3) == 1)
						return true;

					pokemon.statusChange.Enqueue($"{pokemon.data.pname}is confused");
					pokemon.UpdateHP(pokemon.maxHp/8);
					pokemon.statusChange.Enqueue($"It hurt itself due to confusion");
					return false;
				}
			}
		}

	};

	public static float GetStatusBonus(ConditionClass cond)
	{
		if (cond == null)
			return 1f; else 
		if (cond.id == ConditionID.slp || cond.id == ConditionID.frz)
			return 2f; else
		if (cond.id == ConditionID.par || cond.id == ConditionID.psn || cond.id == ConditionID.brn)
			return 1.5f;

		return 1f;
	}
}

public enum ConditionID
{
	none, psn, brn, slp, par, frz,
	confused	
}
