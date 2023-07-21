using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatSystem : NetworkBehaviour
{
    public static ChatSystem Instance;

    [SerializeField] Button SendButton;
    [SerializeField] TMP_InputField ChatInputField;
    [SerializeField] TMP_Text ChatDisplay;

    ChatHistory _chatHistory;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        _chatHistory = ChatHistory.Instance;
        SendButton.onClick.AddListener(delegate { UploadMessage(); });
    }

    void UploadMessage()
    {
        var message = GetMessage();
        if (string.IsNullOrEmpty(message)) return;
        Player.LocalPlayer.RPC_SendMessage(message);
        ChatInputField.text = "";
    }

    public void AppendMessage(string message)
    {
        _chatHistory.AddMessage(message);
        UpdateUI();
    }

    public void UpdateUI()
    {
        ChatDisplay.text = "";
        foreach (var item in _chatHistory.ChatHistoryList)
        {
            ChatDisplay.text += item + "\n";
        }
    }
    string GetMessage() => ChatInputField.text;
}