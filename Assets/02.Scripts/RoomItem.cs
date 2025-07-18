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
            // ���� �̺�Ʈ ������ �����ؼ� �������� �ش� �뿡 ���� 
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
        // �� �ɼ� 
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;
        //PhotonNetwork.IsMessageQueueRunning = false;
        // �� ���� 
         PhotonNetwork.JoinOrCreateRoom(roomName, ro,TypedLobby.Default,null);
        //PhotonNetwork.JoinRoom(roomName);
    }
}
