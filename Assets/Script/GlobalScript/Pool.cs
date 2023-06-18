using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
	public static Pool i;

	private Dictionary<string, Queue<GameObject>> poolDict = new Dictionary<string, Queue<GameObject>>();

	[SerializeField] private Transform poolParent;
	[SerializeField] private List<objects> poolList;

	private void Awake()
	{
		i = this;

		foreach(objects o in poolList)
		{
			Queue<GameObject> qg = new Queue<GameObject>();
			for (int i = 0; i < o.count; i++)
			{
				GameObject ng = Instantiate(o.obj);
				ng.transform.SetParent(poolParent);
				ng.SetActive(false);
				qg.Enqueue(ng);
			}

			poolDict.Add(o.tag.ToLower(), qg);
		}
	}

	public GameObject CreateObject(string tag, Vector3 position, Vector3 rotation, Transform parent = null)
	{
		if (!poolDict.ContainsKey(tag.ToLower()))
			return null;

		GameObject g = poolDict[tag.ToLower()].Dequeue();
		g.SetActive(true);

		g.transform.position = position;
		g.transform.eulerAngles = rotation;
		g.transform.SetParent(parent==null?poolParent:parent);

		return g;
	}

	public void DestroyObject(string tag, GameObject g)
	{
		g.SetActive(false);
		g.transform.SetParent(poolParent);
		poolDict[tag.ToLower()].Enqueue(g);
	}
}			

[System.Serializable]
public class objects
{
	public string tag;
	public GameObject obj;
	public int count;
}
