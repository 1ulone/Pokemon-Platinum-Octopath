using UnityEngine;

[CreateAssetMenu(fileName = "New BCamera Data", menuName = "Data/Battle Camera Data")]
public class BattleCameraData : ScriptableObject
{
	[SerializeField] public Vector3 position;

	[SerializeField] public Vector3 playerPokemonPosition;
	[SerializeField] public Vector3 opponentPokemonPosition;
	[SerializeField] public Vector3 playerTrainerPosition;
	[SerializeField] public Vector3 opponentTrainerPosition;
}								
