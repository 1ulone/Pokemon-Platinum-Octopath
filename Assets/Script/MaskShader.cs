using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum dofState
{
	close,
	veryClose,
	far
}

public class MaskShader : MonoBehaviour
{
	[SerializeField] private LayerMask layerToCheck;
	[SerializeField] private float range = 2f;

	private dofState state;
	private Volume vol;
	private DepthOfField dof;
	private float baseFocustDist;

	private void Awake()
	{
		state = dofState.far;
		vol = GameObject.Find("Volume").GetComponent<Volume>();
		vol.profile.TryGet<DepthOfField>(out dof);
		baseFocustDist = dof.focusDistance.value;
	}
		
	private void Update()
	{
//		Debug.Log(state);
		Debug.DrawRay(transform.position, Vector3.forward* range, Color.red);
		
		if (Physics.Raycast(transform.position, Vector3.forward, range, layerToCheck))
		{
			if (Physics.Raycast(transform.position, Vector3.forward, range/2f, layerToCheck))
			{
				ChangeState(dofState.veryClose, 2f);
			} else { 
				ChangeState(dofState.close, 3f); 
			}
		}
		else 
		{
			ChangeState(dofState.far, baseFocustDist); 
		}
		
	}

	private void ChangeState(dofState s, float val)
	{
		if (s == state)
			return;

//		LeanTween.cancel(vol.gameObject);
//		state = s;
		LeanTween.value(dof.focusDistance.value, val, 0.25f).setOnUpdate((float x) => dof.focusDistance.value = x).setOnComplete(() => state = s);
	}
}
