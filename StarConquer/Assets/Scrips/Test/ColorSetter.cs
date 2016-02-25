using UnityEngine;
using System.Collections;

public class ColorSetter : MonoBehaviour {
	public Color m_Color;
	// Use this for initialization
	void Start () {
		foreach (Transform t in transform) {
			if (t.renderer) {
				t.renderer.material.color = m_Color;
			}		
		}
	}
}
