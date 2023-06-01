using System.Collections.Generic;
using UnityEngine;

public class ConditionDatabase
{
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
		}
	};
}

public enum ConditionID
{
	none, psn, brn, slp, par, frz
}
