using System;
using TMPro;
using UnityEngine;

public class SimulationTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;

    private static float _elapsedTime;

    [SerializeField] private float _timeLimitSeconds = 30f;
    private static bool _limitReached = false;

    public static event Action OnTimeLimitReached;

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(_elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(_elapsedTime % 60f);
        _timerText.text = $"Time: {minutes:D2}:{seconds:D2}";

        if (!_limitReached && _elapsedTime >= _timeLimitSeconds)
        {
            _limitReached = true;
            OnTimeLimitReached?.Invoke();
        }
    }

    public static void ResetTimer()
    {
        _elapsedTime = 0f;
        _limitReached = false;
    }
}

