using Fusion;
using Newtonsoft.Json;
using System.Collections.Generic;

public class ChatHistory : NetworkBehaviour
{
    public static ChatHistory Instance;

    public List<string> ChatHistoryList;

    private void Awake()
    {
        ChatHistoryList = new List<string>();

        if (Instance == null) Instance = this;
    }

    public void AddMessage(string message) { ChatHistoryList.Add(message); }

    public string GetMessageAsJson() => JsonConvert.SerializeObject(ChatHistoryList);

    public void PopulateDataAfterRejoin(string msg, string clickedButtonsJSON, int PlayerTurn)
    {
        RPC_SendMessageJSON(msg, clickedButtonsJSON, PlayerTurn);
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_SendMessageJSON(string msg, string clickedButtonsJSON, int PlayerTurn)
    {
        ChatHistoryList.Clear();
        foreach (var chat in JsonConvert.DeserializeObject<List<string>>(msg))
        {
            ChatSystem.Instance.AppendMessage(chat);
        }
        var clickedButtons = JsonConvert.DeserializeObject<ClickedButton[]>(clickedButtonsJSON);
        foreach (var buttonIndex in clickedButtons)
        {
            BoardManager.Board.ConfigureBoardOnRejoin(buttonIndex.Index, buttonIndex.Value);
        }
        BoardManager.Board.CurrentPlayer = PlayerTurn;
    }
}