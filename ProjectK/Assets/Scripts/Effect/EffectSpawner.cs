using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
    //해당 장소에 효과 발생
    //풀링으로 관리
    [SerializeField]
    private EffectObject effectPrefab;

    private Queue<EffectObject> readyPool;
    private int poolCount;

    private float playRate; //다음 이펙트까지 시간
    private bool isReadyToPlay;

    private void Awake()
    {
        MakeEffectPool();
        playRate = 0.2f;
        isReadyToPlay = true;
    }

    private void MakeEffectPool()
    {
        poolCount = 3;
        readyPool = new();
        for (int i = 0; i < poolCount; i++)
        {
            EffectObject effectObject = Instantiate(effectPrefab, transform);
            effectObject.SetMaker(this);
            readyPool.Enqueue(effectObject);
        }
    }

    public void PlayEffect()
    {
        if(isReadyToPlay == false)
        {
            return;
        }
        if(readyPool.TryDequeue(out EffectObject effectObject))
        {
            effectObject.PlayEffect();
            StartCoroutine(Timer());
            return;
        }

    }

    public void Recycle(EffectObject inEffectObject)
    {
        readyPool.Enqueue(inEffectObject);
    }

    IEnumerator Timer()
    {
        isReadyToPlay = false;
        yield return new WaitForSeconds(playRate);
        isReadyToPlay = true;

    }
}
