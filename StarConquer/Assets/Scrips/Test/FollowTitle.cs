using UnityEngine;
using System.Collections;

public class FollowTitle : MonoBehaviour {
	public GameObject m_RootElement;
	public GameObject m_LabelPrefab;
	public GameObject m_CirclePrefab;
	public Camera m_UICamera;
	public UISlider m_Slider;
	public UILabel m_NumLabel;
	public StarElement[] m_StarElements;
	public UILabel[] m_TitleElements;
	public GameObject[] m_CircleElements;
	public float m_ScaleParam = 1.0f;
	public bool m_IfTest = true;

	private int m_StarFrom = -1;
	private int m_StarTo = -1;

	public int m_ShipTroopNum
	{
		get{
			if(m_StarFrom != -1)
			{
				return (int)((float)(m_StarElements[m_StarFrom].m_TroopNum) * m_Slider.value);
			}
			return -1;
		}
	}

	void Start () {
		GameObject[] starArray = GameObject.FindGameObjectsWithTag("Star");
		if (starArray != null) 
		{
			m_StarElements = new StarElement[starArray.Length];
			m_TitleElements = new UILabel[starArray.Length];
			m_CircleElements = new GameObject[starArray.Length];
			for(int i=0;i<m_StarElements.Length;i++)
			{
				m_StarElements[i] = starArray[i].GetComponent<StarElement>();
				m_TitleElements[i] = (Instantiate(m_LabelPrefab) as GameObject).GetComponent<UILabel>();
				m_CircleElements[i] = Instantiate(m_CirclePrefab) as GameObject;
			}
			UpdateTitlesPosition();
		}
	}
	
	void Update () 
	{
		for (int i=0; i<Input.touchCount; i++) {
			Touch touch = Input.GetTouch(i);
			if(touch.phase == TouchPhase.Began)
			{
				Debug.Log(touch.position);
			}
		}

		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray,out hit))
			{

			}
			else
			{
			}
		}

		if (Input.GetMouseButton (0)) 
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray,out hit))
			{
				int selectIndex = GetIndexByObj(hit.transform.gameObject);
				if(selectIndex != -1)
				{
					if(m_StarFrom == -1)
					{
						m_StarFrom = selectIndex;
						m_CircleElements[m_StarFrom].gameObject.SetActive(true);
					}
					else
					{
						if(m_StarFrom != selectIndex)
						{
							if(m_StarTo == -1)
							{
								m_StarTo = selectIndex;
								m_CircleElements[m_StarTo].gameObject.SetActive(true);
							}
							else
							{
								if(m_StarTo != selectIndex)
								{
									m_CircleElements[m_StarTo].gameObject.SetActive(false);
									m_StarTo = selectIndex;
									m_CircleElements[m_StarTo].gameObject.SetActive(true);
								}
							}
						}
					}
				}
			}
			else
			{
				if(m_StarTo != -1)
				{
					m_CircleElements[m_StarTo].gameObject.SetActive(false);
					m_StarTo = -1;
				}
			}
		}

		if (Input.GetMouseButtonUp (0)) 
		{
			if(m_StarFrom != -1 && m_StarTo != -1) //Success
			{
				m_StarElements[m_StarFrom].SupportStar(m_StarElements[m_StarTo],m_ShipTroopNum);
				m_CircleElements[m_StarFrom].SetActive(false);
				m_CircleElements[m_StarTo].SetActive(false);
				m_StarFrom = m_StarTo = -1;
			}
			else if(m_StarFrom != -1 && m_StarTo == -1) //Only have from
			{
				m_CircleElements[m_StarFrom].gameObject.SetActive(false);
				m_StarFrom = -1;
			}
			else if(m_StarFrom == -1 && m_StarTo != -1)
			{
				Debug.Break();
			}
			else if(m_StarFrom == -1 && m_StarTo == -1)
			{}
		}

		if (m_StarFrom != -1) 
		{
			if(m_NumLabel.gameObject.activeSelf == false)
				m_NumLabel.gameObject.SetActive(true);
			m_NumLabel.text = m_ShipTroopNum.ToString();
		}
		else
		{
			m_NumLabel.gameObject.SetActive(false);
		}
	}

	void LateUpdate(){
		UpdateTitlesPosition ();
	}

	int GetIndexByObj(GameObject obj){
		int index = -1;
		for (int i=0; i<m_StarElements.Length; i++) {
			if(obj.transform.parent != null){
				if(obj.transform.parent.gameObject == m_StarElements[i].gameObject)
				{
					index = i;
					break;
					
				}
			}
		}
		return index;
	}

	private void UpdateTitlesPosition()
	{
		if (m_StarElements != null && m_TitleElements != null) {
			for (int i=0; i<m_StarElements.Length; i++) {
				m_TitleElements[i].text = m_StarElements[i].m_TroopNum.ToString();
				Vector3 starPoint = m_StarElements[i].transform.position;
				Vector3 screenPointTitle = Camera.main.WorldToScreenPoint(new Vector3(starPoint.x+4*m_StarElements[i].transform.localScale.x,starPoint.y,starPoint.z));
				Vector3 screenPointCircle = Camera.main.WorldToScreenPoint(starPoint);
				m_TitleElements[i].transform.position  = m_UICamera.ScreenToWorldPoint(new Vector3(screenPointTitle.x,screenPointTitle.y,1));
				m_CircleElements[i].transform.position = m_UICamera.ScreenToWorldPoint(new Vector3(screenPointCircle.x,screenPointCircle.y,1));
				if(m_IfTest)
				{
					m_CircleElements[i].transform.localScale = Vector3.one * m_ScaleParam * 1.5f;
				}
				else
				{
					m_CircleElements[i].transform.localScale = m_ScaleParam * m_StarElements[i].transform.localScale;
				}
			}
		}
	}
}
