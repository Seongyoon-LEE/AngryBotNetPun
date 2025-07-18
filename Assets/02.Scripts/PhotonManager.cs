using System.Collections;
using System.Collections.Generic;
using Photon.Pun; //유니티 전용 네트워크 라이브러리
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime; //범용 네트워크 라이브러리 
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    //게임의 버전
    private readonly string version = "1.0";
    //유저의 닉네임 
    private string userId = "Zack";

    [SerializeField] TMP_InputField userIF;
    [SerializeField] TMP_InputField roomNameIF;
    //룸목록에 대한 데이터를 저장 하기 위한 딕셔너리 자료형 
    private Dictionary<string,GameObject> rooms = new Dictionary<string,GameObject>();
    //룸목록을 표시할 프리팹 
    [SerializeField] GameObject roomItemPrefab;
    [SerializeField] Transform scrollContents;
    void Awake()
    {
        roomItemPrefab = Resources.Load<GameObject>("Image-RoomItem");

       if(PhotonNetwork.IsConnected) return;

        //마스터 클라이언트의 씬 자동 동기화 옵션
        PhotonNetwork.AutomaticallySyncScene = true;
        // 방장이 새로운 씬을 로딩 했을 때 해당 룸에 입장한
        //다른 접속 유저들에게도 자동으로 해당 씬을 로딩해주는 기능이다.
        //PhotonNetwork.LoadLevel("");
        PhotonNetwork.GameVersion = version;
        PhotonNetwork.NickName = userId;

        // 포톤서버와의 데이터의 초당 전송 횟수 
        Debug.Log(PhotonNetwork.SendRate);
        // 포톤 서버 접속  할 때 버전별로 접속
        PhotonNetwork.ConnectUsingSettings();

        
            
        if(PhotonNetwork.IsConnected == false) // 포톤 서버 접속
        {   // 포톤 서버 연결이 끊어졌다면~
            // 포톤 서버 접속할때 버전별로 접속
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    private void Start()
    {
        // 저장된 유저명을 로드 
        userId = PlayerPrefs.GetString("USER_ID",$"USER_{Random.Range(1,22)}");
        userIF.text = userId;
        
        if(PhotonNetwork.IsConnected) return ;

        // 접속 유저의 닉네임 등록
        PhotonNetwork.NickName =userId;
        
    }
    public void SetUserId() // 유저명을 설정하는 로직 
    {
        if(string.IsNullOrEmpty(userIF.text))
        {
            userId = $"USER_{Random.Range(1, 21):00}";
        }
        else
        {
            userId = userIF.text;
        }
        // 유저명 저장
        PlayerPrefs.SetString("USER_ID",userId);
        // 접속 유저의 닉네임 등록
        PhotonNetwork.NickName=userId;
    }
    string SetRoomName() // 룸명의 입력 여부를 확인 하는 로직 
    {
        if(string.IsNullOrEmpty(roomNameIF.text))
        {
            roomNameIF.text = $"ROOM_ {Random.Range(1, 101): 000}";
        }
        return roomNameIF.text;
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
        //PhotonNetwork.JoinRandomRoom(); //무작위 룸접속을 실행 
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {                   //랜덤 룸 입장이 실패 했을 때 호출 되는 콜백 함수 
        //base.OnJoinRandomFailed(returnCode, message);
        print($"JoinRandom Failed {returnCode} \n {message}");
        //오류 번호      오류가 난 이유를 설명

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
        // 마스터 클라이언트인 경우에 룸에 입장한 후 메인씬을 로딩한다. 
        if (PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.IsMessageQueueRunning = false;
            PhotonNetwork.LoadLevel("AngryBotScene");
            // 마스터 클라이언트가 접속한 씬으로 나머지 클라이언트들이 자동 접속 되게 해준다
        }
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {   // 룸이 삭제되거나 새로 생성될때 자동 호출 되는 콜백함수 
        // 룸목록 룸 정보 변화가 발생 할때마다 콜백함수가 호출된다.
        // 삭제된 룸에 대한 정보도 넘어온다. 룸 삭제 여부는 RemovedFromList 속성으로 확인 할 수 있다.

        // 삭제된 RoomItem 프리팹을 저장할 임시 변수 
        GameObject tempRoom = null;
        foreach(var roomInfo in roomList)
        {
            if(roomInfo.RemovedFromList) // 삭제하는 로직 
            {
                // 딕셔너리에서 룸 이름으로 검색해 저장된 RoomItem 프리팹 추출
                rooms.TryGetValue(roomInfo.Name, out tempRoom);

                // RoomItem 프리팹 삭제
                Destroy(tempRoom);
                // 딕셔너리에서 해당 룸이름의 데이터삭제 
                rooms.Remove(roomInfo.Name);
            }
            else // 룸 정보가 변경된 경우 // 생성하는 로직 
            {   // 룸 이름이 딕셔너리에 없는 경우 새로 생성 
                if(rooms.ContainsKey(roomInfo.Name) == false) // 비교를 해서 있는지 없는지 검사 
                {
                    // RoomItem 프리팹을 scrollContents 하위에 생성 
                    GameObject roomPrefabs = Instantiate(roomItemPrefab, scrollContents);
                    // 룸 정보를 표시하기 위해 RoomInfo 정보 전달 
                    roomPrefabs.GetComponent<RoomItem>().RoomInfo = roomInfo;
                    // 딕셔너리 자료형에 데이터 추가 
                    rooms.Add(roomInfo.Name,roomPrefabs);
                }
                else // 룸 이름이 딕셔너리에 있는 경우에 룸 정보를 갱신 
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
        //무작위로 추출한 룸으로 입장 
    }
    public void OnMakeRoomClick()
    {
        SetUserId(); // 유저명 저장  
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;
        //PhotonNetwork.IsMessageQueueRunning = false;
        // 씬 이동시 포톤 서버 네트워크 메세지 수신 금지
        PhotonNetwork.CreateRoom(SetRoomName(), ro, TypedLobby.Default);
    }
    #endregion
    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.SendRate.ToString());
                        // 포톤 서버와의 데이터 초당 전송 횟수 
    }

    //private void CreatePlayer()
    //{
    //    Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
    //    int idx = Random.Range(1, points.Length);
    //    PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0, null);
    //}
}
