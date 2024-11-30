using System.Collections.Generic;
using UnityEngine;

public class ModelController : MonoBehaviour
{
    [SerializeField] private List<GameObject> modelPrefabs;
    [SerializeField] private Level defaultLevel;

    private Dictionary<Level, GameObject> levelToModel;
    private CraneDanceAnimator craneDanseAnimator;

    public Animator BodyAnimator
    {
        get
        {
            return craneDanseAnimator?.BodyAnimator;
        }
    }

    public Animator WingAnimator
    {
        get
        {
            return craneDanseAnimator?.WingAnimator;
        }
    }

    public Animator Animator
    {
        get
        {
            Animator animator = null;
            if (levelToModel[currentLevel] != null)
            {
                levelToModel[currentLevel].TryGetComponent<Animator>(out animator);
            }
            return animator;
        }
    }

    private Level currentLevel = Level.Base;
    void Start()
    {
        List<GameObject> models = new List<GameObject>();
        foreach (GameObject prefab in modelPrefabs)
        {
            GameObject model = Instantiate(prefab);
            model.transform.SetParent(transform, false);
            models.Add(model);
        }
        levelToModel = new Dictionary<Level, GameObject>{
            {Level.Base, models[0]},
            {Level.Squirrel, models[1]},
            {Level.Deer, models[2]},
            {Level.Crane, models[3]}
        };
        ChangeModel(defaultLevel);
        EventManager.Instance.RegisterLevelEnterEventListener(OnLevelChange);
    }

    void OnDestroy()
    {
        EventManager.Instance.UnregisterLevelEnterEventListener(OnLevelChange);

    }

    void ChangeModel(Level level)
    {
        currentLevel = level;
        foreach (KeyValuePair<Level, GameObject> kvp in levelToModel)
        {
            GameObject model = kvp.Value;
            if (kvp.Key == currentLevel)
            {
                model.SetActive(true);
            }
            else
            {
                model.SetActive(false);
            }
        }

        if (level.Equals(Level.Crane))
        {
            craneDanseAnimator = new CraneDanceAnimator(levelToModel[currentLevel]);
        }
    }

    public void Dance(DanceMove danseMove)
    {
        if (currentLevel.Equals(Level.Crane) && craneDanseAnimator != null)
        {
            craneDanseAnimator.Dance(danseMove);
        }
    }

    void OnLevelChange(Level level)
    {
        ChangeModel(level);
    }
}
