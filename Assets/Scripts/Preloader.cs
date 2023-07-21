using UnityEngine;

public class Preloader : MonoBehaviour
{
    [Header("Arguments")]
    public string ClientArgument = "-start-as-client";
    public string HostArgument = "-start-as-host";
    public string ServerArgument = "-start-as-server";

    void Start()
    {
        var args = System.Environment.GetCommandLineArgs();

        foreach (var arg in args)
        {
            if (arg == ServerArgument)
            {
                GetComponent<Spawner>().StartGame(Fusion.GameMode.Server);
            }
            else if (arg == HostArgument)
            {
                GetComponent<Spawner>().StartGame(Fusion.GameMode.Host);
            }
            else if (arg == ClientArgument)
            {
                GetComponent<Spawner>().StartGame(Fusion.GameMode.Client);
            }
        }
    }
}
