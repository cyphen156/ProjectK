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
        transform.SetParent(null, true); //별도로 생성
        lifeTime = defaultLifeTime;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
        {
            gameObject.SetActive(false);
            if(effectMaker != null)
            {
                effectMaker.Recycle(this);
                transform.SetParent(effectMaker.transform);
            }
            else
            {
                Destroy(gameObject);
            }
            
        }
    }
}
