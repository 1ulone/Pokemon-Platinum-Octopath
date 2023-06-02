using System.Collections.Generic;
using UnityEngine;

public class GlobalVariable : MonoBehaviour
{
	public static GlobalVariable instances;

	[SerializeField] private List<material> materials;
	[SerializeField] private List<moveSprite> moveSprites;
	[SerializeField] private List<statusIcon> statusIcons;

	private void Awake()
	{
		instances = this;
	}

	public Sprite GetMoveSprite(PokemonType type)
	{
		Sprite s = null;
			
		foreach(moveSprite m in moveSprites)
			if (m.type == type)
				s = m.sprite;
		
		return s;
	}

	public Sprite GetTypeIcon(PokemonType type)
	{
		Sprite s = null;
			
		foreach(moveSprite m in moveSprites)
			if (m.type == type)
				s = m.icon;
		
		return s;
	}


	public Material GetMaterial(materialType type)
	{
		Material mm = null;
		
		foreach(material m in materials)
			if (m.type == type)
				mm = m.mat;

		return mm;
	}

	public Sprite GetStatusIcon(ConditionID cond)
	{
		Sprite s = null;

		foreach(statusIcon ss in statusIcons)
			if (ss.condition == cond)
				s = ss.sprite;

		return s;
	}
}						 

public enum materialType
{
	Default,
	Glow
}

[System.Serializable]
public class material
{
	[SerializeField] public materialType type;
	[SerializeField] public Material mat;
}

[System.Serializable]
public class moveSprite
{
	[SerializeField] public PokemonType type;
	[SerializeField] public Sprite sprite;
	[SerializeField] public Sprite icon;
}

[System.Serializable]
public class statusIcon
{
	[SerializeField] public ConditionID condition;
	[SerializeField] public Sprite sprite;
}
