using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LayoutDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;

    private void Start()
    {
        PopulateDropdown();
        _dropdown.onValueChanged.AddListener(OnLayoutChanged);
    }

    private void PopulateDropdown()
    {
        var options = new List<string>();

        foreach (var layout in GridConfig.SupportedLayouts)
        {
            options.Add(layout.Label);
        }

        _dropdown.ClearOptions();
        _dropdown.AddOptions(options);
    }

    private void OnLayoutChanged(int index)
    {
        var selected = GridConfig.SupportedLayouts[index];
        BoardManager.Instance.ResetBoard(selected);
    }
}