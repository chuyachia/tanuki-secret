using System.Collections.Generic;
using UnityEngine;

public class SquirrelLevelManager : MonoBehaviour
{

    [SerializeField] private List<GameObject> _nutPrefabs;
    [SerializeField] private List<GameObject> nutBuckets;
    [SerializeField] private GameObject _squirrelPrefab;
    [SerializeField] private float _nutSpawnRadius = 5f;
    [SerializeField] private int _squirrelCount = 5;
    [SerializeField] private int _nutCount = 5;
    [SerializeField] private float _nextNutAppearsIn = 2f;
    [SerializeField] private Door _doorToNextLevel;
    [SerializeField] private float _bucketDistance = 3f;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _exitGatherPoint;
    [SerializeField] private float _distToGatherPoint;



    private List<ObjectPool> _nutPools;
    private List<GameObject> _squirrels;
    private HashSet<int> _playerPutCorrectBucket;

    private Queue<GameObject> _nutsToAssign;
    private Dictionary<int, GameObject> _squirrelToTarget;
    private Dictionary<int, int> _nutIdToNutType;
    private float _nextNutTimer = 0f;
    private int _nutsToSpawn = 0;
    private int _playerTargetBucketId = -1;
    private bool _levelFinished;

    void Start()
    {
        _nutPools = new List<ObjectPool>();
        foreach (GameObject nutPrefab in _nutPrefabs)
        {
            _nutPools.Add(new ObjectPool(nutPrefab));
        }
        _nutsToAssign = new Queue<GameObject>();
        _nutIdToNutType = new Dictionary<int, int>();
        _squirrelToTarget = new Dictionary<int, GameObject>();
        _squirrels = new List<GameObject>();
        _playerPutCorrectBucket = new HashSet<int>();
        for (int i = 0; i < _nutCount; i++)
        {
            int nutType;
            GameObject newNut = AddToSceneFromPool(_nutPools, out nutType);
            _nutsToAssign.Enqueue(newNut);
            AddOrUpdateDict(newNut.GetInstanceID(), nutType, _nutIdToNutType);
        }

        for (int i = 0; i < _squirrelCount; i++)
        {
            GameObject squirrel = Instantiate(_squirrelPrefab, GetRandomPositionWithinCircle(0f, _nutSpawnRadius), Quaternion.identity);
            squirrel.transform.parent = transform;
            _squirrels.Add(squirrel);
            AssignAvailableNutToSquirrel(i);
        }

        float totalLength = (_nutPrefabs.Count - 1) * _bucketDistance;
        float startOffset = -totalLength / 2;
        EventManager.Instance.RegisterSquirrelLevelEventListener(HandleEvent);
    }

    void OnDestroy()
    {
        EventManager.Instance.UnregisterSquirrelLevelEventListener(HandleEvent);
    }

    void SquirrelGatherAroundExit()
    {
        List<Vector3> positions = Utils.GetCircleAroundTargetPosition(_squirrels.Count, _exitGatherPoint.transform.position, 90f, 180f, 5f);
        for (int i = 0; i < _squirrels.Count; i++)
        {
            _squirrels[i].GetComponent<SquirrelBehaviour>().StayAtPoint = positions[i];
            HideNutInMouth(_squirrels[i]);
        }
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
            else if (Utils.DistanceToTargetWithinThreshold(currentSquirrel.transform.position, currentTarget.transform.position, currentSquirrel.GetComponent<SquirrelBehaviour>().TargetReachedThreshold))
            {
                if (currentTarget.CompareTag(Constants.Tags.Nut))
                {
                    if (_nutIdToNutType.TryGetValue(currentTarget.GetInstanceID(), out int nutType))
                    {
                        ShowNutInMouth(currentSquirrel, nutType);
                        AssignTargetToSquirrel(nutBuckets[nutType], i);
                        RemoveFromScene(currentTarget, _nutPools[nutType]);
                    }
                    else
                    {
                        AssignAvailableNutToSquirrel(i);
                    }
                }
                else if (currentTarget.CompareTag(Constants.Tags.NutBucket))
                {
                    HideNutInMouth(currentSquirrel);
                    AssignAvailableNutToSquirrel(i);
                }
            }
            // else
            // {
            //     currentSquirrel.GetComponent<SquirrelBehaviour>().Target = currentTarget;
            // }
        }
    }

    void ShowNutInMouth(GameObject squirrel, int nutType)
    {
        Utils.ActivateChild(squirrel.transform, 1 + nutType, Constants.Tags.NutInMouth);
    }

    void HideNutInMouth(GameObject squirrel)
    {
        foreach (Transform child in squirrel.transform)
        {
            if (child.CompareTag(Constants.Tags.NutInMouth))
            {
                child.gameObject.SetActive(false);
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
                    int nutType;
                    GameObject newNut = AddToSceneFromPool(_nutPools, out nutType);
                    _nutsToAssign.Enqueue(newNut);
                    AddOrUpdateDict(newNut.GetInstanceID(), nutType, _nutIdToNutType);
                    _nutsToSpawn--;
                }
            }
        }
    }

    void Update()
    {
        if (!_levelFinished)
        {
            UpdateSquirrelTarget();
            SpawnNewNuts();
        }
    }

    GameObject AddToSceneFromPool(List<ObjectPool> pools, out int nutType)
    {
        nutType = Random.Range(0, pools.Count);
        GameObject instance = pools[nutType].Get();
        instance.transform.position = GetRandomPositionWithinCircle(0, _nutSpawnRadius);
        instance.transform.parent = transform;
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

    public void HandleEvent(GameObject[] target, EventManager.SquirelLevelEvent eventType)
    {
        switch (eventType)
        {
            case EventManager.SquirelLevelEvent.PickUpNut:
                {
                    OnNutCollectedByPlayer(target[0]);
                    break;
                }
            case EventManager.SquirelLevelEvent.PutNutInBucket:
                {
                    OnPlayerPutNutToBucket(target[0]);
                    break;
                }
        }
    }

    private void OnNutCollectedByPlayer(GameObject nut)
    {
        GameObject targetBucket = null;
        if (_nutIdToNutType.TryGetValue(nut.GetInstanceID(), out int nutType))
        {
            targetBucket = nutBuckets[nutType];
            RemoveFromScene(nut, _nutPools[nutType]);
        }
        if (targetBucket != null)
        {
            _playerTargetBucketId = targetBucket.GetInstanceID();
        }
        else
        {
            _playerTargetBucketId = nutBuckets[0].GetInstanceID();
        }

    }

    private void OnPlayerPutNutToBucket(GameObject bucket)
    {
        if (bucket.GetInstanceID() == _playerTargetBucketId)
        {
            EventManager.Instance.InvokeSquirrelLevelEvent(new GameObject[] { bucket }, EventManager.SquirelLevelEvent.CorrectBucket);
            _playerPutCorrectBucket.Add(bucket.GetInstanceID());
            if (_playerPutCorrectBucket.Count == nutBuckets.Count)
            {
                _doorToNextLevel.ToggleDoor(true);
                SquirrelGatherAroundExit();
                _levelFinished = true;
            }
        }
        else
        {
            EventManager.Instance.InvokeSquirrelLevelEvent(new GameObject[] { bucket }, EventManager.SquirelLevelEvent.WrongBucket);
        }
    }
}
