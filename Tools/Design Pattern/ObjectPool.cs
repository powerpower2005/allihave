using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using System;

[System.Serializable]
public struct ObjectData
{
    [Tooltip("오브젝트 풀의 부모")] [SerializeField] public Transform parent;
    [Tooltip("풀링할 게임 오브젝트")] [SerializeField] public GameObject gameObject;
    [Tooltip("미리 생성할 게임 오브젝트의 수")] [Range(0, 100)] [SerializeField] public int generatedCount;
}
[System.Serializable] public class ObjectDataDictionaryToString : SerializableDictionary<string, ObjectData> { }
public class ObjectPool : Singleton<ObjectPool>
{


    [BoxGroup("Setting to objectpool")]
    [Tooltip("오브젝트를 관리하는 풀의 이름을 키 값으로 사용합니다.")] [SerializeField] private ObjectDataDictionaryToString storage = null;
    [Tooltip("키 값으로 구현된 오브젝트 풀 컨테이너입니다.")] private Dictionary<string, Stack<GameObject>> objectPool = new Dictionary<string, Stack<GameObject>>();
    [Tooltip("키 값으로 구현된 활성화된 오브젝트 리스트입니다.")] private Dictionary<string, List<GameObject>> ActivePool = new Dictionary<string, List<GameObject>>();




    public void Start()
    {
        Init();
    }

    /// <summary>
    /// 초기화(Inspector 입력대로 초기화)
    /// </summary>
    public void Init()
    {
        foreach (var objectPool in storage)
        {
            Debug.Log(objectPool.Key);
            Create(objectPool.Key, objectPool.Value.generatedCount);
        }
    }

    /// <summary>
    /// key값으로 오브젝트 풀 리스트를 생성하여 count 값 만큼 storage에 저장 된 객체를 생성합니다.
    /// </summary>
    public void Create(string key, int count)
    {
        // 키 값이 중복 되거나, 없을 때 에러 출력
        if (objectPool.ContainsKey(key)) throw new ArgumentException("ObjectPool: (" + key + ") 중복 된 키 값을 사용하여 생성 할 수 없습니다.");
        else if (!storage.ContainsKey(key)) throw new KeyNotFoundException("ObjectPool: (" + key + ") 해당 값을 찾을 수 없기 때문에 생성할 수 없습니다. ObjectPool객체의 Storage를 확인하세요.");

        // 오브젝트의 부모 저장소를 생성
        GameObject storageName = new GameObject(key);
        storageName.transform.SetParent(storage[key].parent);

        // 키 값으로 오브젝트 풀 스택를 생성
        Stack<GameObject> stk = new Stack<GameObject>();
        objectPool.Add(key, stk);

        // 키 값으로 Active Pool 생성
        List<GameObject> lst = new List<GameObject>();
        ActivePool.Add(key, lst);



        // 풀링에 필요한 오브젝트를 count만큼 생성 후 Active비 활성화
        for (int idx = 0; idx < count; idx++)
        {
            GameObject temp = Instantiate(storage[key].gameObject, storageName.transform);
            temp.SetActive(false);
            objectPool[key].Push(temp);
        }

        //numOfObjects.Add(key, 0);
    }

    /// <summary>
    /// key값으로 오브젝트 풀 리스트를 검색하여 반환합니다.
    /// </summary>
    public Stack<GameObject> Get(string key)
    {
        if (!objectPool.ContainsKey(key)) throw new ArgumentException("ObjectPool: (" + key + ") 존재하지 않는 키 값을 사용하려 하고 있습니다. 이 오류의 호출 스택을 참고하거나 정확한 키 값을 입력하십시오.");

        return objectPool[key];
    }

    /// <summary>
    /// key값으로 오브젝트 풀을 검색하여 요소를 비 활성화 상태로 추가합니다.
    /// </summary>
    public void Add(string key)
    {
        if (!objectPool.ContainsKey(key)) throw new ArgumentException("ObjectPool: (" + key + ") 존재하지 않는 키 값을 사용하려 하고 있습니다. 이 오류의 호출 스택을 참고하거나 정확한 키 값을 입력하십시오.");
        GameObject storageName = GameObject.Find(key);
        storageName.transform.SetParent(storage[key].parent);

        GameObject temp = Instantiate(storage[key].gameObject, storageName.transform);
        temp.SetActive(false);
        objectPool[key].Push(temp);
        // numOfObjects[key]++;
    }

    /// <summary>
    /// key값으로 검색한 오브젝트 풀의 Active가 비 활성화 된 요소를 제거합니다.
    /// </summary>
    public void Remove(string key)
    {
        // 스택의 요소를 제거 한 후 실제 오브젝트를 제거합니다.
        if (objectPool[key].Count != 0)
        {
            GameObject obj = objectPool[key].Peek();
            objectPool[key].Pop();
            Destroy(obj);
            return;
        }
    }

    /// <summary>
    /// 오브젝트 풀에서 비활성화 된 요소를 반환합니다. -> 당연히 맨 위에 있음.
    /// </summary>
    public GameObject GetDisabledObject(string key)
    {
        if (objectPool[key].Count != 0)
        {
            GameObject obj = objectPool[key].Peek();
            objectPool[key].Pop();
            ActivePool[key].Add(obj);//활성화 된 오브젝트를 넣음.
            return obj;
        }

        return null;
    }
    /// <summary>
    /// 썼으면 다시 오브젝트 풀에 집어넣어야함.
    /// </summary>
    /// <param name="key">다시 스택에 집어넣을 풀 이름</param>
    /// <param name="obj">해당되는 오브젝트</param>
    public void GetBackObject(string key, GameObject obj)
    {
        objectPool[key].Push(obj);

        //AcitvePool에서도 지워줘야 한다.
        foreach (GameObject x in ActivePool[key])
        {
            Debug.Log(x);
            if (x.GetInstanceID() == obj.GetInstanceID())
            {
                ActivePool[key].Remove(x);
                break;
            }
        }
        return;
    }


    /// <summary>
    /// 해당 Key에 대한 Active Pool의 숫자를 반환한다.-> 활성화된 오브젝트의 수를 반환한다.
    /// </summary>

    public int GetNumberOfActiveObjects(string key)
    {
        return ActivePool[key].Count;
    }

    /// <summary>
    /// 해당 Key에 대한 Active Pool을 반환받는다. -> 활성화된 오브젝트에 대한 접근이나 연산을 위함.
    /// </summary>

    public List<GameObject> GetActivePool(string key)
    {
        return ActivePool[key];
    }

}