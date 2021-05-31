using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetManager : MonoBehaviourPunCallbacks
{
    int maxPlayer = 4;
    
    void Start()
    {
        PhotonNetwork.GameVersion = "0.1";
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnected()
    {
        print("OnConnected");
    }

    public override void OnConnectedToMaster()
    {
        print("OnConnectedToMaster");
        PhotonNetwork.NickName = "Player" + Random.Range(0, 1000);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        print("OnJoinedLobby");
        PhotonNetwork.JoinOrCreateRoom("Tesla", new RoomOptions() { MaxPlayers = (byte)maxPlayer }, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        print("OnCreatedRoom");
    }

    public override void OnJoinedRoom()
    {
        print("OnJoinedRoom");
        PhotonNetwork.Instantiate("Player", new Vector3(0, 1.2f, 0), Quaternion.identity);
    }

}
