using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundObject : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float defaultLifeTime;
    private float lifeTime;
    private SoundSpawner soundMaker;

    public void SetInfo(SoundSpawner inSoundMaker, AudioClip inClip)
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = inClip;
        soundMaker = inSoundMaker;
        defaultLifeTime = inClip.length;
    }

    public void PlaySound()
    {
        transform.SetParent(null, true);
        lifeTime = defaultLifeTime;
        gameObject.SetActive(true);
        audioSource.Play();
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            gameObject.SetActive(false);
            if (soundMaker != null)
            {
                soundMaker.Recycle(this);
                transform.SetParent(soundMaker.transform);
            }
            else
            {
                Destroy(gameObject);
            }
            
        }
    }
}
