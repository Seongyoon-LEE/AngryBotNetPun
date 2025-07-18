using System.Collections;
using System.Collections.Generic;
using Photon.Pun; //����Ƽ ���� ��Ʈ��ũ ���̺귯��
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime; //���� ��Ʈ��ũ ���̺귯�� 
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    //������ ����
    private readonly string version = "1.0";
    //������ �г��� 
    private string userId = "Zack";

    [SerializeField] TMP_InputField userIF;
    [SerializeField] TMP_InputField roomNameIF;
    //���Ͽ� ���� �����͸� ���� �ϱ� ���� ��ųʸ� �ڷ��� 
    private Dictionary<string,GameObject> rooms = new Dictionary<string,GameObject>();
    //������ ǥ���� ������ 
    [SerializeField] GameObject roomItemPrefab;
    [SerializeField] Transform scrollContents;
    void Awake()
    {
        roomItemPrefab = Resources.Load<GameObject>("Image-RoomItem");

       if(PhotonNetwork.IsConnected) return;

        //������ Ŭ���̾�Ʈ�� �� �ڵ� ����ȭ �ɼ�
        PhotonNetwork.AutomaticallySyncScene = true;
        // ������ ���ο� ���� �ε� ���� �� �ش� �뿡 ������
        //�ٸ� ���� �����鿡�Ե� �ڵ����� �ش� ���� �ε����ִ� ����̴�.
        //PhotonNetwork.LoadLevel("");
        PhotonNetwork.GameVersion = version;
        PhotonNetwork.NickName = userId;

        // ���漭������ �������� �ʴ� ���� Ƚ�� 
        Debug.Log(PhotonNetwork.SendRate);
        // ���� ���� ����  �� �� �������� ����
        PhotonNetwork.ConnectUsingSettings();

        
            
        if(PhotonNetwork.IsConnected == false) // ���� ���� ����
        {   // ���� ���� ������ �������ٸ�~
            // ���� ���� �����Ҷ� �������� ����
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    private void Start()
    {
        // ����� �������� �ε� 
        userId = PlayerPrefs.GetString("USER_ID",$"USER_{Random.Range(1,22)}");
        userIF.text = userId;
        
        if(PhotonNetwork.IsConnected) return ;

        // ���� ������ �г��� ���
        PhotonNetwork.NickName =userId;
        
    }
    public void SetUserId() // �������� �����ϴ� ���� 
    {
        if(string.IsNullOrEmpty(userIF.text))
        {
            userId = $"USER_{Random.Range(1, 21):00}";
        }
        else
        {
            userId = userIF.text;
        }
        // ������ ����
        PlayerPrefs.SetString("USER_ID",userId);
        // ���� ������ �г��� ���
        PhotonNetwork.NickName=userId;
    }
    string SetRoomName() // ����� �Է� ���θ� Ȯ�� �ϴ� ���� 
    {
        if(string.IsNullOrEmpty(roomNameIF.text))
        {
            roomNameIF.text = $"ROOM_ {Random.Range(1, 101): 000}";
        }
        return roomNameIF.text;
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
        //PhotonNetwork.JoinRandomRoom(); //������ �������� ���� 
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {                   //���� �� ������ ���� ���� �� ȣ�� �Ǵ� �ݹ� �Լ� 
        //base.OnJoinRandomFailed(returnCode, message);
        print($"JoinRandom Failed {returnCode} \n {message}");
        //���� ��ȣ      ������ �� ������ ����

        OnMakeRoomClick();
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
        // ������ Ŭ���̾�Ʈ�� ��쿡 �뿡 ������ �� ���ξ��� �ε��Ѵ�. 
        if (PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.IsMessageQueueRunning = false;
            PhotonNetwork.LoadLevel("AngryBotScene");
            // ������ Ŭ���̾�Ʈ�� ������ ������ ������ Ŭ���̾�Ʈ���� �ڵ� ���� �ǰ� ���ش�
        }
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {   // ���� �����ǰų� ���� �����ɶ� �ڵ� ȣ�� �Ǵ� �ݹ��Լ� 
        // ���� �� ���� ��ȭ�� �߻� �Ҷ����� �ݹ��Լ��� ȣ��ȴ�.
        // ������ �뿡 ���� ������ �Ѿ�´�. �� ���� ���δ� RemovedFromList �Ӽ����� Ȯ�� �� �� �ִ�.

        // ������ RoomItem �������� ������ �ӽ� ���� 
        GameObject tempRoom = null;
        foreach(var roomInfo in roomList)
        {
            if(roomInfo.RemovedFromList) // �����ϴ� ���� 
            {
                // ��ųʸ����� �� �̸����� �˻��� ����� RoomItem ������ ����
                rooms.TryGetValue(roomInfo.Name, out tempRoom);

                // RoomItem ������ ����
                Destroy(tempRoom);
                // ��ųʸ����� �ش� ���̸��� �����ͻ��� 
                rooms.Remove(roomInfo.Name);
            }
            else // �� ������ ����� ��� // �����ϴ� ���� 
            {   // �� �̸��� ��ųʸ��� ���� ��� ���� ���� 
                if(rooms.ContainsKey(roomInfo.Name) == false) // �񱳸� �ؼ� �ִ��� ������ �˻� 
                {
                    // RoomItem �������� scrollContents ������ ���� 
                    GameObject roomPrefabs = Instantiate(roomItemPrefab, scrollContents);
                    // �� ������ ǥ���ϱ� ���� RoomInfo ���� ���� 
                    roomPrefabs.GetComponent<RoomItem>().RoomInfo = roomInfo;
                    // ��ųʸ� �ڷ����� ������ �߰� 
                    rooms.Add(roomInfo.Name,roomPrefabs);
                }
                else // �� �̸��� ��ųʸ��� �ִ� ��쿡 �� ������ ���� 
                {
                    rooms.TryGetValue (roomInfo.Name, out tempRoom);
                    tempRoom.GetComponent<RoomItem>().RoomInfo = roomInfo;
                }
            }    
        }
    }
    #region UI_BUTTON_EVENT
    public void OnLoginClick()
    {
        SetUserId();

        PhotonNetwork.JoinRandomRoom();
        //�������� ������ ������ ���� 
    }
    public void OnMakeRoomClick()
    {
        SetUserId(); // ������ ����  
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;
        //PhotonNetwork.IsMessageQueueRunning = false;
        // �� �̵��� ���� ���� ��Ʈ��ũ �޼��� ���� ����
        PhotonNetwork.CreateRoom(SetRoomName(), ro, TypedLobby.Default);
    }
    #endregion
    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.SendRate.ToString());
                        // ���� �������� ������ �ʴ� ���� Ƚ�� 
    }

    //private void CreatePlayer()
    //{
    //    Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
    //    int idx = Random.Range(1, points.Length);
    //    PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0, null);
    //}
}
