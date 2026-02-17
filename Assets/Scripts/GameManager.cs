using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{

    public GameObject PlayerPrefab;
    void Start()
    {
        if (!PhotonNetwork.InRoom) return;
        SpawnPlayer();
    }

    void Update()
    {
        
    }

    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("Player {0} entered room", newPlayer.NickName);
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player {0} left room", otherPlayer.NickName);
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public override void OnJoinedRoom()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (!PhotonNetwork.InRoom) return;

        PhotonNetwork.Instantiate(PlayerPrefab.name,
            new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f)),
            Quaternion.identity);
    }
}
