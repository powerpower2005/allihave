using System.Collections;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Sprite로 애니메이션 만들기 without 애니메이터
/// </summary>
public class SpriteAnimator : MonoBehaviour
{

    public Sprite[] sprites; //연속 재생할 스프라이트

    public float frameDelay = 0.1f; //재생 딜레이
    public float routineDelay = 0; //해당 애니메이션 루프의 딜레이
    public float firstDelay = 0; //등장 딜레이


    public bool isHideWhenDelay = false;
    private SpriteRenderer spriteRenderer = null;
    private Image image = null;
    private Coroutine routineIeUpdate = null;


    //해당 Animation은 Active상태가 될 때 자동으로 진행
    private void OnEnable()
    {
        if (!gameObject.activeInHierarchy) return; //하지만 이 오브젝트가 Active더라도 상위 오브젝트가 비활성화면 x
        //Sprite가 0이 될 시에 애니메이션을 재생하면 Delay 없이 무한루프가 발생할 수 있으므로 체크
        if (sprites?.Length == 0)
        {
            Debug.LogWarning("[SpriteAnimator] sprites?.Length == 0");
            return;
        }

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (image == null) image = GetComponent<Image>();
        if(routineIeUpdate!=null) StopCoroutine(routineIeUpdate);
        StartCoroutine(RoutineUpdate());

        //실제 애니메이션 동작
        IEnumerator RoutineUpdate()
        {
            var delay = new WaitForSeconds(frameDelay);
            var delay2 = new WaitForSeconds(routineDelay);

            //등장 딜레이
            if (firstDelay > 0)
            {
                yield return new WaitForSeconds(firstDelay);
            }

            //애니메이션 시작
            while (true)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    if(spriteRenderer!=null)
                        spriteRenderer.sprite = sprites[i ];
                    if(image!=null)
                        image.sprite = sprites[i ];
                    yield return delay;
                }

                //해당 애니메이션이 끝나고 루틴이 언제 다시 시작될 지 결정.
                if (routineDelay > 0)
                {
                    if (isHideWhenDelay)
                    {
                        if (spriteRenderer != null)
                            spriteRenderer.enabled = false;
                        if (image != null)
                            image.enabled = false;
                    }

                    yield return delay2;

                    if (isHideWhenDelay)
                    {
                        if (spriteRenderer != null)
                            spriteRenderer.enabled = true;
                        if (image != null)
                            image.enabled = true;
                    }
                }
            }
        }
    }
}

