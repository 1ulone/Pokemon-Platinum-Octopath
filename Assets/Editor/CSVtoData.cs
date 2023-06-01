using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVtoSO 
{
	private static string enemyCSVPath = "/Editor/CSV/moves.csv";

	[MenuItem("Utilities/GenerateMoveData")]
	public static void GenerateEnemies()
	{
		string[] allLines = File.ReadAllLines(Application.dataPath + enemyCSVPath);

		foreach(string s in allLines)
		{
			string[] splitData = s.Split(';');

			MoveBaseData move = ScriptableObject.CreateInstance<MoveBaseData>();
			string objname = $"[{splitData[0]}]{splitData[1].ToUpper()}";
			move.mname = splitData[1];
			move.type = (PokemonType)int.Parse(splitData[2]);
			move.power = int.Parse(splitData[3]);
			move.pp = int.Parse(splitData[4]);
			move.accuracy = int.Parse(splitData[5]);
			move.priority = int.Parse(splitData[6]);
			move.category = (MoveCategory)int.Parse(splitData[8])-1;

			AssetDatabase.CreateAsset(move, $"Assets/Data/Move/{objname}.asset");
		}

		AssetDatabase.SaveAssets();
	}
}					   
