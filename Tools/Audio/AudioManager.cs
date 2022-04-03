using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum AudioSourceKey
{
    Touch,
    UnitCreate,
    UnitDestroy,
    Reinforce,
    Coin,
    EnemyCreate,
    EnemyDestroy,
    Hit,
    None,
}




/// <summary>
/// 모든 AudioSource를 가지고 있음.
/// </summary>
public class AudioManager : MonoBehaviour
{


    public AudioPoolSetting[] audioPoolSettings;
    public Dictionary<AudioSourceKey, AudioPool> pools;


    //Public 으로 선언되어서 key, Source, count를 입력받음.
    [System.Serializable]
    public class AudioPoolSetting
    {
        public AudioSourceKey key;
        public AudioClip clip;
        public int count;

    }

    //해당 오디오는 전역으로 접근 가능하게 만듬.
    private void Awake()
    {
        if (G.Manager.audio != null)
        {
            Destroy(gameObject);
            return;
        }
        G.Manager.audio = this;
        DontDestroyOnLoad(gameObject);

        pools = new Dictionary<AudioSourceKey, AudioPool>();


        //사용자가 입력한 audioPoolSetting을 바탕으로 AudioSource를 가지는 오브젝트 생성
        //-> 해당 AudioSource를 AudioPool로 접근하여 Clip을 실행할 수 있음.
        foreach (var setting in audioPoolSettings)
        {
            //실행 시 해당 소리를 담당할 오브젝트를 만들고
            var go = new GameObject();
            go.name = setting.key.ToString();
            go.transform.SetParent(this.transform);

            //소스를 입력한 만큼 하위에 만듦
            AudioSource[] sources = new AudioSource[setting.count];
            for (int i = 0; i < setting.count; i++)
            {
                var g = new GameObject();
                g.name = "AudioPool";
                sources[i] = g.AddComponent<AudioSource>();
                sources[i].transform.SetParent(go.transform);
                sources[i].clip = setting.clip;
                sources[i].playOnAwake = false;

            }

            //AudioPool에 추가하여 접근가능하도록 함.
            //Dictionary에 담아서 key롤 해당 AudioPool을 등록함.
            pools.Add(setting.key, new AudioPool
            {
                _source = sources
            });

            pools[setting.key].SetUp();
        }

    }


    //AudioPool의 Play만으로도 실행할 수 있도록 함.
    public void Play(AudioSourceKey key)
    {
        pools[key].Play();
    }
}


//AudioSource를 가지고 있음.
[System.Serializable]
public class AudioPool
{
    public AudioSource[] _source;
    int seq = 0;

    public void SetUp()
    {
        seq %= _source.Length;
    }

    //AudioSource는 한 번에 하나의 소리밖에 재생하지 못하므로 같은 Clip을 가지는 여러 소스를 만들어 재생하게함.
    public void Play()
    {
        _source[seq++].Play();
        seq %= _source.Length;
    }

}







