using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ModelController : MonoBehaviour
{
    [SerializeField] private List<GameObject> modelPrefabs;
    private static Dictionary<Level, int> levelToModelIndex;

    private List<GameObject> models;
    public Animator Animator
    {
        get
        {
            if (models != null && currentModel < models.Count)
            {
                return models[currentModel].GetComponent<Animator>();
            }
            return null;
        }
    }

    private int currentModel = 0;
    void Start()
    {
        levelToModelIndex = new Dictionary<Level, int>
        {
            { Level.Squirrel, 1 },
            { Level.Deer, 2 },
            { Level.Crane, 3 }
        };
        models = new List<GameObject>();
        for (int i = 0; i < modelPrefabs.Count; i++)
        {
            models.Add(Instantiate(modelPrefabs[i]));
        }
        ChangeModel(0);
        EventManager.Instance.RegisterLevelEnterEventListener(OnLevelChange);
    }

    void OnDestroy()
    {
        EventManager.Instance.UnregisterLevelEnterEventListener(OnLevelChange);

    }

    void ChangeModel(int index)
    {
        currentModel = index;
        for (int i = 0; i < models.Count; i++)
        {
            GameObject model = models[i];
            if (i == currentModel)
            {
                model.SetActive(true);
                model.transform.SetParent(transform, false);
            }
            else
            {
                model.SetActive(false);
                model.transform.SetParent(null);
            }
        }
    }

    void OnLevelChange(Level level)
    {
        ChangeModel(levelToModelIndex[level]);
    }
}
