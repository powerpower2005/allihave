using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 비동기 함수(서버 요청 응답의 순서를 보장하기 위한 EventQueue의 변형 버전)
/// </summary>
public class RequestQueueManager : MonoBehaviour
{
    //비동기 함수(Unitask를 넣는 큐)
    public Queue<UniTask> requestQueue;


    private bool IsPending=>requestQueue.Count>0; //큐에 남아있다면 할 일이 남은 것
    private bool _isDoingRequest = false; //현재 Request에 대한 Response를 받았는지

    private void Awake()
    {
        //싱글톤 등록
        if (G.Manager.reqManager != null)
            Destroy(gameObject);

        G.Manager.reqManager = this;
        requestQueue = new Queue<UniTask>();

        DontDestroyOnLoad(this);
    }

    public async UniTask Enqueue(UniTask cmd)
    {
        if (!_isDoingRequest&& !IsPending) {
            //큐가 비어있고 실행중인게 없다면 바로 실행.
            await Process(cmd);
        }
        else
        {   //해당 명령을 큐에 넣음
            requestQueue.Enqueue(cmd);
        }
    }
    async UniTask Process(UniTask cmd)
    {
        //상태를 실행중으로 바꿈.
        _isDoingRequest = true;
        await UniTask.Run(() => cmd);
        //실행끝
        _isDoingRequest = false;
        await DoNext();
    }

    //큐에서 꺼내는 작업.
    public async UniTask DoNext()
    {
        //큐가 비었으면 실행하지 않음.
        if (requestQueue.Count == 0)
            return;

        //큐에 있다면 해당 함수를 꺼내서 실행.
        var cmd = requestQueue.Dequeue();
        await Process(cmd);

    }

 
}
