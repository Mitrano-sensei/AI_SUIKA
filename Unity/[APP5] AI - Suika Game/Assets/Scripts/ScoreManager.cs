using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : Singleton<ScoreManager>
{
    private int _score = 0;
    public UnityEvent<int> OnScoreChanged = new();
    [SerializeField] private TextMeshProUGUI _scoreText;

    public void Start()
    {
        _score = 0;
        OnScoreChanged.AddListener((score) => _scoreText.text = score.ToString());

        OnScoreChanged.Invoke(0);
    }

    /**
     * Add score to the current score
     */
    public void AddScore(int score)
    {
        _score += score;
        OnScoreChanged.Invoke(_score);
    }

}
