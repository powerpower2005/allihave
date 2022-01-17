using System;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsToUnity;

[CreateAssetMenu(fileName = "GSTU", menuName = "THREE RABBIT PACKAGE/GSTU_Data", order = int.MaxValue)]
//Scriptable로 선언하여 값복사하는 오버헤드 줄임.
public class GSTU : ScriptableObject
{
	public string associatedSheet; //해당 sheet
	public string associatedWorksheet; //작업할 sheet
	public string[] coloumId; //column으로 접근


	[System.Serializable]
	public struct Data
	{
		public string key;
		public string[] data;
	}

	public List<Data> data = new List<Data>();


	/// <summary>
    /// key를 이용하여
    /// </summary>
	public string GetData(string key, int coloumId)
	{

		for (int i = 0; i < data.Count; i++)
		{
			if (data[i].key == key)
			{
				return data[i].data[coloumId];
			}
		}

		return null;
	}


}