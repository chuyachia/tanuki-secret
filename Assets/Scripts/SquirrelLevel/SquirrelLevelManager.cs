using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquirrelLevelManager : MonoBehaviour
{

    [SerializeField] private GameObject _nutPrefab;
    [SerializeField] private GameObject _squirrelPrefab;
    [SerializeField] private GameObject _nutBucketPrefab;
    [SerializeField] private float _spawnRadius = 5f;
    [SerializeField] private float _spawnHeight = 10f;
    [SerializeField] private int _squirrelCount = 5;
    [SerializeField] private int _nutCount = 5;
    [SerializeField] private float _targetReachedThreshold = 4f;
    [SerializeField] private float _nextNutAppearsIn = 2f;

    private ObjectPool _nutPool;
    private ObjectPool _squirrelPool;
    private List<GameObject> _squirrels;
    private Queue<GameObject> _nutsToAssign;
    private GameObject _nutBucket;
    private Dictionary<int, GameObject> _squirrelToTarget;
    private float _nextNutTimer = 0f;
    private int _nutsToSpawn = 0;

    void Start()
    {
        _nutPool = new ObjectPool(_nutPrefab);
        _squirrelPool = new ObjectPool(_squirrelPrefab);
        _squirrels = new List<GameObject>();
        _nutsToAssign = new Queue<GameObject>();
        _squirrelToTarget = new Dictionary<int, GameObject>();
        for (int i = 0; i < _nutCount; i++)
        {
            _nutsToAssign.Enqueue(AddToScene(_nutPool));
        }
        for (int i = 0; i < _squirrelCount; i++)
        {
            GameObject squirrel = AddToScene(_squirrelPool);
            _squirrels.Add(squirrel);
            AssignAvailableNutToSquirrel(i);
        }
        _nutBucket = Instantiate(_nutBucketPrefab, transform.position, Quaternion.identity);
        EventManager.Instance.RegisterGetNutEventListener(OnNutCollectedByPlayer);
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
                    AssignTargetToSquirrel(_nutBucket, i);
                }
                else
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
        AddOrUpdateTarget(squirrelIndex, target);
        SquirrelBehaviour squirrelBehaviour = currentSquirrel.GetComponent<SquirrelBehaviour>();
        squirrelBehaviour.Target = target;
    }

    void AddOrUpdateTarget(int squirrelIndex, GameObject target)
    {
        if (!_squirrelToTarget.ContainsKey(squirrelIndex))
        {
            _squirrelToTarget.Add(squirrelIndex, target);
        }
        else
        {
            _squirrelToTarget[squirrelIndex] = target;
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
                    GameObject newNut = AddToScene(_nutPool);
                    _nutsToAssign.Enqueue(newNut);
                    // assign new nut to a random squirrel
                    // int targetSquirrel = Random.Range(0, _squirrels.Count);
                    // AssignTargetToSquirrel(newNut, targetSquirrel);
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

    GameObject AddToScene(ObjectPool pool)
    {
        GameObject instance = pool.Get();
        Vector2 randomPosition = Random.insideUnitCircle * _spawnRadius;
        instance.transform.position = new Vector3(transform.position.x + randomPosition.x, _spawnHeight, transform.position.z + randomPosition.y);
        // TODO adjust rotation based on real model
        // instance.transform.rotation = Utils.RandomRotationRoundY();
        return instance;
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
        Debug.Log("Player picked a nut");
        RemoveFromScene(nut, _nutPool);
    }
}
