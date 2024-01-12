using UnityEngine;

public class CloudScript : MonoBehaviour
{
    [SerializeField] private Transform _fruitPosition;

    private Fruit _myFruit;
    [SerializeField] private GameManager _gameManager;

    public Fruit MyFruit { get => _myFruit; private set => _myFruit = value; }

    void Start()
    {        
        _gameManager.OnRollFruit.AddListener((fruit, fruit2) => {
            SetPlaceHolderFruit();
        });

        if (_myFruit != null) EnableMyFruit(false);
    }

    [SerializeField] private float _timeBetweenFruits = 0.5f;
    private float _lastTimePlayed = 0f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - _lastTimePlayed < _timeBetweenFruits) return;
            _lastTimePlayed = Time.time;

            ReleasePlaceHolderFruit(_fruitPosition.position);
            _gameManager.RollFruits();
        }

        _myFruit.transform.position = _fruitPosition.position;
    }

    /**
     * Enable or disable the fruit
     */
    private void EnableMyFruit(bool enable)
    {
        if (_myFruit == null)
        {
            Debug.LogError("MyFruit is null on CloudScript");
            return;
        }
        _myFruit.GetComponent<Rigidbody2D>().simulated = enable;
        _myFruit.GetComponent<Collider2D>().enabled = enable;
    }

    /*
     * Set the placeholder fruit (in the hand of the cloud)
     */
    public void SetPlaceHolderFruit()
    {
        _myFruit = _gameManager.InstantiateCurrentFruit();
        EnableMyFruit(false);
    }

    /*
     * Release the placeholder fruit so it falls
     */
    public void ReleasePlaceHolderFruit(Vector3 position)
    {
        var currentPos = transform.position;
        transform.position = new Vector3(position.x, currentPos.y, currentPos.z);

        EnableMyFruit(true);
    }
}
