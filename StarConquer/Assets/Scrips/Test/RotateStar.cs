using UnityEngine;
using System.Collections;

public class RotateStar : MonoBehaviour {
	public Transform center;
	public float rotateSpeed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.RotateAround (center.position, Vector3.up, rotateSpeed * Time.deltaTime);
		//this.transform.Rotate (Vector3.up, -rotateSpeed * Time.deltaTime);
	}
}
