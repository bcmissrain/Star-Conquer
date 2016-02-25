using UnityEngine;
using System.Collections;

public class TestShip : ShipElement {
	public Color m_ShipColor;
	// Use this for initialization
	void Start () {
		this._Init ();
		SetColor (m_ShipColor);
		SetShipScaleBySize ();
	}
	
	// Update is called once per frame
	void Update () {
		this._Update ();
	}

	void OnTriggerEnter(Collider collider)
	{
		this._TriggerEnter (collider);
		//Debug.Log ("Enter " + collider.name);
	}

	void OnTriggerExit(Collider collider)
	{
		this._TriggerExit (collider);
		//Debug.Log ("Exit " + collider.name);
	}

	public override void SetShipScaleBySize (ShipSize shipSize)
	{
		float tempSize = (float)shipSize / 50.0f;
		tempSize = tempSize > 0.5f ? 0.5f : tempSize;
		this.transform.localScale = Vector3.one * tempSize;
	}

	public override void _TriggerEnter (Collider collider)
	{
		if (collider.gameObject == this.m_StarTo.gameObject) 
		{
			_EnterTarget(collider);	
		} 
		else if (collider.gameObject == this.m_StarFrom.gameObject) 
		{

		}
		else
		{
			if(collider.CompareTag("Star"))
			{
				_EnterOther(collider);
			}
		}
	}

	protected override void _EnterTarget (Collider targetCollider)
	{
		TestStar targetStar = targetCollider.gameObject.GetComponent<TestStar> ();
		targetStar.m_TroopNum += this.m_ShipTroopNum;
		targetStar._CollideWithShip (this.transform);
		targetStar.ResetSize ();
		base._EnterTarget (targetCollider);
	}

	protected override void _Destroy ()
	{
		/* Test
		var tempStar = this.m_StarFrom;
		this.m_StarFrom = this.m_StarTo;
		this.m_StarTo = tempStar;
		m_StateManager.ChangeStateTo (ShipState.Born);
		*/
		base._Destroy ();
	}

	public void SetColor(Color color)
	{
		foreach (Transform t in this.transform) {
			if(t.particleSystem)
			{
				//t.particleSystem.startColor = this.m_ShipColor;
			}
			else if(t.renderer)
			{
				t.renderer.material.color = this.m_ShipColor;
			}		
		}
	}
}
