using UnityEngine;
using System.Collections;

public class ShipElement : MonoBehaviour {
	public enum ShipSize
	{
		Size0 = 0,
		Size5 = 5,
		Size10 = 10,
		Size20 = 20,
		Size30 = 30,
		Size50 = 50,
		Size100 = 100,
		Size200 = 200,
	}

	public enum ShipFunc
	{
		Attack,
		Support,
	}

	public enum ShipState
	{
		Born,
		Fly,
		Surrond,
		Land,
		Hide,
	}

	#region State Change
	protected abstract class ShipStateBase
	{
		protected StateManager m_StateManager;
		public ShipElement m_Ship;
		public abstract ShipState m_ShipState { get;}
		public ShipStateBase(StateManager stateManager,ShipElement ship)
		{
			this.m_StateManager = stateManager;
			this.m_Ship = ship;
		}
		public virtual void UpdateState(){}
		public virtual void EnterTargetStar(Collider collider){} //Trigger By Manager Earlier Than UpdateState
		public virtual void EnterOtherStar(Collider collider){} //Trigger By Manager Earlier Than UpdateState

		protected bool CanResumeMove(Vector3 dir0,Vector3 dir1)
		{
			if (Vector3.Dot (dir0.normalized, dir1.normalized) > MIN_RADIAN)
				return true;
			
			return false;
		}
	}

	//Can change to Fly
	private class ShipBornState:ShipStateBase
	{
		public ShipBornState(StateManager stateManager,ShipElement ship):base(stateManager,ship){}

		public override ShipState m_ShipState {
			get {
				return ShipState.Born;
			}
		}

		public override void UpdateState ()
		{
			m_StateManager.ChangeStateTo (ShipState.Fly);
		}
	}

	//Can change to Surrond & Land & Hide
	private class ShipFlyState:ShipStateBase
	{
		public ShipFlyState(StateManager stateManager,ShipElement ship):base(stateManager,ship){}

		public override ShipState m_ShipState {
			get {
				return ShipState.Fly;
			}
		}

		public override void UpdateState ()
		{
			m_Ship.MoveToTarget ();
		}

		public override void EnterTargetStar (Collider collider)
		{
			m_StateManager.ChangeStateTo (ShipState.Land);
		}

		public override void EnterOtherStar (Collider collider)
		{
			StarElement otherStar = collider.gameObject.GetComponent<StarElement>();
			SetShipSurrondDirection (otherStar);
			if (CanBeginSurrond(m_Ship,otherStar)) //Change To Surrond
			{
				m_Ship.m_SurrondStar = collider.gameObject.GetComponent<StarElement>();
				m_Ship.transform.parent = otherStar.transform;
				m_StateManager.ChangeStateTo (ShipState.Surrond);
			}
		}

		private void SetShipSurrondDirection(StarElement otherStar)
		{
			Vector3 crossV1 = otherStar.transform.position - m_Ship.transform.position;
			Vector3 crossV2 = new Vector3 (crossV1.x, crossV1.y + 1, crossV1.z);//ship.transform.up;
			Vector3 crossResult = Vector3.Cross (crossV1, crossV2) * m_Ship.m_SurrondSpeed;
			Vector3 ship2Target = m_Ship.m_StarTo.transform.position - this.m_Ship.transform.position;
			float distance1 = (ship2Target - crossResult * 1).sqrMagnitude;
			float distance2 = (ship2Target - crossResult * (-1)).sqrMagnitude;

			if (distance1 < distance2) 
			{
				m_Ship.m_SurrondDirection = 1;
			}
			else
			{
				m_Ship.m_SurrondDirection = -1;
			}
		}

		private bool CanBeginSurrond(ShipElement ship,StarElement otherStar)
		{
			Vector3 crossV1 = otherStar.transform.position - ship.transform.position;
			Vector3 crossV2 = new Vector3 (crossV1.x, crossV1.y + 1, crossV1.z);//ship.transform.up;
			Vector3 crossResult = Vector3.Cross (crossV1, crossV2) * m_Ship.m_SurrondDirection;
			Vector3 plane2TargetDirection = m_Ship.m_StarTo.transform.position - m_Ship.transform.position;
			return !CanResumeMove (plane2TargetDirection, crossResult);
		}
	}

	//Can change to Fly & Land & Hide
	private class ShipSurrondState:ShipStateBase
	{
		public ShipSurrondState(StateManager stateManager,ShipElement ship):base(stateManager,ship){}

		public override ShipState m_ShipState {
			get {
				return ShipState.Surrond;
			}
		}
		
		public override void UpdateState ()
		{
			m_Ship.MoveSurrond ();

			Vector3 plane2TargetDirection = m_Ship.m_StarTo.transform.position - m_Ship.transform.position;
			Vector3 planeDirection = m_Ship.m_CurrentDirection;
			if (CanResumeMove (plane2TargetDirection, planeDirection)) 
			{
				m_Ship.m_SurrondStar = null;
				m_StateManager.ChangeStateTo(ShipState.Fly);
				m_Ship.transform.parent = ShipElement.ORIGIN_PARENT;
				m_StateManager.UpdateState();			
			}
		}
	}

