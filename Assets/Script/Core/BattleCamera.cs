using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum camState
{
	onIdle,
	onAttack,
	onZoomPlayer,
	onZoomEnemy
}

public class BattleCamera : MonoBehaviour
{
	public static BattleCamera cam;
	public List<staticCameraInfo> infos;
	private staticCameraInfo currentInfo;
	
	[SerializeField] private Transform playerPokemon;
	[SerializeField] private Transform opponentPokemon;

	private camState state;

	private void Awake()
		=> cam = this;

	public void SetBattleCamera(int unit1, int unit2)
	{
		int h = unit1 <= unit2 ? unit1 : unit2;
		foreach(staticCameraInfo i in infos)
			if (h == i.tag)
				SetCameraInfo(i);
	}

	public IEnumerator ChangeState(camState state)
	{
		if (this.state == state)
			yield return null;

		this.state = state;
		switch(this.state)
		{
			case (camState.onIdle): { IdleCamera(); } break;
			case (camState.onAttack): { AttackCamera(); } break;
			case (camState.onZoomPlayer): { PlayerCamera(); } break;
			case (camState.onZoomEnemy): { EnemyCamera(); } break;
		}

		yield return new WaitForSeconds(0.5f);
	}

	private void IdleCamera()
		=> LeanTween.move(this.gameObject, currentInfo.position, 0.5f).setEaseInCubic();

	private void AttackCamera()
	{
		Vector3 npos = currentInfo.position + new Vector3(0, -1, 1);
		LeanTween.move(this.gameObject, npos, 0.5f).setEaseInCubic();
	}

	private void PlayerCamera()
	{
		float pHeight = playerPokemon.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
		Vector3 npos = currentInfo.playerPokemonPosition + new Vector3(0, pHeight+currentInfo.playerMod, currentInfo.position.z/2f);
		LeanTween.move(this.gameObject, npos, 0.5f).setEaseInCubic();
	}

	private void EnemyCamera()
	{
		float pHeight = opponentPokemon.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
		Vector3 npos = currentInfo.opponentPokemonPosition + new Vector3(0, pHeight+currentInfo.opponentMod, currentInfo.position.z/2f);
		LeanTween.move(this.gameObject, npos, 0.5f).setEaseInCubic();
	}

	private void SetCameraInfo(staticCameraInfo cInfo)
	{
		this.transform.position = cInfo.position;
		playerPokemon.position = cInfo.playerPokemonPosition;
		opponentPokemon.position = cInfo.opponentPokemonPosition;
		currentInfo = cInfo;
	}

}

[System.Serializable]
public class staticCameraInfo
{
	[SerializeField] public float tag;
	[SerializeField] public Vector3 position;
	[SerializeField] public Vector3 playerPokemonPosition;
	[SerializeField] public Vector3 opponentPokemonPosition;

	[SerializeField] public float playerMod;
	[SerializeField] public float opponentMod;
}
