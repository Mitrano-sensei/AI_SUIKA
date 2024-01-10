using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Fruit : MonoBehaviour
{
    private Collider2D _collider;
    [SerializeField] private FruitType _fruitType;
    public Transform LimitYPosition;

    private GameManager _gameManager;

    public void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    public void Start()
    {
        _gameManager = GameManager.Instance;

        _gameManager.OnLoose.AddListener(() =>
        {
            GetComponent<Rigidbody2D>().simulated = false;
            _collider.enabled = false;
        });
    }

    public void Update()
    {
        CheckLoose();
    }

    private float _looseTime = 0f;

    /**
     * Check if the fruit is too high and loose if it is
     */
    private void CheckLoose()
    {
        if (_gameManager.HasLost) return;

        bool isTooHigh = transform.position.y > LimitYPosition.position.y;
        if (isTooHigh)
        {
            _looseTime += Time.deltaTime;
            if (_looseTime > 1f)
            {
                _gameManager.Loose();
            }
        }
        else
        {
            _looseTime = 0f;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        var otherFruit = collision.collider.GetComponent<Fruit>();
        if (otherFruit == null) return;

        if (otherFruit.GetFruitType() == GetFruitType())
        {
            Debug.Log("Merging !");
            Merge(otherFruit);
        }
    }

    private bool _merging = false;
    private Action<Fruit> _destroyAction;

    public bool Merging { get => _merging; set => _merging = value; }

    /**
     * Merge the fruit with another fruit
     */
    private void Merge(Fruit otherFruit)
    {
        if (_merging || otherFruit.Merging)
        {
            Debug.LogError("Fruit is already merging");
            return;
        }
        Merging = true;
        otherFruit.Merging = true;

        otherFruit.CallDestroyAction();

        var averagePosition = (transform.position + otherFruit.transform.position) / 2f;
        _gameManager.SpawnMergedFruitFrom(averagePosition, GetFruitType());

        CallDestroyAction();
    }

    /**
     * Register a destroy action
     */
    public void RegisterDestroyAction(Action<Fruit> action)
    {
        _destroyAction = action;
    }

    /**
     * Calls destroy action
     */
    public void CallDestroyAction()
    {
        _destroyAction.Invoke(this);
    }

    /**
     * Get the fruit type
     */
    public FruitType GetFruitType()
    {
        return _fruitType;
    }

    public enum FruitType
    {
        Cherry = 0,
        Strawberry = 1,
        Grapes = 2,
        Dekopon = 3,
        Persimmon = 4,
        Apple = 5,
        Pear = 6,
        Peach = 7,
        Pineapple = 8,
        Honeydew = 9,
        Watermelon = 10
    }
}
