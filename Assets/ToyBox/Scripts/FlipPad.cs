using UnityEngine;
using System.Collections;

public class FlipPad : MonoBehaviour {

	private AnimatorPuzzle ani;

	public bool state = false;

	public float flipTime = 4;

	void Start () {
		ani = gameObject.GetComponent<AnimatorPuzzle> ();
		state = false;
	}
}
