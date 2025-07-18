using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class RoomItem : MonoBehaviour
{
    [SerializeField] RoomInfo _roomInfo;
    [SerializeField] TMP_Text roomInfoText;
    [SerializeField] PhotonManager photonManager;

    public RoomInfo RoomInfo
    {
        get { return _roomInfo; }
        set
        {
            _roomInfo = value;
            roomInfoText.text = $"{_roomInfo.Name} ({_roomInfo.PlayerCount} / {_roomInfo.MaxPlayers})";
            // 동적 이벤트 리스너 구현해서 눌렀을때 해당 룸에 접속 
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnEnterRoom(_roomInfo.Name));
        }
    }

    private void Awake()
    {
        roomInfoText = GetComponentInChildren<TMP_Text>();
        photonManager = GameObject.Find("PhotonManager").GetComponent<PhotonManager>();
    }

    void OnEnterRoom(string roomName)
    {

        photonManager.SetUserId();
        // 룸 옵션 
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;
        //PhotonNetwork.IsMessageQueueRunning = false;
        // 룸 접속 
         PhotonNetwork.JoinOrCreateRoom(roomName, ro,TypedLobby.Default,null);
        //PhotonNetwork.JoinRoom(roomName);
    }
}
