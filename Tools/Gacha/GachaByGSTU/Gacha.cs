using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Gacha : Singleton<Gacha>
{
    //Scriptable Object (GSTU)
    public GSTU items;


    /// <summary>
    /// 해당 Scriptable Object에 있는 데이터에 대해서 확률대로 뽑기
    /// </summary>
    public void GetItem()
    {
        float curPercentage = UnityEngine.Random.Range(0f, 100f);
        Debug.Log(curPercentage);
        for (int i = 0; i < items.data.Count; i++)
        {
            float percent = float.Parse(items.data[i].data[3]);
            if (curPercentage < percent)
            {
                Debug.Log(items.data[i].data[2]);
                Debug.Log(items.data[i].data[1]);
                break;
            }
            curPercentage -= percent;
        }
        return;
    }
}
