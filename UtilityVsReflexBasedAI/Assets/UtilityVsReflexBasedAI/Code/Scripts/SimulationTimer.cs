using AweDev.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimulationTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;

    private float _elapsedTime;

    private void Update()
    {
        _elapsedTime += Time.deltaTime * AdjustGameSpeed.gameSpeed;
        int minutes = Mathf.FloorToInt(_elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(_elapsedTime % 60f);
        _timerText.text = $"T: {minutes:D2}:{seconds:D2}";
    }
}

