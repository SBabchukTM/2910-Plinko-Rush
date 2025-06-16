using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecordDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _placeText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    public void Initialize(int place, string name, int score)
    {
        SetPlace(place);
        _nameText.text = name;
        _scoreText.text = score.ToString();
    }

    private void SetPlace(int place)
    {
        _placeText.text = place.ToString();
    }
}
