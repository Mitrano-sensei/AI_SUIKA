using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Fruit;

public class FruitPlaceHolder : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] _sprites;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null ) Debug.LogError("SpriteRenderer is null on PlaceHolder");

        GameManager.Instance.OnRollFruit.AddListener((fruit, fruit2) =>
        {
            SetSprite(fruit2);
        });
    }

    public void SetSprite(FruitType fruitType)
    {
        _spriteRenderer.sprite = _sprites[(int)fruitType];
    }
}
