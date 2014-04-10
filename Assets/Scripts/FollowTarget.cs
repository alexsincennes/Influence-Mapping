using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour {

	public Unit u;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = u.target;
	}
}
