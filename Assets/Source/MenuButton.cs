using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] string defaultText = "button";
    [SerializeField] string highlightedText = "> button <";
    [SerializeField] int defaultFontSize = 100;
    [SerializeField] int highlightedFontSize = 120;
    [SerializeField] Color defaultTextColor = Color.gray;
    [SerializeField] Color highlightedTextColor = Color.white;

    private TextMeshProUGUI text;
    private Button button;
    private bool isMouseOver = false;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void Update()
    {
        if (isMouseOver)
        {
            text.text = highlightedText;
            text.fontSize = highlightedFontSize;
            text.color = highlightedTextColor;
        }
        else
        {
            text.text = defaultText;
            text.fontSize = defaultFontSize;
            text.color = defaultTextColor;
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        isMouseOver = true;
        AudioManager.Instance.PlaySound("ButtonHover");
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        isMouseOver = false;
    }

    private void OnButtonClick()
    {
        isMouseOver = false;

        text.text = defaultText;
        text.fontSize = defaultFontSize;
        text.color = defaultTextColor;

        AudioManager.Instance.PlaySound("ButtonClick");
    }
}
