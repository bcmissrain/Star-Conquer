using UnityEngine;
using System.Collections;

public class TestManager : MonoBehaviour {
	public TestStar[] starArray;

	// Use this for initialization
	void Start () {
		GameObject[] starObjArray = GameObject.FindGameObjectsWithTag ("Star");
		starArray = new TestStar[starObjArray.Length];
		for (int i=0; i<starArray.Length; i++) 
		{
			starArray[i] = starObjArray[i].GetComponent<TestStar>();			
		}
	}
	
	// Update is called once per frame
	void Update () {
		foreach (TestStar s in starArray) 
		{
			s._Update(this);
		}

		for (int i=0; i<starArray.Length; i++) {
			//starArray[i].SupportStar(starArray[GetRandomIndex(i)]);
		}
	}

	public int GetRandomIndex(int index)
	{
		if (starArray.Length == 0) {
			throw new UnityException();		
		}

		int resultIndex = Random.Range (0, starArray.Length);
		if (resultIndex == index) {
			resultIndex = (index + 1)%starArray.Length;		
		}
		return resultIndex ;
	}
}
