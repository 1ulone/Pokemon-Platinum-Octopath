using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "[pkdx]name", menuName = "Data/Pokemon")]
public class PokemonBaseData : ScriptableObject 
{
	[SerializeField] public string pname;
	[TextArea][SerializeField] public string description;

	[SerializeField] public RuntimeAnimatorController animator;

	[SerializeField] public PokemonType type1;
	[SerializeField] public PokemonType type2;

	[SerializeField] public int maxHp;
	[SerializeField] public int attack;
	[SerializeField] public int defense;
	[SerializeField] public int specialAttack;
	[SerializeField] public int specialDefense;
	[SerializeField] public int speed;
	
	[SerializeField] public List<LearnableMove> learnables;

	[SerializeField] public int inGameSize;
	[SerializeField] public float height;
	[SerializeField] public float width;
	[SerializeField] public Sprite icon;

	[Header("Glow or not")]
	[SerializeField] public bool emmitsLight;
	[SerializeField][ColorUsage(true, true)] public Color lightColor;
	[SerializeField] public Vector3 lightPosition;
	[SerializeField] public float lightIntensity;
}

[System.Serializable]
public class LearnableMove
{
	[SerializeField] public MoveBaseData move;
	[SerializeField] public int level;
}

public enum stat
{
	attack,
	defense,
	spAttack,
	spDefense,
	speed,

	accuracy,
	evasion
}

public enum PokemonType
{
	none,
	normal,
	fire,
	water,
	grass,
	electric,
	ice,
	fighting,
	poison,
	ground,
	flying,
	psychic,
	bug,
	rock,
	ghost,
	dragon,
	dark,
	steel,
	fairy
}

public class TypeChart
{
	static float[][] chart =
	{							
		//				 		   NORM FIRE WATR GRSS ELEC ICE  FIGH PSN  GRND FLY  PSY  BUG  ROCK GHST DRGN DARK STEL FAIRY
		/*60NORMAL */ new float[] { 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f ,0.5f, 0f , 1f , 1f ,0.5f, 1f  },
		/*61FIRE   */ new float[] { 1f ,0.5f,0.5f, 2f , 1f , 2f , 1f , 1f , 1f , 1f , 1f , 2f ,0.5f, 1f ,0.5f, 1f , 2f , 1f  },
		/*62WATER  */ new float[] { 1f , 2f ,0.5f,0.5f, 1f , 1f , 1f , 1f , 2f , 1f , 1f , 1f , 2f , 1f ,0.5f, 1f , 1f , 1f  },
		/*63GRASS  */ new float[] { 1f ,0.5f, 2f ,0.5f, 1f , 1f , 1f ,0.5f, 2f ,0.5f, 1f ,0.5f, 2f , 1f ,0.5f, 1f ,0.5f, 1f  }, 
		/*64ELECTRC*/ new float[] { 1f , 1f , 2f ,0.5f,0.5f, 1f , 1f , 1f , 0f , 2f , 1f , 1f , 1f , 1f ,0.5f, 1f , 1f , 1f  },
		/*65ICE    */ new float[] { 1f ,0.5f,0.5f, 2f , 1f ,0.5f, 1f , 1f , 2f , 2f , 1f , 1f , 1f , 1f , 2f , 1f ,0.5f, 1f  },
		/*66FGHTING*/ new float[] { 2f , 1f , 1f , 1f , 1f , 2f , 1f ,0.5f, 1f ,0.5f,0.5f,0.5f, 2f , 0f , 1f , 2f , 2f ,0.5f },
		/*67POISON */ new float[] { 1f , 1f , 1f , 2f , 1f , 1f , 1f ,0.5f,0.5f, 1f , 1f , 1f ,0.5f,0.5f, 1f , 1f , 0f , 2f  },
		/*68GROUND */ new float[] { 1f , 2f , 1f ,0.5f, 2f , 1f , 1f , 2f , 1f , 0f , 1f ,0.5f, 2f , 1f , 1f , 1f , 2f , 1f  },
		/*69FLYING */ new float[] { 1f , 1f , 1f , 2f ,0.5f, 1f , 2f , 1f , 1f , 1f , 1f , 2f ,0.5f, 1f , 1f , 1f ,0.5f, 1f  },
		/*70PSYCHIC*/ new float[] { 1f , 1f , 1f , 1f , 1f , 1f , 2f , 2f , 1f , 1f ,0.5f, 1f , 1f , 1f , 1f , 0f ,0.5f, 1f  },
		/*71BUG    */ new float[] { 1f ,0.5f, 1f , 2f , 1f , 1f ,0.5f,0.5f, 1f ,0.5f, 2f , 1f , 1f ,0.5f, 1f , 2f ,0.5f,0.5f },
		/*72ROCK   */ new float[] { 1f , 2f , 1f , 1f , 1f , 1f ,0.5f, 1f ,0.5f, 2f , 1f , 2f , 1f , 1f , 1f , 1f ,0.5f, 1f  },
		/*73GHOST  */ new float[] { 0f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 2f , 1f , 1f , 2f , 1f ,0.5f, 1f , 1f  },
		/*74DRAGON */ new float[] { 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 2f , 1f ,0.5f, 0f  },
		/*75DARK   */ new float[] { 1f , 1f , 1f , 1f , 1f , 1f ,0.5f, 1f , 1f , 1f , 2f , 1f , 1f , 2f , 1f ,0.5f, 1f ,0.5f },
		/*76STEEL  */ new float[] { 1f ,0.5f,0.5f, 1f ,0.5f, 2f , 1f , 1f , 1f , 1f , 1f , 1f , 2f , 1f , 1f , 1f ,0.5f, 2f  },
		/*77FAIRY  */ new float[] { 1f ,0.5f, 1f , 1f , 1f , 1f , 2f ,0.5f, 1f , 1f , 1f , 1f , 1f , 1f , 2f , 2f ,0.5f, 1f  }
	};

	public static float GetEffectiveness(PokemonType attackType, PokemonType receiveType)
	{
		if (attackType == PokemonType.none || receiveType == PokemonType.none)
			return 1;
		int row = (int) attackType - 1;
		int col = (int) receiveType -1;

		return chart[row][col];
	}
}
