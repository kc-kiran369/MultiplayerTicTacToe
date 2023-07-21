using System.Collections.Generic;
using UnityEngine;

public struct ClickedButton
{
    public int Index;
    public char Value;
}

public class BoardManager : MonoBehaviour
{
    public static BoardManager Board;
    public GridButton[] buttons;
    [field: SerializeField] public int CurrentPlayer { get; set; } = 0;
    [field: SerializeField] public List<int> ClickedButtons;
    [SerializeField] private UnityEngine.UI.Button PlayAgainButton;
    [SerializeField] public TMPro.TMP_Text PlayersTurnText;

    private void Awake()
    {
        if (Board == null) { Board = this; }

        PlayAgainButton.onClick.AddListener(delegate { PlayAgain(); });
    }

    private void Start()
    {
        buttons = GetComponentsInChildren<GridButton>();

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].ButtonIndex = i;
        }
    }

    public void OnButtonClicked(GridButton button, int PlayerID, int buttonIndex)
    {
        if (PlayerID != CurrentPlayer || Player.LocalPlayer.GetTotalPlayers() != 2) return;

        if (string.IsNullOrEmpty(button.text.text))
        {
            button.SetValue(GetFillShape());
            ClickedButtons.Add(buttonIndex);
            UpdateUI(buttonIndex, GetFillShape());
            Check();
            ChangePlayersTurn();
        }
    }

    private void ChangePlayersTurn()
    {
        if (CurrentPlayer == 1)
        {
            CurrentPlayer = 0;
            Player.LocalPlayer.RPC_SendInt(CurrentPlayer);
        }
        else if (CurrentPlayer == 0)
        {
            CurrentPlayer = 1;
            Player.LocalPlayer.RPC_SendInt(CurrentPlayer);
        }
    }

    public string GetFillShape()
    {
        if (CurrentPlayer == 1) return "X";
        else return "O";
    }

    private void Check()
    {
        var shape = GetFillShape();

        //Check horizontally
        if (buttons[0].Value == shape && buttons[1].Value == shape && buttons[2].Value == shape)
            GameOver();
        else if (buttons[3].Value == shape && buttons[4].Value == shape && buttons[5].Value == shape)
            GameOver();
        else if (buttons[6].Value == shape && buttons[7].Value == shape && buttons[8].Value == shape)
            GameOver();


        //Check Vertically
        else if (buttons[0].Value == shape && buttons[3].Value == shape && buttons[6].Value == shape)
            GameOver();
        else if (buttons[1].Value == shape && buttons[4].Value == shape && buttons[7].Value == shape)
            GameOver();
        else if (buttons[2].Value == shape && buttons[5].Value == shape && buttons[8].Value == shape)
            GameOver();


        //Check Diagonally
        else if (buttons[0].Value == shape && buttons[4].Value == shape && buttons[8].Value == shape)
            GameOver();
        else if (buttons[2].Value == shape && buttons[4].Value == shape && buttons[6].Value == shape)
            GameOver();


        if (ClickedButtons.Count == 9)
        {
            Player.LocalPlayer.RPC_OpenMessageBox($"Game Over\nDraw");
        }
    }

    private void GameOver()
    {
        Player.LocalPlayer.RPC_OpenMessageBox("Game Over\nWinner : " + GetFillShape());
    }

    public void UpdateUI(int ButtonIndex, string value)
    {
        Player.LocalPlayer.RPC_SetBoardUI(ButtonIndex, value);
        Player.LocalPlayer.RPC_SetUI();
    }

    public void PlayAgain()
    {
        Player.LocalPlayer.RPC_PlayAgain();
    }

    public void Reset()
    {
        CurrentPlayer = 0;
        ClickedButtons.Clear();

        foreach (var button in buttons)
        {
            button.Reset();
        }
    }

    public void ConfigureBoardOnRejoin(int index, char shape)
    {
        buttons[index].SetValue(shape.ToString());
        ClickedButtons.Add(index);
    }
}