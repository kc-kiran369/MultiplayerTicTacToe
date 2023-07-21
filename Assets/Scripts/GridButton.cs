using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridButton : MonoBehaviour
{
    public string Value { get; private set; }
    public bool HasClicked { get; private set; } = false;
    public TMP_Text text;
    public Button button;
    public int ButtonIndex { get; set; }

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        text = GetComponentInChildren<TMP_Text>();
    }

    private void OnClick()
    {
        BoardManager.Board.OnButtonClicked(this, Player.LocalPlayer.PlayerID, ButtonIndex);
    }

    public void SetValue(string value)
    {
        Value = value;
        HasClicked = true;
        text.text = value;
    }

    public void Reset()
    {
        Value = "";
        HasClicked = false;
        text.text = Value;
    }
}