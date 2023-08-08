using System.Collections.Generic;
using UnityEngine;

public class DialogBoxSprite : MonoBehaviour
{
	public static DialogBoxSprite b;
	[SerializeField] public List<boxStyle> boxes;

	private void Awake() => b = this;

	public Sprite GetBoxStyle(string tag, dialogType type)
	{
		Sprite s = null;

		foreach(boxStyle bs in boxes)
			if (bs.tag.ToLower() == tag.ToLower())
				foreach(boxType t in bs.type)
					if (t.type == type)
						s = t.spr;		

		return s;
	}
}

[System.Serializable]
public class boxStyle
{
	[SerializeField] public string tag;
	[SerializeField] public List<boxType> type;
}

[System.Serializable]
public class boxType
{
	[SerializeField] public dialogType type;
	[SerializeField] public Sprite spr;
}
