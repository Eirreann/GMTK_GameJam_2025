using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(ScrollRect))]
public class ScrollRectAutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scrollSpeed = 10f;
    private bool mouseOver = false;

    private List<Selectable> m_Selectables = new List<Selectable>();
    private ScrollRect m_ScrollRect;
    
    private InputSystem_Actions m_Actions;

    private Vector2 m_NextScrollPosition = Vector2.up;
    void OnEnable()
    {
        if (m_ScrollRect)
        {
            m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
        }
    }
    void Awake()
    {
        m_ScrollRect = GetComponent<ScrollRect>();
    }
    void Start()
    {
        m_Actions = new InputSystem_Actions();
        if (m_ScrollRect)
        {
            m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
        }
        ScrollToSelected(true);
    }
    void Update()
    {
        // Scroll via input.
        InputScroll();
        
        if (!mouseOver)
        {
            // Lerp scrolling code.
            m_ScrollRect.normalizedPosition = Vector2.Lerp(m_ScrollRect.normalizedPosition, m_NextScrollPosition, scrollSpeed * Time.deltaTime);
        }
        else
        {
            m_NextScrollPosition = m_ScrollRect.normalizedPosition;
        }
    }
    void InputScroll()
    {
        var navigate = m_Actions.UI.Navigate.ReadValue<Vector2>();
        if (m_Selectables.Count > 0)
        {
            ScrollToSelected(true);
        }
    }
    void ScrollToSelected(bool quickScroll)
    {
        int selectedIndex = -1;
        Selectable selectedElement = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;

        if (selectedElement)
        {
            selectedIndex = m_Selectables.IndexOf(selectedElement);
        }
        if (selectedIndex > -1)
        {
            if (quickScroll)
            {
                m_ScrollRect.normalizedPosition = new Vector2(0, 1 - (selectedIndex / ((float)m_Selectables.Count - 1)));
                m_NextScrollPosition = m_ScrollRect.normalizedPosition;
            }
            else
            {
                m_NextScrollPosition = new Vector2(0, 1 - (selectedIndex / ((float)m_Selectables.Count - 1)));
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        ScrollToSelected(false);
    }
}