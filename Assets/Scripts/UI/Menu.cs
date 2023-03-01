using System;
using TMPro;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private ButtonInteractiveZone _buttonInteractiveZone;
    [SerializeField] private GeneratorInteractiveZone _generatorInteractiveZone;
    [SerializeField] private CallElevatorButtonZone _callElevatorButtonZone;
    [SerializeField] private TMP_Text _textE;
    [SerializeField] private TMP_Text _textZ;
    [SerializeField] private UIImage _uiImage;
    [SerializeField] private Player _player;

    private bool _isMenuOpened;

    private void OnEnable()
    {
        _player.MenuOpened += OnMenuInteractive;
        _buttonInteractiveZone.OpenedText += OnOpenTextE;
        _buttonInteractiveZone.ClosedText += OnCloseTextE;
        
        _callElevatorButtonZone.OpenedText += OnOpenTextE;
        _callElevatorButtonZone.ClosedText += OnCloseTextE;
        
        _generatorInteractiveZone.OpenedText += OnOpenTextE;
        _generatorInteractiveZone.ClosedText += OnCloseTextE;

        _player.OpenedText += OnOpenTextZ;
    }
    
    private void OnDisable()
    {
        _player.MenuOpened -= OnMenuInteractive;
        
        _buttonInteractiveZone.OpenedText -= OnOpenTextE;
        _buttonInteractiveZone.ClosedText -= OnCloseTextE;
        
        _callElevatorButtonZone.OpenedText -= OnOpenTextE;
        _callElevatorButtonZone.ClosedText -= OnCloseTextE;
        
        _generatorInteractiveZone.OpenedText -= OnOpenTextE;
        _generatorInteractiveZone.ClosedText -= OnCloseTextE;
        
        _player.OpenedText += OnOpenTextZ;
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

    private void OnOpenTextE()
    {
        _textE.gameObject.SetActive(true);
    }
    
    private void OnOpenTextZ()
    {
        _textZ.gameObject.SetActive(true);
    }
    
    private void OnCloseTextE()
    {
        _textE.gameObject.SetActive(false);
        _textZ.gameObject.SetActive(false);
    }
}
