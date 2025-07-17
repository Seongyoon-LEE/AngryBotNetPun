using System.Collections;
using System.Collections.Generic;
using Photon.Pun; //����Ƽ ���� ��Ʈ��ũ ���̺귯��
using Photon.Realtime; //���� ��Ʈ��ũ ���̺귯�� 
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    //������ ����
    private readonly string version = "1.0";
    //������ �г��� 
    private string userId = "Zack";
    void Awake()
    {
        //������ Ŭ���̾�Ʈ�� �� �ڵ� ����ȭ �ɼ�
        PhotonNetwork.AutomaticallySyncScene = true;
        // ������ ���ο� ���� �ε� ���� �� �ش� �뿡 ������
        //�ٸ� ���� �����鿡�Ե� �ڵ����� �ش� ���� �ε����ִ� ����̴�.
        //PhotonNetwork.LoadLevel("");
        PhotonNetwork.GameVersion = version;
        PhotonNetwork.NickName = userId;

        // ���漭������ �������� �ʴ� ���� Ƚ�� 
        Debug.Log(PhotonNetwork.SendRate);
        //���� ���� ����  �� �� �������� ����
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster() //������Ŭ���̾�Ʈ ������ ����
    {
        Debug.Log($"Connected to Master");
        print($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();

    }
    public override void OnJoinedLobby() //�κ� ���� �� ȣ�� �Ǵ� �ݹ� �Լ� 
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinRandomRoom(); //������ �������� ���� 
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {                   //���� �� ������ ���� ���� �� ȣ�� �Ǵ� �ݹ� �Լ� 
        //base.OnJoinRandomFailed(returnCode, message);
        print($"JoinRandom Failed {returnCode} \n {message}");
        //���� ��ȣ      ������ �� ������ ����
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
