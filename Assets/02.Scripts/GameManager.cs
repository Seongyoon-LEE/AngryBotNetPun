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
        SetRoomInfo(); // 접속 정보를 추출 
        exitButton.onClick.AddListener(() => OnExitClick()); // 버튼의 온클릭 이벤트 리스너
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
    void SetRoomInfo() // 룸 접속 정보를 출력 
    {
        Room room = PhotonNetwork.CurrentRoom;
        roomName.text = room.Name;
        connectInfo.text = $"({room.PlayerCount} / {room.MaxPlayers})";
    }
    void OnExitClick()
    {
        PhotonNetwork.LeaveRoom(); // 룸을 나가면 
    }
    public override void OnLeftRoom() // 자동 호출
    {
        SceneManager.LoadScene(0); // 로비씬으로 
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {   // 네트워크 유저가 입장 했다면
        SetRoomInfo();
        string msg = $"\n<color=#0f0>{newPlayer}</color> is joined room";
        msgList.text += msg;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {   // 네트워크 유저가 퇴장 했다면
        SetRoomInfo();
        string msg = $"\n<color=#f00>{otherPlayer}</color> is left room";
        msgList.text += msg;
    }
}
