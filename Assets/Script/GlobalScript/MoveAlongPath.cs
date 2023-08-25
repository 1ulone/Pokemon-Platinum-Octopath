using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongPath : MonoBehaviour
{
	public static MoveAlongPath p;
	private void Awake()=> p = this;

	[SerializeField] public List<Path> paths; 
	public static Vector3[] GetPath(string tag)
	{
		List<Path> pp = MoveAlongPath.p.paths;
		Vector3[] path = null;

		foreach(Path p in pp)
		{
			if (p.tag.ToLower() == tag.ToLower())
			{
				path = new Vector3[p.path.Length];
				for (int i = 0; i < path.Length; i++)
					path[i] = p.path[i].position;
			}
		}

		Debug.Log(path == null);
		return path;
	}

	public static IEnumerator Initiate(GameObject go, Vector3[] path, float speed, LeanTweenType easeType)
	{
		bool isDon = false;
		go.transform.position = path[0];

		LTDescr tween = LeanTween.moveSpline(go, path, speed).setEase(easeType).setOnComplete(()=> isDon = true);
		yield return new WaitUntil(()=> isDon == true);
	}
}

[System.Serializable]
public class Path
{
	public string tag;
	public Transform[] path;
}
