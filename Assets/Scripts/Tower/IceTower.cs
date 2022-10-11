using UnityEngine;

public class IceTower : Tower
{
    [Header("Ice Tower Settings")]
    [SerializeField] private ParticleSystem _iceRingEffect;
    [SerializeField] private float _freezingTime;

    [Header("Pool Settings")]
    [SerializeField] private int _effectPoolInitialCapacity;

    private static ObjectsPool<ParticleSystem> _effectPool;

    private Collider[] _colliders;

    private void Awake()
    {
        if (_effectPool != null)
            return;

        _effectPool = new ObjectsPool<ParticleSystem>(_iceRingEffect.gameObject, _effectPoolInitialCapacity);
    }

    private void OnEnable()
    {
        InvokeRepeating(CheckTargetsMethod, 0, 1 / UpdateTargetsPerFrame);
    }

    private void Update()
    {
        LastShootTime += Time.deltaTime;

        if (_colliders.Length <= 0)
            return;

        if (LastShootTime >= FiringRate)
        {
            Shot();

            LastShootTime = 0;
        }
    }

    protected override void CheckTargets()
    {
        _colliders = Physics.OverlapSphere(FirePoint.position, FireRange, EnemiesLayerMask);
    }

    protected override void Shot()
    {
        var effect = Helpers.GetEffectFromPool(_effectPool, FirePoint.position, Quaternion.identity);

        StartCoroutine(Helpers.DeactivateEffectWithDelay(effect));

        foreach (var collider in _colliders)
        {
            if (collider.gameObject.TryGetComponent(out Enemy enemy))
            {
                if (enemy.IsAlive)
                    enemy.Freeze(_freezingTime);
            }
        }
    }
}