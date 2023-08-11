using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum camState
{
	onIdle,
	onAttack,
	onTrainerZoom
//	onZoomPlayer,
//	onZoomEnemy
}

public class BattleCamera : MonoBehaviour
{
	public static BattleCamera cam;
	public List<staticCameraInfo> infos;
	private staticCameraInfo currentInfo;
	
	[SerializeField] private Transform playerPokemon;
	[SerializeField] private Transform opponentPokemon;
	[SerializeField] private Transform playerTrainer; 
	[SerializeField] private Transform opponentTrainer;

	private camState state;

	private void Awake()
		=> cam = this;

	public void SetBattleCamera(int unit1, int unit2)
	{
		int h = unit1 <= unit2 ? unit1 : unit2;
		if (h == 10)
			if (unit1 == unit2) 
				h = unit1* unit2; 

		foreach(staticCameraInfo i in infos)
			if (h == i.tag)
				SetCameraInfo(i);
	}

	public IEnumerator ChangeState(camState state)
	{
		if (this.state == state)
			yield break;

		this.state = state;
		switch(this.state)
		{
			case (camState.onIdle): { IdleCamera(); } break;
			case (camState.onAttack): { AttackCamera(); } break;
			case (camState.onTrainerZoom) : { TrainerZoom(); } break;

//			case (camState.onZoomPlayer): { PlayerCamera(); } break;
//			case (camState.onZoomEnemy): { EnemyCamera(); } break;
		}

		yield return new WaitForSeconds(0.5f);
	}
	////FIX THIS MAKE IT MORE DYNAMIC ORRR JUST REMOVE IT !

	private void IdleCamera()
		=> LeanTween.move(this.gameObject, currentInfo.data.position, 0.5f).setEaseInCubic();

	private void AttackCamera()
	{
		Vector3 npos = currentInfo.data.position + new Vector3(0, -1, 1);
		LeanTween.move(this.gameObject, npos, 0.5f).setEaseInCubic();
	}

	private void TrainerZoom()
	{
		Vector3 tpos = currentInfo.data.opponentTrainerPosition;
		Vector3 npos = new Vector3(tpos.x, tpos.y + 3, tpos.z - 5);
		Debug.Log("a");
		LeanTween.move(this.gameObject, npos, 0.5f).setEaseInCubic();
	}

	/*
	private void PlayerCamera()
	{
		float pHeight = playerPokemon.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
		Vector3 npos = currentInfo.data.playerPokemonPosition + new Vector3(0, pHeight+currentInfo.playerMod, currentInfo.data.position.z/2f);
		LeanTween.move(this.gameObject, npos, 0.5f).setEaseInCubic();
	}

	private void EnemyCamera()
	{
		float pHeight = opponentPokemon.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
		Vector3 npos = currentInfo.data.opponentPokemonPosition + new Vector3(0, pHeight+currentInfo.opponentMod, currentInfo.data.position.z/2f);
		LeanTween.move(this.gameObject, npos, 0.5f).setEaseInCubic();
	}
	*/

	private void SetCameraInfo(staticCameraInfo cInfo)
	{
		state = camState.onIdle;
		BattleCameraData data = cInfo.data;
		this.transform.position = data.position;
		playerPokemon.position = data.playerPokemonPosition;
		opponentPokemon.position = data.opponentPokemonPosition;

		playerTrainer.position = data.playerTrainerPosition;
		if (opponentTrainer.gameObject.activeSelf)
			opponentTrainer.position = data.opponentTrainerPosition;

		currentInfo = cInfo;
	}

}

[System.Serializable]
public class staticCameraInfo
{
	[SerializeField] public float tag;
	[SerializeField] public BattleCameraData data;

	[SerializeField] public float playerMod;
	[SerializeField] public float opponentMod;
}
