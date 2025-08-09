using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "TooltipSO", menuName = "Scriptable Objects/TooltipSO")]
public class TooltipSO : ScriptableObject
{
    public String tooltipText = "";
    public List<string> bindingsList;
}
