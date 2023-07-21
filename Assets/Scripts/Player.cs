using Fusion;
using System.Linq;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public static Player LocalPlayer { get; private set; }
    public ChatHistory ChatHistory { get; private set; }
    [field: SerializeField][Networked] public bool IsMyTurn { get; set; } = false;
    [field: SerializeField][Networked] public int PlayerID { get; set; }

    private void Awake()
    {
        ChatHistory = ChatHistory.Instance;
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            LocalPlayer = this;
            ChatHistory = ChatHistory.Instance;
            LocalPlayer.RPC_SetUI();
        }
    }

    public int GetTotalPlayers() => Runner.ActivePlayers.Count();

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SendMessage(string message, RpcInfo info = default)
    {
        ChatSystem.Instance.AppendMessage(message);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All, InvokeLocal = false)]
    public void RPC_SendInt(int currentPlayer, RpcInfo info = default)
    {
        BoardManager.Board.CurrentPlayer = currentPlayer;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All, InvokeLocal = false)]
    public void RPC_SetBoardUI(int buttonIndex, string value, RpcInfo info = default)
    {
        BoardManager.Board.ClickedButtons.Add(buttonIndex);
        BoardManager.Board.buttons[buttonIndex].SetValue(value);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SetUI(RpcInfo info = default)
    {
        if (Runner.IsServer) return;
        BoardManager.Board.PlayersTurnText.text = (LocalPlayer.PlayerID != BoardManager.Board.CurrentPlayer ? "Your Turn [" + (BoardManager.Board.GetFillShape() == "O" ? "X" + "]" : "O" + "]") : "Other's Player Turn");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_OpenMessageBox(string text, float time = 0, RpcInfo info = default)
    {
        MessageBox.Instance.Show(text, time);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_PlayAgain(RpcInfo info = default)
    {
        BoardManager.Board.Reset();
        MessageBox.Instance.Hide();
        BoardManager.Board.PlayersTurnText.text = "";
    }
}