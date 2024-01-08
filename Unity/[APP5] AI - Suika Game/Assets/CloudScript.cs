using UnityEngine;

public class CloudScript : MonoBehaviour
{
    [SerializeField] private Transform _fruitPosition;

    private GameObject _myFruit;
    private GameManager _gameManager;

    void Start()
    {
        _gameManager = GameManager.Instance;
        
        _gameManager.OnRollFruit.AddListener((fruit, fruit2) => {
            SetPlaceHolderFruit();
        });

        _gameManager.OnSpawnFruit.AddListener(position =>{
            ReleasePlaceHolderFruit(position);
        });

        if (_myFruit != null) EnableMyFruit(false);
    }

    private void EnableMyFruit(bool enable)
    {
        if (_myFruit == null) {
            Debug.LogError("MyFruit is null on CloudScript"); 
            return;
        }
        _myFruit.GetComponent<Rigidbody2D>().simulated = enable;
        _myFruit.GetComponent<Collider2D>().enabled = enable;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _gameManager.SpawnFruit(_fruitPosition.position);
        }

        _myFruit.transform.position = _fruitPosition.position;
    }

    public void SetPlaceHolderFruit()
    {
        _myFruit = _gameManager.InstantiateCurrentFruit();
        EnableMyFruit(false);
    }

    public void ReleasePlaceHolderFruit(Vector3 position)
    {
        var currentPos = transform.position;
        transform.position = new Vector3(position.x, currentPos.y, currentPos.z);

        EnableMyFruit(true);
    }
}
