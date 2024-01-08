using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Fruit : MonoBehaviour
{
    private Collider2D _collider;
    [SerializeField] private FruitType _fruitType;

    public void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }
    public void Start()
    {
        
    }

    public void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        var otherFruit = collision.collider.GetComponent<Fruit>();
        if (otherFruit == null) return;

        if (otherFruit.GetFruitType() == GetFruitType())
        {
            Merge(otherFruit);
        }
    }

    private bool _merging = false;
    public bool Merging { get => _merging; set => _merging = value; }

    private void Merge(Fruit otherFruit)
    {
        if (_merging || otherFruit.Merging) return;
        Merging = true;
        otherFruit.Merging = true;

        otherFruit.gameObject.SetActive(false);
        Destroy(otherFruit.gameObject);

        gameObject.SetActive(false);
        GameManager.Instance.SpawnFruit(gameObject.transform, GetFruitType());

        Destroy(gameObject);
    }

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
