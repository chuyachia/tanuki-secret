using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquirrelLevelManager : MonoBehaviour
{

    [SerializeField] private GameObject _nutPrefab;
    [SerializeField] private GameObject _squirrelPrefab;
    [SerializeField] private GameObject _nutBucketPrefab;
    [SerializeField] private float _nutSpawnRadius = 5f;
    [SerializeField] private int _squirrelCount = 5;
    [SerializeField] private int _nutCount = 5;
    [SerializeField] private float _targetReachedThreshold = 4f;
    [SerializeField] private float _nextNutAppearsIn = 2f;
    [SerializeField] private Door _doorToNextLevel;
    [SerializeField] private List<Material> _typeOfNuts;
    [SerializeField] private float _bucketDistance = 3f;

    private ObjectPool _nutPool;
    private List<GameObject> _squirrels;
    private List<GameObject> _buckets;

    private Queue<GameObject> _nutsToAssign;
    private Dictionary<int, GameObject> _squirrelToTarget;
    private Dictionary<int, int> _nutIdToBucket;
    private float _nextNutTimer = 0f;
    private int _nutsToSpawn = 0;

    void Start()
    {
        _nutPool = new ObjectPool(_nutPrefab);
        _nutsToAssign = new Queue<GameObject>();
        _nutIdToBucket = new Dictionary<int, int>();
        _squirrelToTarget = new Dictionary<int, GameObject>();
        _squirrels = new List<GameObject>();
        _buckets = new List<GameObject>();

        for (int i = 0; i < _nutCount; i++)
        {
            _nutsToAssign.Enqueue(AssignNutType(AddToSceneFromPool(_nutPool)));
        }

        for (int i = 0; i < _squirrelCount; i++)
        {
            GameObject squirrel = Instantiate(_squirrelPrefab, GetRandomPositionWithinCircle(0f, _nutSpawnRadius), Quaternion.identity);
            squirrel.transform.parent = transform;
            _squirrels.Add(squirrel);
            AssignAvailableNutToSquirrel(i);
        }

        float totalLength = (_typeOfNuts.Count - 1) * _bucketDistance;
        float startOffset = -totalLength / 2;
        for (int i = 0; i < _typeOfNuts.Count; i++)
        {
            Vector3 position = transform.position + new Vector3(startOffset + i * _bucketDistance, 0, 0);
            GameObject bucket = Instantiate(_nutBucketPrefab, position, Quaternion.identity);
            bucket.transform.parent = transform;
            _buckets.Add(bucket);
        }
        EventManager.Instance.RegisterGetNutEventListener(OnNutCollectedByPlayer);
        EventManager.Instance.RegisterPutNutInBucketEventListener(OpenDoorToNextLevel);
    }

    void OnDestroy()
    {
        EventManager.Instance.UnregisterGetNutEventListener(OnNutCollectedByPlayer);
        EventManager.Instance.UnregisterPutNutInBucketEventListener(OpenDoorToNextLevel);
    }

    void UpdateSquirrelTarget()
    {
        for (int i = 0; i < _squirrels.Count; i++)
        {
            GameObject currentSquirrel = _squirrels[i];
            GameObject currentTarget = _squirrelToTarget[i];
            if (currentTarget == null || !currentTarget.activeSelf)
            {
                AssignAvailableNutToSquirrel(i);
            }
            else if ((Utils.StripYDimension(currentTarget.transform.position) - Utils.StripYDimension(currentSquirrel.transform.position)).sqrMagnitude < _targetReachedThreshold)
            {
                if (currentTarget.CompareTag("Nut"))
                {
                    RemoveFromScene(currentTarget, _nutPool);
                    if (_nutIdToBucket.TryGetValue(currentTarget.GetInstanceID(), out int bucketId))
                    {
                        AssignTargetToSquirrel(_buckets[bucketId], i);
                    }
                    else
                    {
                        AssignAvailableNutToSquirrel(i);

                    }
                }
                else // reached bucket
                {
                    AssignAvailableNutToSquirrel(i);
                }
            }
        }
    }

    void AssignAvailableNutToSquirrel(int squirrelIndex)
    {
        GameObject newTarget = GetAvailableTargetNut();
        AssignTargetToSquirrel(newTarget, squirrelIndex);
    }

    void AssignTargetToSquirrel(GameObject target, int squirrelIndex)
    {
        GameObject currentSquirrel = _squirrels[squirrelIndex];
        AddOrUpdateDict(squirrelIndex, target, _squirrelToTarget);
        SquirrelBehaviour squirrelBehaviour = currentSquirrel.GetComponent<SquirrelBehaviour>();
        squirrelBehaviour.Target = target;
    }

    void AddOrUpdateDict<K, V>(K key, V value, Dictionary<K, V> dict)
    {
        if (!dict.ContainsKey(key))
        {
            dict.Add(key, value);
        }
        else
        {
            dict[key] = value;
        }
    }

    GameObject GetAvailableTargetNut()
    {
        GameObject targetNut;
        if (!_nutsToAssign.TryDequeue(out targetNut))
        {
            // Use same target as other if no unassigned nut
            // var nonNullTargets = _squirrelToTarget.Values.AsEnumerable().Where(item => item != null && item.CompareTag("Nut")).ToList();
            // if (!nonNullTargets.Any())
            // {
            //     targetNut = null;
            // }
            // else
            // {
            //     int targetIndex = Random.Range(0, nonNullTargets.Count);
            //     targetNut = nonNullTargets[targetIndex];
            // }
        }
        return targetNut;
    }

    void SpawnNewNuts()
    {

        if (_nutsToSpawn <= 0)
        {
            return;
        }
        else
        {
            _nextNutTimer -= Time.deltaTime;
            if (_nextNutTimer < 0)
            {
                while (_nutsToSpawn > 0)
                {
                    _nutsToAssign.Enqueue(AssignNutType(AddToSceneFromPool(_nutPool)));
                    // assign new nut to a random squirrel?
                    _nutsToSpawn--;
                }
            }
        }
    }

    void Update()
    {
        UpdateSquirrelTarget();
        SpawnNewNuts();
    }

    GameObject AssignNutType(GameObject nut)
    {
        Renderer renderer = nut.GetComponent<Renderer>();
        int nutType = Random.Range(0, _typeOfNuts.Count);
        if (renderer != null)
        {
            renderer.material = _typeOfNuts[nutType];
        }
        AddOrUpdateDict(nut.GetInstanceID(), nutType, _nutIdToBucket);
        return nut;
    }

    GameObject AddToSceneFromPool(ObjectPool pool)
    {
        GameObject instance = pool.Get();
        instance.transform.position = GetRandomPositionWithinCircle(0, _nutSpawnRadius);
        instance.transform.parent = transform;
        // TODO adjust rotation based on real model
        // instance.transform.rotation = Utils.RandomRotationRoundY();
        return instance;
    }


    Vector3 GetRandomPositionWithinCircle(float minDistance, float maxDistance)
    {
        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Random.Range(minDistance, maxDistance);
        float angleInRadians = randomAngle * Mathf.Deg2Rad;
        float x = transform.position.x + randomDistance * Mathf.Cos(angleInRadians);
        float z = transform.position.z + randomDistance * Mathf.Sin(angleInRadians);
        return new Vector3(x, transform.position.y, z);
    }

    void RemoveFromScene(GameObject instance, ObjectPool pool)
    {
        pool.Reclaim(instance);
        _nutsToSpawn++;
        if (_nextNutTimer <= 0)
        {
            _nextNutTimer = _nextNutAppearsIn;
        }
    }

    public void OnNutCollectedByPlayer(GameObject nut)
    {
        RemoveFromScene(nut, _nutPool);
    }

    public void OpenDoorToNextLevel()
    {
        _doorToNextLevel.ToggleDoor(true);
    }
}
