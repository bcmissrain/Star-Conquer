using UnityEngine;
using System.Collections;

public class StarElement : MonoBehaviour {
	public enum StarType
	{
		TroopStar = 0,
		DefenceStar,
		MoneyStar,
	}

	public enum StarLevel
	{
		Level0 = 0,
		Level1,
		Level2,
	}

	public enum StarStatus
	{
		None = 0,
		SelectMain,
		SelectSupport,
		SelectAttack,
	}

	public MasterElement m_Master;
	public StarType m_StarType;
	public StarLevel m_StarLevel;
	public int m_MaxTroop;
	public int m_TroopNum;

	public virtual void LevelUp(){}
	public virtual void AttackStar(StarElement enemyStar){}
	public virtual void SupportStar(StarElement friendStar){}
	public virtual void SupportStar(StarElement friendStar,int supportNum){}
	protected virtual void UpdateCreateTroop(){}
	protected virtual void UpdateCreateMoney(){}
}
