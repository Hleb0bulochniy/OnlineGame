using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI LogText;

    private bool playRequested;

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
        if (LogText != null)
            LogText.text += "\n" + message;
    }

    public void Play()
    {
        playRequested = true;
        Log("Play pressed");

        if (PhotonNetwork.IsConnectedAndReady)
        {
            TryJoinRandom();
        }
        else
        {
            Log("Not connected yet");
            if (!PhotonNetwork.IsConnected)
                PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void TryJoinRandom()
    {
        Log("Trying to join random room");
        PhotonNetwork.JoinRandomRoom();
    }

    private void TryCreateRoom()
    {
        Log("No rooms available. Creating a new room");
        var options = new RoomOptions
        {
            MaxPlayers = 10
        };

        PhotonNetwork.CreateRoom(null, options);
    }

    public override void OnConnectedToMaster()
    {
        Log("Connected to Master");

        if (playRequested)
            TryJoinRandom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Log($"JoinRandom failed ({returnCode}): {message}");
        TryCreateRoom();
    }

    public override void OnCreatedRoom()
    {
        Log("Room created");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Log($"CreateRoom failed ({returnCode}): {message}");
        TryJoinRandom();
    }

    public override void OnJoinedRoom()
    {
        Log("Joined the room. Loading Game scene...");
        PhotonNetwork.LoadLevel("Game");
    }
}
