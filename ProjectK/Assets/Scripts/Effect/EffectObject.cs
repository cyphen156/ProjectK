using UnityEngine;

public class EffectObject : MonoBehaviour
{
    [SerializeField] private float defaultLifeTime;
    private float lifeTime;
    private EffectSpawner effectMaker;

    private void Awake()
    {
        defaultLifeTime = 2f;
    }

    public void SetMaker(EffectSpawner inEffectMaker)
    {
        effectMaker = inEffectMaker;
    }

    public void PlayEffect()
    {
        lifeTime = defaultLifeTime;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
        {
            gameObject.SetActive(false);
            effectMaker.Recycle(this);
        }
    }
}
