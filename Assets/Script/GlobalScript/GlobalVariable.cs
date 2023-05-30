using System.Collections.Generic;
using UnityEngine;

public class GlobalVariable : MonoBehaviour
{
	public static GlobalVariable instances;

	[SerializeField] private List<material> materials;
	[SerializeField] private List<moveSprite> moveSprites;

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

	public Material GetMaterial(materialType type)
	{
		Material mm = null;
		
		foreach(material m in materials)
			if (m.type == type)
				mm = m.mat;

		return mm;
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
}