	//Can change to Hide
	private class ShipLandState:ShipStateBase
	{
		public ShipLandState(StateManager stateManager,ShipElement ship):base(stateManager,ship){}

		public override ShipState m_ShipState {
			get {
				return ShipState.Land;
			}
		}

		public override void UpdateState ()
		{
			m_StateManager.ChangeStateTo (ShipState.Hide);
		}
	}

	private class ShipHideState:ShipStateBase
	{
		public ShipHideState(StateManager stateManager,ShipElement ship):base(stateManager,ship){}

		public override ShipState m_ShipState {
			get {
				return ShipState.Hide;
			}
		}

		public override void UpdateState ()
		{
			m_Ship._Destroy ();
		}
	}

	public class StateManager
	{
		public ShipState m_ShipStateDesc
		{
			get
			{
				return m_ShipState.m_ShipState;
			}
		}
		private ShipStateBase m_ShipState;
		private StateManager(){}
		public StateManager(ShipElement ship)
		{
			m_ShipState = new ShipBornState(this,ship);
		}

		public void UpdateState(){
			m_ShipState.UpdateState ();
		}

		public void EnterTargetStar(Collider collider)
		{
			m_ShipState.EnterTargetStar (collider);		
		}

		public void EnterOtherStar(Collider collider)
		{
			m_ShipState.EnterOtherStar (collider);		
		}

		public void ChangeStateTo(ShipState shipState)
		{
			switch (shipState) {
			case ShipState.Born:
				m_ShipState = new ShipBornState(this,m_ShipState.m_Ship);
				break;
			case ShipState.Fly:
				m_ShipState = new ShipFlyState(this,m_ShipState.m_Ship);
				break;
			case ShipState.Surrond:
				m_ShipState = new ShipSurrondState(this,m_ShipState.m_Ship);
				break;
			case ShipState.Land:
				m_ShipState = new ShipLandState(this,m_ShipState.m_Ship);
				break;
			case ShipState.Hide:
				m_ShipState = new ShipHideState(this,m_ShipState.m_Ship);
				break;
			}
		}
	}
	#endregion

	public static readonly float MIN_RADIAN = 0.98f; //10 degree
	public static readonly Transform ORIGIN_PARENT = null;
	public StarElement m_StarFrom;
	public StarElement m_StarTo;
	public StarElement m_SurrondStar;
	public ShipSize m_ShipSize;
	public ShipFunc m_ShipFunc;
	public int m_ShipTroopNum;
	public StateManager m_StateManager;

	public float m_FlySpeed;
	public float m_SurrondSpeed;
	public int m_SurrondDirection = 1;
	public Vector3 m_CurrentDirection
	{
		get
		{
			return this.transform.forward;
		}

		set
		{
			this.transform.forward = value.normalized;
		}
	}

	public Vector3 m_Direction2TargetNormalized
	{
		get
		{
			return (m_StarTo.transform.position - this.transform.position).normalized;
		}
	}

	public virtual void _Init()
	{
		this.m_StateManager = new StateManager (this);
	}

	protected virtual void _Update()
	{
		this.m_StateManager.UpdateState ();
	}

	protected virtual void MoveToTarget()
	{
		this.m_CurrentDirection = m_Direction2TargetNormalized;
		this.transform.position = this.transform.position + m_Direction2TargetNormalized * Time.deltaTime * m_FlySpeed;
	}

	protected virtual void MoveSurrond()
	{
		Vector3 crossV1 = m_SurrondStar.transform.position - this.transform.position;
		Vector3 crossV2 = new Vector3 (crossV1.x, crossV1.y + 1, crossV1.z); //this.transform.up;
		Vector3 crossResult = Vector3.Cross (crossV1, crossV2) * m_SurrondDirection;
		this.m_CurrentDirection = crossResult;
		this.transform.position = this.transform.position + this.m_CurrentDirection * Time.deltaTime * m_FlySpeed;
	}

	public void SetShipScaleBySize(){
		SetShipScaleBySize (m_ShipSize);
	}

	public virtual void SetShipScaleBySize(ShipSize shipSize){}

	protected virtual void _EnterTarget(Collider targetCollider)
	{
		this.m_StateManager.EnterTargetStar (targetCollider);
	}

	protected virtual void _EnterOther(Collider otherCollider)
	{
		this.m_StateManager.EnterOtherStar (otherCollider);
	}

	public virtual void _TriggerEnter(Collider collider)
	{
		
	}

	public virtual void _TriggerExit(Collider collider)
	{

	}

	protected virtual void _Destroy()
	{
		Destroy (this.gameObject);
	}
}