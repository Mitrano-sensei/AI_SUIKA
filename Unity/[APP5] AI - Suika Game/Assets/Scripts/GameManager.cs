using System;
using UnityEngine;
using UnityEngine.Events;
using static Fruit;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject[] _fruits;
    [SerializeField] private CloudScript _cloud;
    [SerializeField] private Transform _lowerPos;
    [SerializeField] private Transform _upperPos;
    [SerializeField] private Transform _gameEnvironment;

    // CurrentFruit, NextFruit
    public UnityEvent<FruitType, FruitType> OnRollFruit = new();
    public UnityEvent<Vector3> OnSpawnFruit = new();

    private FruitType _currentFruit;
    private FruitType _nextFruit;

    public void Start()
    {
        _currentFruit = (FruitType)UnityEngine.Random.Range(0, 3);
        _nextFruit = (FruitType)UnityEngine.Random.Range(0, 3);

        OnRollFruit.Invoke(_currentFruit, _nextFruit);
    }

    public void Update()
    {
        var localMousePos = _gameEnvironment.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        var mouseXPosition = Mathf.Clamp(localMousePos.x, _lowerPos.position.x, _upperPos.position.x);
        _cloud.transform.position = new Vector3( mouseXPosition,
                                                _cloud.transform.position.y,
                                                _cloud.transform.position.z);
        var percent = (mouseXPosition - _lowerPos.position.x) / (_upperPos.position.x - _lowerPos.position.x);
    }

    public void SpawnFruit(Transform position, FruitType fruitType)
    {
        if (fruitType == FruitType.Watermelon) { return; } // Watermelon destroy themselves

        int fruitIndex = (int)(fruitType + 1);
        var fruit = Instantiate(_fruits[fruitIndex], _gameEnvironment);
        fruit.transform.position = position.position;
    }

    public void SpawnFruit(Vector3 position, FruitType fruitType)
    {
        if (fruitType == FruitType.Watermelon) { return; } // Watermelon destroy themselves

        int fruitIndex = (int)(fruitType + 1);
        var fruit = Instantiate(_fruits[fruitIndex], _gameEnvironment);
        fruit.transform.position = position;
    }

    internal void SpawnFruit(Vector3 position)
    {
        OnSpawnFruit.Invoke(position);
        RollFruits();
    }

    private void RollFruits()
    {
        _currentFruit = _nextFruit;
        _nextFruit = (FruitType)UnityEngine.Random.Range(0, 3);

        OnRollFruit.Invoke(_currentFruit, _nextFruit);
    }

    internal GameObject InstantiateCurrentFruit()
    {
        GameObject myFruit = Instantiate(_fruits[(int)_currentFruit], _gameEnvironment);
        return myFruit;
    }
}
