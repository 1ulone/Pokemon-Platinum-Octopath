using UnityEngine;

public enum dialogType
{
	normal,
	exclamatation,
	thoughtBubble
}

[System.Serializable]
public class Dialog 
{
	[SerializeField] dialogType type;
	[TextArea][SerializeField] string lines;

	public dialogType Type { get { return type; } }
	public string Lines { get { return lines; } }
}
