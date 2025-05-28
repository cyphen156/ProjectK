using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSpawner : MonoBehaviour
{
    //해당 장소에 효과 발생
    //풀링으로 관리
    [SerializeField]
    private SoundObject soundPrefab;
    [SerializeField]
    private AudioClip soundClip;

    private Queue<SoundObject> readyPool;
    private int poolCount;

    private float playRate; //다음 이펙트까지 시간
    private bool isReadyToPlay;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            PlaySound();
        }
    }

    private void Awake()
    {
        playRate = 0.2f;
        isReadyToPlay = true;
        poolCount = 3;
        MakeSoundPool();
        
    }

    private void MakeSoundPool()
    {
        readyPool = new();
        for (int i = 0; i < poolCount; i++)
        {
            SoundObject effectObject = Instantiate(soundPrefab, transform);
            effectObject.SetInfo(this, soundClip);
            readyPool.Enqueue(effectObject);
        }
    }

    public void PlaySound()
    {
        if (isReadyToPlay == false)
        {
            return;
        }
        if (readyPool.TryDequeue(out SoundObject effectObject))
        {
            effectObject.PlaySound();
            StartCoroutine(Timer());
            return;
        }

    }

    public void Recycle(SoundObject inEffectObject)
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
