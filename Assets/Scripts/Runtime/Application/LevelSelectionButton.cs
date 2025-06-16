using Application.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionButton : SimpleButton
{
    [SerializeField] private int _level = 0;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private GameObject _lockBG;
    [SerializeField] private Image _goldenStar;


    public event Action<int> OnLevelSelected;

    private void OnValidate()
    {
        _level = transform.parent.GetSiblingIndex();
        _levelText.text = (_level + 1).ToString();
    }

    public void Initialize(bool locked, bool finished, Sprite sprite)
    {
        _lockBG.SetActive(locked);
        _goldenStar.enabled = finished;

        _image.sprite = sprite;
        if (!locked)
        {
            Button.onClick.AddListener(() =>
            {
                PlayPressAnimation();
                OnLevelSelected?.Invoke(_level);
            });
        }
    }
}