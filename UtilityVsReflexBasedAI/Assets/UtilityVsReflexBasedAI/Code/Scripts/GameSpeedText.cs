using AweDev.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSpeedText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        AdjustGameSpeed.OnGameSpeedChanged += UpdateGameSpeedText;
    }

    private void UpdateGameSpeedText(float gameSpeed)
    {
        _text.text = $"x{gameSpeed:F2}";
    }
}

