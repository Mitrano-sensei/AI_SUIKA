using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    private int _score = 0;
    public UnityEvent<int, int> OnScoreChanged = new();
    [SerializeField] private TextMeshProUGUI _scoreText;

    public void Start()
    {
        _score = 0;
        OnScoreChanged.AddListener((score, addedScore) => _scoreText.text = score.ToString());

        OnScoreChanged.Invoke(0, 0);
    }

    /**
     * Add score to the current score
     */
    public void AddScore(int score)
    {
        _score += score;
        OnScoreChanged.Invoke(_score, score);
    }

    public void ResetScore()
    {
        _score = 0;
        OnScoreChanged.Invoke(0, 0);
    }

}
