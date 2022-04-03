using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ViewMessage : MonoBehaviour
{

    public Text textMessage; //보여주고자 하는 메세지
    public RectTransform rect; // 메세지가 보이는 위치

    public void StartMessage(string message, float time)
    {
        gameObject.SetActive(true); //해당 오브젝트를 true로 해야보임
        textMessage.text = message; //메세지
        StopAllCoroutines();
        StartCoroutine(IeMessage(time)); //애니메이션
        transform.SetAsFirstSibling(); //UI최상단에 놓기 위함
    }

    IEnumerator IeMessage(float time)
    {
        //사이즈 조절
        rect.sizeDelta=new Vector2(rect.sizeDelta.x,60f);
        rect.localScale = Vector3.one * 1.4f;
        //글자색 조절
        textMessage.color = new Color(1f,1f,1f,0f);


        //메세지 애니메이션 (생성)
        float t = 0f;
        float duration = 0.1f;
        while (t < duration)
        {
            float r = t / duration;
            rect.localScale = Vector3.one * (1.4f-(1f-r)*0.4f); //글자가 점점 커지는(1 -> 1.4)
            t += Time.unscaledDeltaTime; //unscaled로 독립된 시간임.
            textMessage.color = new Color(1f,1f,1f,r); //투명도도 점점 사라지는 0 -> 255
            yield return null;
        }

        //크기와 색상 조절 (유지)
        rect.localScale = Vector3.one;
        textMessage.color = new Color(1f,1f,1f,1f);
        yield return new WaitForSecondsRealtime(time);
        duration = 0.25f;
        t = 0f;

        //0.25초에 걸쳐서 작아짐(소멸)
        while (t < duration)
        {
            float r = t / duration;
            rect.sizeDelta=new Vector2(rect.sizeDelta.x,60f-r*60f); // -> 시간이 지날수록 r이 1에 가까워짐.
            textMessage.color = new Color(1f,1f,1f,1f-r); // -> Alpha값이 작아져 안보이게 됨.
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        //시간이 지나면 해당 글자 사라짐.
        gameObject.SetActive(false);
    }
}
