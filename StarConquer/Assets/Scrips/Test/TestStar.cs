using UnityEngine;
using System.Collections;

public class TestStar : StarElement {
	public float m_bornTime_s;
	public int m_bornNum;
	public Transform[] m_bornPoint;
	public GameObject m_shipPrefab;
	public GameObject m_explodePrefab;

	private float bornTimeCounter = 0.0f;
	private float coldTime = 0.0f;
	private float coldTimeCounter = 0.0f;
	private bool CanSupport = true;

	public void _Update (TestManager manager) {
		UpdateCreateTroop ();
		UpdateColdTime ();
		ResetSize ();
	}

	public void _CollideWithShip(Transform shipPos){
		GameObject explodeObj = Instantiate (m_explodePrefab) as GameObject;
		explodeObj.transform.position = shipPos.position;
		explodeObj.transform.parent = this.transform;
	}

	protected override void UpdateCreateTroop ()
	{
		if (bornTimeCounter >= m_bornTime_s) 
		{
			bornTimeCounter = 0.0f;

			this.m_TroopNum += m_bornNum;
		}
		else
		{
			bornTimeCounter+=Time.deltaTime;
		}
	}

	private void UpdateColdTime()
	{
		coldTimeCounter += Time.deltaTime;
		if (coldTimeCounter >= coldTime) {
			coldTimeCounter = 0.0f;
			CanSupport = true;
		}
	}

	public override void SupportStar (StarElement friendStar, int supportNum)
	{
		int troopNum = supportNum;
		if (troopNum > 0) 
		{
			Transform currentBornPos = m_bornPoint[Random.Range(0,m_bornPoint.Length)];
			GameObject newShip = Instantiate (m_shipPrefab) as GameObject;
			if (newShip) {
				ShipElement shipElement = newShip.GetComponent<ShipElement>();
				shipElement.m_StarFrom = this;
				shipElement.m_StarTo = friendStar;
				shipElement.m_ShipSize = GetSizeByNum(troopNum);
				shipElement.m_ShipTroopNum = troopNum;
				shipElement.m_FlySpeed = shipElement.m_SurrondSpeed = 4 + Random.Range(0.0f,5.0f);
				newShip.transform.position = currentBornPos.position;
				((TestShip)shipElement).SetShipScaleBySize();
				this.m_TroopNum -= troopNum;
			}
		}
		
		ResetSize ();
	}

	public override void SupportStar (StarElement friendStar)
	{
		if (CanSupport == false)
			return;
		else {
			CanSupport = false;
			coldTime = Random.Range(2.0f,6.0f);
		}

		int randomTroopNum = GetRandomTroopNum ();
		if (randomTroopNum > 0) 
		{
			Transform currentBornPos = m_bornPoint[Random.Range(0,m_bornPoint.Length)];
			GameObject newShip = Instantiate (m_shipPrefab) as GameObject;
			if (newShip) {
				ShipElement shipElement = newShip.GetComponent<ShipElement>();
				shipElement.m_StarFrom = this;
				shipElement.m_StarTo = friendStar;
				shipElement.m_ShipSize = (ShipElement.ShipSize)randomTroopNum;
				shipElement.m_ShipTroopNum = randomTroopNum;
				shipElement.m_FlySpeed = shipElement.m_SurrondSpeed = 4 + Random.Range(0.0f,5.0f);
				newShip.transform.position = currentBornPos.position;
				((TestShip)shipElement).SetShipScaleBySize();

				this.m_TroopNum -= randomTroopNum;
			}
		}

		ResetSize ();
	}

	private int GetRandomTroopNum()
	{
		int randomNum = Random.Range (0, m_TroopNum);
		if (randomNum >= (int)ShipElement.ShipSize.Size200) {
				return 200;		
		} else if (randomNum >= (int)ShipElement.ShipSize.Size100) {
				return 100;
		} else if (randomNum >= (int)ShipElement.ShipSize.Size50) {
				return 50;		
		} else if (randomNum >= (int)ShipElement.ShipSize.Size30) {
				return 30;		
		} else if (randomNum >= (int)ShipElement.ShipSize.Size20) {
				return 20;		
		} else if (randomNum >= (int)ShipElement.ShipSize.Size10) {
				return 10;
		} else if (randomNum >= (int)ShipElement.ShipSize.Size5) {
				return 5;
		} else {
			return 0;		
		}
	}

	private ShipElement.ShipSize GetSizeByNum(int num){
		if (num >= (int)ShipElement.ShipSize.Size200) {
			return ShipElement.ShipSize.Size200;
		} else if (num >= (int)ShipElement.ShipSize.Size100) {
			return ShipElement.ShipSize.Size100;
		} else if (num >= (int)ShipElement.ShipSize.Size50) {
			return ShipElement.ShipSize.Size50;		
		} else if (num >= (int)ShipElement.ShipSize.Size30) {
			return ShipElement.ShipSize.Size30;		
		} else if (num >= (int)ShipElement.ShipSize.Size20) {
			return ShipElement.ShipSize.Size20;		
		} else if (num >= (int)ShipElement.ShipSize.Size10) {
			return ShipElement.ShipSize.Size10;
		} else if (num >= (int)ShipElement.ShipSize.Size5) {
			return ShipElement.ShipSize.Size5;
		} else {
			return ShipElement.ShipSize.Size0;		
		}
	}

	public void ResetSize()
	{
		if (this.m_TroopNum >= 150) {
			this.transform.localScale = Vector3.one * 1.5f;
		} else if (this.m_TroopNum >= 100) {
			this.transform.localScale = Vector3.one * (1 + 0.5f * (this.m_TroopNum - 100) / 100.0f);	
		} else {
			this.transform.localScale = Vector3.one * (0.5f + 0.5f * this.m_TroopNum /100.0f);
		}
	}
}
