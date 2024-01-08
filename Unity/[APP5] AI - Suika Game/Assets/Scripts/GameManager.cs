using UnityEngine;
using static Fruit;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject[] _fruits;
    [SerializeField] private Transform _gameEnvironment;

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition = _gameEnvironment.InverseTransformPoint(mousePosition);
            mousePosition.z = 0;

            SpawnFruit(mousePosition, FruitType.Peach);
        }
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

}
