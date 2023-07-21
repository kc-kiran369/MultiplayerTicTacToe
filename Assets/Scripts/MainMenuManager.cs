using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Button ServerButton;
    [SerializeField] Button HostButton;
    [SerializeField] Button JoinButton;

    [SerializeField] Spawner Spawner;

    private void Start()
    {
        ServerButton.onClick.AddListener(OnServerBtnClicked);
        HostButton.onClick.AddListener(OnHostBtnClicked);
        JoinButton.onClick.AddListener(OnJoinBtnClicked);
    }

    private void OnServerBtnClicked() { Spawner.StartGame(Fusion.GameMode.Server); }
    private void OnHostBtnClicked() { Spawner.StartGame(Fusion.GameMode.Host); }
    private void OnJoinBtnClicked() { Spawner.StartGame(Fusion.GameMode.Client); }
}