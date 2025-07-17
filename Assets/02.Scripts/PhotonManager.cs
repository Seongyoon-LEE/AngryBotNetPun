using System.Collections;
using System.Collections.Generic;
using Photon.Pun; //유니티 전용 네트워크 라이브러리
using Photon.Realtime; //범용 네트워크 라이브러리 
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    //게임의 버전
    private readonly string version = "1.0";
    //유저의 닉네임 
    private string userId = "Zack";
    void Awake()
    {
        //마스터 클라이언트의 씬 자동 동기화 옵션
        PhotonNetwork.AutomaticallySyncScene = true;
        // 방장이 새로운 씬을 로딩 했을 때 해당 룸에 입장한
        //다른 접속 유저들에게도 자동으로 해당 씬을 로딩해주는 기능이다.
        //PhotonNetwork.LoadLevel("");
        PhotonNetwork.GameVersion = version;
        PhotonNetwork.NickName = userId;

        // 포톤서버와의 데이터의 초당 전송 횟수 
        Debug.Log(PhotonNetwork.SendRate);
        //포톤 서버 접속  할 때 버전별로 접속
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster() //마스터클라이언트 공간에 접속
    {
        Debug.Log($"Connected to Master");
        print($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();

    }
    public override void OnJoinedLobby() //로비에 접속 후 호출 되는 콜백 함수 
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinRandomRoom(); //무작위 룸접속을 실행 
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {                   //랜덤 룸 입장이 실패 했을 때 호출 되는 콜백 함수 
        //base.OnJoinRandomFailed(returnCode, message);
        print($"JoinRandom Failed {returnCode} \n {message}");
        //오류 번호      오류가 난 이유를 설명
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;

        PhotonNetwork.CreateRoom("MyRoom", ro, TypedLobby.Default);
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("Create Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name} ");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log($" PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($" Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName},{player.Value.ActorNumber}");
        }
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);
        PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0, null);
    }

}
