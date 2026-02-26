using UnityEngine;
using Unity.Netcode;
using TMPro;

public class NetworkLauncher : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] TMP_Text statusText;

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        statusText.text = "HOST started";
        panel.SetActive(false);
    }

    public void Client()
    {
        NetworkManager.Singleton.StartClient();
        statusText.text = "CLIENT connecting...";
        panel.SetActive(false);
    }

    public void Server()
    {
        NetworkManager.Singleton.StartServer();
        statusText.text = "SERVER started";
        panel.SetActive(false);
    }

    void Update()
    {
        if (!NetworkManager.Singleton) return;

        if (NetworkManager.Singleton.IsHost)
            statusText.text = "Running as HOST";

        else if (NetworkManager.Singleton.IsClient)
            statusText.text = "Running as CLIENT";

        else if (NetworkManager.Singleton.IsServer)
            statusText.text = "Running as SERVER";
    }
}