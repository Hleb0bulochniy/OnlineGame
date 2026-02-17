using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI LogText;



    void Start()
    {
        PhotonNetwork.NickName = "Player" + Random.Range(1000, 9999);
        Log("Player's name is set to " + PhotonNetwork.NickName);

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Log(string message)
    {
        Debug.Log(message);
        LogText.text += "\n";
        LogText.text += message;
    }

    public override void OnConnectedToMaster()
    {
        Log("Connected to master");
        base.OnConnectedToMaster();
    }

    public override void OnJoinedRoom()
    {
        Log("Joined the room");

        PhotonNetwork.LoadLevel("Game");
        base.OnJoinedRoom();
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 10 });
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
}
