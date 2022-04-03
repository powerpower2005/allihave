using UnityEngine;

public class MessageManager : MonoBehaviour
{
    //Message 수
    public ViewMessage[] viewMessages;
    
        
    public int MessageCount { get; private set; }
    private void Awake()
    {
        //Singleton
        if (G.Manager.message != null)
        {
            Destroy(gameObject);
            return;
        }
        //전역으로 사용하기 위해 등록.
        G.Manager.message = this;
    }

    public void Open(string message,float time = 3f)
    {
        //메세지 애니메이션
        viewMessages[MessageCount].StartMessage(message,time);
        MessageCount = (MessageCount + 1) % viewMessages.Length;
    }
}

