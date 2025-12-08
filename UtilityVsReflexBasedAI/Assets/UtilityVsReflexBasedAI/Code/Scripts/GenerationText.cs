using TMPro;
using UnityEngine;

public class GenerationText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private void Awake()
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
