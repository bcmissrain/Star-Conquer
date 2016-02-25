using UnityEngine;
using System.Collections;

public class TestExplode : MonoBehaviour {
	public float m_LiveTime = 1.0f;
	private	float currentTime;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		currentTime += Time.deltaTime;
		if (currentTime >= m_LiveTime) {
			Destroy(this.gameObject);		
		}
	}
}
