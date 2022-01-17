using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using GoogleSheetsToUnity;


//GSTU에 대한 커스터마이징
[CustomEditor(typeof(GSTU))]
public class GSTUEditor : Editor
{
	//Scriptable Object
	GSTU gstuData;


	public void OnEnable()
	{
		gstuData = target as GSTU;
	}


	//Inpector창 커스터마이징
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		//Update버튼 생성 -> GoogleSpreadsheet의 변경 반영
		if (GUILayout.Button("Update"))
		{
			UpdateStats(UpdateMethod);
		}
	}

	/// <summary>
    /// 
    /// </summary>
	void UpdateStats(UnityAction<GstuSpreadSheet> callback, bool mergedCells = false)
	{
		SpreadsheetManager.Read(new GSTU_Search(gstuData.associatedSheet, gstuData.associatedWorksheet), callback, mergedCells);
	}


	/// <summary>
	/// GSTU가 참조하고 있는 sheet에 접근해서 데이터를 받아옴.
	/// </summary>
	void UpdateMethod(GstuSpreadSheet sheet)
	{
		gstuData.data.ForEach(delegate (GSTU.Data gstuData)
		{
			for (int j = 0; j < sheet.rows[gstuData.key].Count; j++)
			{
				gstuData.data[j] = sheet.rows[gstuData.key][j].value;
			}
		});

		EditorUtility.SetDirty(target);
	}
}
