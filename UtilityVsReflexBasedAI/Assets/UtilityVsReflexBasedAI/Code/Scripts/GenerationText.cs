using AweDev.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenerationText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        PopulationManager.OnNewGenerationCreated += UpdateGenerationText;
    }

    private void OnDestroy()
    {
        PopulationManager.OnNewGenerationCreated -= UpdateGenerationText;
    }

    private void UpdateGenerationText(int generationID)
    {
        _text.SetText($"Gen: {generationID}");
    }
}

