using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using static Fruit;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _fruits;
    [SerializeField] private CloudScript _cloud;
    [SerializeField] private Transform _lowerPos;
    [SerializeField] private Transform _upperPos;
    [SerializeField] private Transform _fruitsParent;

    [SerializeField] private Transform[] _corners;

    // CurrentFruit, NextFruit
    public UnityEvent<FruitType, FruitType> OnRollFruit = new();
    public UnityEvent<FruitType> OnMerge = new();
    public UnityEvent OnLoose = new();

    private FruitType _currentFruit;
    private FruitType _nextFruit;

    private bool _hasLost = false;
    [SerializeField] private ScoreManager _scoreManager;

    private Dictionary<FruitType, ObjectPool<Fruit>> _fruitsPool;

    public bool HasLost { get => _hasLost; private set => _hasLost = value; }

    protected void Awake()
    {
        // Init Object Pool
        _fruitsPool = new();

        foreach (var fruit in _fruits)
        {
            var fruitComponent = fruit.GetComponent<Fruit>();
            if (fruitComponent == null) Debug.LogError("Fruit component is null on " + fruit.name);

            _fruitsPool.Add(fruitComponent.GetFruitType(), 
                new ObjectPool<Fruit>(
                    () => {
                            var fruit = Instantiate(fruitComponent, _fruitsParent);
                            Helper.DelayedAction(0.1f, () => fruit.Merging = false);
                            fruit.LimitYPosition = _lowerPos;
                            fruit.RegisterDestroyAction(fruit => _fruitsPool[fruit.GetFruitType()].Release(fruit));
                            fruit.GameManager = this;
                            return fruit;
                        },
                    fruit => { 
                        fruit.gameObject.SetActive(true);
                        fruit.transform.rotation = Quaternion.identity;
                        fruit.Merging = false;
                    },
                    fruit => { fruit.gameObject.SetActive(false); },
                    fruit => Destroy(fruit.gameObject),
                    false,
                    10)
                );
        }
    }

    public void Start()
    {
        _currentFruit = (FruitType)Random.Range(0, 3);
        _nextFruit = (FruitType)Random.Range(0, 3);

        RollFruits();

        // Logs the longest chain after every player action
        OnRollFruit.AddListener((fruit, fruit2) => Debug.Log("Longest Chain : " + FindLongestChain()));
        OnRollFruit.AddListener((fruit, fruit2) => Debug.Log("Biggest Fruit Distance To Corner : " + FindBiggestFruitDistanceToCorner()));
    }

    public void Update()
    {
        if (_hasLost) return;

        var localMousePos = _fruitsParent.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        var mouseXPosition = Mathf.Clamp(localMousePos.x, _lowerPos.position.x, _upperPos.position.x);
        _cloud.transform.position = new Vector3( mouseXPosition,
                                                _cloud.transform.position.y,
                                                _cloud.transform.position.z);
        // var percent = (mouseXPosition - _lowerPos.position.x) / (_upperPos.position.x - _lowerPos.position.x);
    }

    /**
     * At the given position, spawns the fruit that would be spawned if a fruit of type fruitType is merged.
     * Note : Watermelon destroy themselves
     */
    public void SpawnMergedFruitFrom(Vector3 position, FruitType fruitType)
    {
        if (_hasLost) return;
        if (fruitType == FruitType.Watermelon) { return; } // Watermelon destroy themselves

        var fruit = InstantiateFruit(fruitType+1).GetComponent<Fruit>();
        fruit.LimitYPosition = _lowerPos;
        fruit.transform.position = position;

        EarnPoints(fruitType+1);
        OnMerge.Invoke(fruitType+1);
    }

    private void EarnPoints(FruitType fruitType)
    {
        switch (fruitType)
        {
            case FruitType.Cherry:
                Debug.LogError("Cherry should not be merged");
                break;
            case FruitType.Strawberry:
                _scoreManager.AddScore(3);
                break;
            case FruitType.Grapes:
                _scoreManager.AddScore(6);
                break;
            case FruitType.Dekopon:
                _scoreManager.AddScore(10);
                break;
            case FruitType.Persimmon:
                _scoreManager.AddScore(15);
                break;
            case FruitType.Apple:
                _scoreManager.AddScore(21);
                break;
            case FruitType.Pear:
                _scoreManager.AddScore(28);
                break;
            case FruitType.Peach:   
                _scoreManager.AddScore(36);
                break;
            case FruitType.Pineapple:
                _scoreManager.AddScore(45);
                break;
            case FruitType.Honeydew:
                _scoreManager.AddScore(55);
                break;  
            case FruitType.Watermelon:
                _scoreManager.AddScore(66);
                break;
            default:
                Debug.LogError("Unknown fruit type");
                break;

        }
    }

    /**
     * At the given position, spawns the fruit that would be spawned if a fruit of type fruitType is merged.
     * Note : Watermelon destroy themselves
     */
    public void SpawnMergedFruitFrom(Transform position, FruitType fruitType)
    {
        SpawnMergedFruitFrom(position.position, fruitType);
    }

    /**
     * Rolls current and next fruits
     */
    public void RollFruits()
    {
        if (_hasLost) return;

        _currentFruit = _nextFruit;
        _nextFruit = (FruitType)Random.Range(0, 3);

        OnRollFruit.Invoke(_currentFruit, _nextFruit);
    }

    /**
     * Returns a new instantiated fruit of the current fruit type
     */
    internal Fruit InstantiateCurrentFruit()
    {
        return InstantiateFruit(_currentFruit);
    }

    /**
     * Returns a new instantiated fruit of the given fruit type using Object pooling
     */
    private Fruit InstantiateFruit(FruitType fruitType)
    {
        Fruit myFruit = _fruitsPool[fruitType].Get();
        return myFruit;
    }

    /**
     * Called when the player loose
     */
    internal void Loose()
    {
        Debug.Log("Lost");
        _hasLost = true;

        OnLoose.Invoke();
    }

    public int FindLongestChain()
    {
        var fruits = FindObjectsOfType<Fruit>();
        var longestChain = 1;

        // Select fruits that are not held by cloud
        var fruitsNotHeld = fruits.Where(fruit => fruit != _cloud.MyFruit);

        // Sort from biggest to smallest fruit type
        var sortedFruits = fruitsNotHeld.OrderByDescending(fruit => fruit.GetFruitType());

        foreach (var fruit in sortedFruits)
        {
            if (fruit.GetFruitType() == FruitType.Cherry) break;
            if ((int)fruit.GetFruitType() <= longestChain) continue;
            var chain = fruit.FindChain();
            if (chain > longestChain) longestChain = chain;
        }

        return longestChain;
    }

    public float FindBiggestFruitDistanceToCorner()
    {
        var fruits = FindObjectsOfType<Fruit>();
        var fruitsNotHeld = fruits.Where(fruit => fruit != _cloud.MyFruit);
        var sortedFruits = fruitsNotHeld.OrderByDescending(fruit => fruit.GetFruitType());

        // Select biggest fruit and every fruit that are the same type as the biggest fruit
        var biggestFruit = sortedFruits.First();
        var biggestFruitType = biggestFruit.GetFruitType();
        var biggestFruits = sortedFruits.Where(fruit => fruit.GetFruitType() == biggestFruitType);
        
        var lowestDistance = 10000f;

        foreach (var fruit in biggestFruits)
        {
            lowestDistance = Mathf.Min(lowestDistance, fruit.FindDistanceToCorner(_corners[0], _corners[1]));
        }

        return lowestDistance;
    }
}
