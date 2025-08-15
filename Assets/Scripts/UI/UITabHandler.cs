using System.Collections.Generic;
using Game;
using Input;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UITabHandler : MonoBehaviour
{
    public List<Button> buttons;
    public List<GameObject> tabs;

    public Sprite upSprite;
    public Sprite downSprite;

    [SerializeField] private GameObject _backButton;
    [SerializeField] private GameObject _nextButton;

    [SerializeField] private ControlsChangedHelper helper;
    [SerializeField] private UISelectionHelper _selectionHelper;
    
    [SerializeField] private int _currentTab;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentTab = 0;
        
        buttons[_currentTab].interactable = false;
        tabs[_currentTab].gameObject.SetActive(true);

        SetNextBackState();
        
        helper.OnControlsChanged += SetNextBackState;
        
        var buttonCount = 1;
        foreach (var button in buttons)
        {
            button.onClick.AddListener(ClickTabButton(button));

            if (buttonCount % 2 != 0)
            {
                button.GetComponent<Image>().sprite = upSprite;
            }
            else
            {
                button.GetComponent<Image>().sprite = downSprite;
            }
            
            buttonCount++;
        }

        var tabCount = 0;
        foreach (var tab in tabs)
        {
            tab.SetActive(tabCount == 0);
            tabCount++;
        }
    }

    public void Update()
    {
        if (_selectionHelper._inputSystem.UI.Previous.WasPressedThisFrame())
        {
            if (_currentTab != 0)
            {
                buttons[_currentTab - 1].onClick.Invoke();
                _currentTab--;
            }
            
        }
        
        if (_selectionHelper._inputSystem.UI.Next.WasPressedThisFrame())
        {
            if (_currentTab < buttons.Count - 1)
            {
                buttons[_currentTab + 1].onClick.Invoke();
                _currentTab++;
            }
        }
    }
    
    public void SetNextBackState()
    {
        _backButton.SetActive(_selectionHelper.playerInput.currentControlScheme == "Gamepad");
        _nextButton.SetActive(_selectionHelper.playerInput.currentControlScheme == "Gamepad");
    }

    UnityAction ClickTabButton(Button button)
    {
        return () =>
        {
            _currentTab = buttons.IndexOf(button);
            foreach(Button btn in buttons)
            {
                btn.interactable = btn != button;
            }

            var count = 0;
            foreach(GameObject tabs in tabs)
            {
                tabs.SetActive(count == _currentTab);
                count++;
            }
        };
    }
}
