using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviourPunCallbacks
{
    public Volume volume;
    public TMP_Text roomName;
    public TMP_Text connectInfo;
    public Button exitButton;
    public TMP_Text msgList;

    IEnumerator Start()
    {

        //PhotonNetwork.IsMessageQueueRunning = true;
        while(!PhotonNetwork.InRoom)
        {
            yield return null;
        }
        CreatePlayer();
        SetRoomInfo(); // ���� ������ ���� 
        exitButton.onClick.AddListener(() => OnExitClick()); // ��ư�� ��Ŭ�� �̺�Ʈ ������
    }
    private void CreatePlayer()
    {
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);
        PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0, null);

        DepthOfField dof;
        volume.profile.TryGet<DepthOfField>(out dof);
        dof.active = false;
    }
    void SetRoomInfo() // �� ���� ������ ��� 
    {
        Room room = PhotonNetwork.CurrentRoom;
        roomName.text = room.Name;
        connectInfo.text = $"({room.PlayerCount} / {room.MaxPlayers})";
    }
    void OnExitClick()
    {
        PhotonNetwork.LeaveRoom(); // ���� ������ 
    }
    public override void OnLeftRoom() // �ڵ� ȣ��
    {
        SceneManager.LoadScene(0); // �κ������ 
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {   // ��Ʈ��ũ ������ ���� �ߴٸ�
        SetRoomInfo();
        string msg = $"\n<color=#0f0>{newPlayer}</color> is joined room";
        msgList.text += msg;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {   // ��Ʈ��ũ ������ ���� �ߴٸ�
        SetRoomInfo();
        string msg = $"\n<color=#f00>{otherPlayer}</color> is left room";
        msgList.text += msg;
    }
}
