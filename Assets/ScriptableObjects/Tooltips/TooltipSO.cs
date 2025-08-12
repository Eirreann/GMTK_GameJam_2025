using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "TooltipSO", menuName = "Scriptable Objects/TooltipSO")]



public class TooltipSO : ScriptableObject
{
    public enum Binding
    {
        Move,
        Look,
        Jump,
        Crouch,
        Attack,
        Interact,
        Restart
    }
    
    public String tooltipText = "";
    public List<Binding> bindingsList;
    private List<InputAction> _bindingsLookup = new List<InputAction>();

    public void InitializeList(List<InputAction> bindingsList)
    {
        _bindingsLookup = bindingsList;
    }

    public List<InputAction> GetBindingList()
    {
        return _bindingsLookup;
    }
    
    public InputAction GetBinding(Binding binding)
    {
        return _bindingsLookup[(int)binding];
    }
}
