using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        if(button) button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if(button.interactable) SoundManager.Instance.PlaySound(SoundManager.Instance.clickSound);
    }
}
