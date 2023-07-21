using Fusion;
using Fusion.Sockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable UNT0006
#pragma warning disable IDE0090

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public ChatHistory ChatHistory;
    public Dictionary<PlayerRef, NetworkObject> SpawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();
    public Dictionary<PlayerRef, Tuple<NetworkObject, int>> LeftPlayers = new Dictionary<PlayerRef, Tuple<NetworkObject, int>>();

    [SerializeField] private NetworkPrefabRef PlayerPrefab;

    private NetworkRunner _networkRunner;

    public async void StartGame(GameMode mode)
    {
        _networkRunner = gameObject.AddComponent<NetworkRunner>();
        _networkRunner.ProvideInput = true;

        var task = await _networkRunner.StartGame(new StartGameArgs
        {
            GameMode = mode,
            SessionName = "",
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 2
        });

        print(task.ErrorMessage);

        if (mode == GameMode.Server) _networkRunner.SetActiveScene(1);
    }

    public async Task LoadScene(int index = 1)
    {
        if (SceneManager.GetActiveScene().buildIndex == index) return;
        _networkRunner.SetActiveScene(index);
        await Task.Delay(2000);
    }

    public async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (Player.LocalPlayer)
            ChatHistory = ChatHistory.Instance;

        if (_networkRunner.IsServer)
        {
            await LoadScene(1);

            if (LeftPlayers.TryGetValue(player, out Tuple<NetworkObject, int> netObject))
            {
                Debug.Log($"Player Rejoin {player}");

                Invoke(nameof(SyncDataWithRejoinPlayers), 3.0f);

                netObject.Item1.gameObject.GetComponent<Player>().PlayerID = netObject.Item2;
                SpawnedPlayers.Add(player, netObject.Item1);
                LeftPlayers.Remove(player);
            }
            else
            {
                Debug.Log($"Player join {player}");

                NetworkObject netObj = _networkRunner.Spawn(PlayerPrefab, Vector3.zero, Quaternion.identity, player);

                netObj.gameObject.GetComponent<Player>().PlayerID = SpawnedPlayers.Count;
                runner.SetPlayerObject(player, netObj);

                SpawnedPlayers.Add(player, netObj);
            }
        }

        PlayerPrefs.SetString("SessionName", runner.SessionInfo.Name);
    }

    private void SyncDataWithRejoinPlayers()
    {
        var clickedButtonsRegistry = BoardManager.Board.ClickedButtons;

        ClickedButton[] clickedButtons = new ClickedButton[clickedButtonsRegistry.Count];

        var buttonsRegistry = BoardManager.Board.buttons;

        for (int i = 0; i < clickedButtonsRegistry.Count; i++)
        {
            clickedButtons[i].Index = clickedButtonsRegistry.ElementAt(i);
            clickedButtons[i].Value = char.Parse(buttonsRegistry[clickedButtonsRegistry.ElementAt(i)].Value);
        }

        ChatHistory.Instance.PopulateDataAfterRejoin(ChatHistory.Instance.GetMessageAsJson(), JsonConvert.SerializeObject(clickedButtons), BoardManager.Board.CurrentPlayer);
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_networkRunner.IsServer)
        {
            Debug.Log($"Player Left {player}");

            if (SpawnedPlayers.TryGetValue(player, out NetworkObject netObj))
            {
                LeftPlayers.Add(player, new Tuple<NetworkObject, int>(netObj, netObj.gameObject.GetComponent<Player>().PlayerID));
                SpawnedPlayers.Remove(player);
            }
        }
    }

    #region UNUSED_INTERFACE_OVERRIDES
    public void OnConnectedToServer(NetworkRunner runner)
    {
        print("Connectiong To Server");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        print("Connection Failed");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        print("Connection Request : " + request);
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        print("Disconnected From Server");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    #endregion
}