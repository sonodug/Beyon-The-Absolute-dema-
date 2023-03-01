using System;
using TMPro;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private ButtonInteractiveZone _buttonInteractiveZone;
    [SerializeField] private CallElevatorButtonZone _callElevatorButtonZone;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private UIImage _uiImage;
    [SerializeField] private Player _player;

    private bool _isMenuOpened;

    private void OnEnable()
    {
        _player.MenuOpened += OnMenuInteractive;
        _buttonInteractiveZone.OpenedText += OnOpenText;
        _buttonInteractiveZone.ClosedText += OnCloseText;
        _callElevatorButtonZone.OpenedText += OnOpenText;
        _callElevatorButtonZone.ClosedText += OnCloseText;
    }
    
    private void OnDisable()
    {
        _player.MenuOpened -= OnMenuInteractive;
        _buttonInteractiveZone.OpenedText -= OnOpenText;
        _buttonInteractiveZone.ClosedText -= OnCloseText;
        _callElevatorButtonZone.OpenedText -= OnOpenText;
        _callElevatorButtonZone.ClosedText -= OnCloseText;
    }

    private void OnMenuInteractive()
    {
        if (!_isMenuOpened)
        {
            _uiImage.gameObject.SetActive(true);
            _isMenuOpened = !_isMenuOpened;
        }
        else
        {
            _uiImage.gameObject.SetActive(false);
            _isMenuOpened = !_isMenuOpened;
        }
    }

    private void OnOpenText()
    {
        _text.gameObject.SetActive(true);
    }
    
    private void OnCloseText()
    {
        _text.gameObject.SetActive(false);
    }
}
