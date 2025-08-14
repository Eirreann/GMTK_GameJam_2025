using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UITabHandler : MonoBehaviour
{
    public List<Button> buttons;
    public List<GameObject> tabs;

    public Sprite upSprite;
    public Sprite downSprite;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttons[0].interactable = false;
        tabs[0].gameObject.SetActive(true);
        
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

    UnityAction ClickTabButton(Button button)
    {
        return () =>
        {
            foreach(Button btn in buttons)
            {
                btn.interactable = btn != button;
            }

            var count = 0;
            foreach(GameObject tabs in tabs)
            {
                tabs.SetActive(count == buttons.IndexOf(button));
                count++;
            }
        };
    }
}
