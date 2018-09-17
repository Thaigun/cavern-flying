using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RequireInputFieldsButton : MonoBehaviour
{
    [SerializeField]
    private List<TMP_InputField> _requiredInputFields;

    protected Button ButtonComponent
    {
        get
        {
            return GetComponent<Button>();
        }
    }

    // Update is called once per frame
    protected virtual void Update ()
    {
        SetInteractable();
	}

    /// <summary>
    /// Checks and sets the button interactable if prerequisites are fulfilled.
    /// </summary>
    void SetInteractable()
    {
        bool emptyFieldsFound = false;
        foreach (var input in _requiredInputFields)
        {
            emptyFieldsFound = emptyFieldsFound || string.IsNullOrEmpty(input.text);
        }
        ButtonComponent.interactable = !emptyFieldsFound;
    }
}
