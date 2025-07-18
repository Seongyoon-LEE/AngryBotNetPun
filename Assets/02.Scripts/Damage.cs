using Photon.Pun;
using Photon.Realtime;
using Player = Photon.Realtime.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 피격시 발사한 네트워크 유저의 확인 및 메세지 출력 로직 
public class Damage : MonoBehaviourPunCallbacks
{
    [SerializeField] Renderer[] renderers;
    [SerializeField] int initHp = 100;
    [SerializeField] int curHp = 0;
    Animator animator;
    CharacterController cc;
    WaitForSeconds wsPlayerDie;
    GameManager gameManager;

    [Header("Hash 관련")]
    readonly int hashDie = Animator.StringToHash("Die");
    readonly int hashRespawn = Animator.StringToHash("Respawn");
    readonly string bulletTag = "BULLET";
    void Start()
    {
        curHp = initHp;
        renderers = GetComponentsInChildren<Renderer>();
        animator = GetComponent<Animator>();
        cc = gameObject.GetComponent<CharacterController>();
        wsPlayerDie = new WaitForSeconds(Random.Range(3, 7));
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void OnCollisionEnter(Collision c)
    {
        if (curHp > 0 && c.collider.CompareTag(bulletTag))
        {
            curHp -= 20;
            curHp = Mathf.Clamp(curHp, 0, initHp);
            if (curHp <= 0)
            {
                if (photonView.IsMine)
                {   // 맞은 총알의 ActorNumber 추출 
                    var actorNo = c.collider.GetComponent<Bullet>().actorNumber;
                    // ActorNumber로 현재 입장한 플레이어 추출 
                    Player lastShooterPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNo);
                    // 메세지 출력을 위한 문자열 포맷 
                    string msg = string.Format($"\n<color=#0f0>{photonView.Owner.NickName}</color> is Killed by<color=#f00> {lastShooterPlayer.NickName} </color>");

                    photonView.RPC("KillMessage", RpcTarget.AllBufferedViaServer, msg);
                }

                StartCoroutine(PlayerDie());
            }
        }
    }
    [PunRPC]
    void KillMessage(string msg)
    {
        gameManager.msgList.text += msg;
    }
        IEnumerator PlayerDie()
        {
            cc.enabled = false;
            animator.SetBool(hashRespawn, false);
            animator.SetTrigger(hashDie);
           
            yield return wsPlayerDie;
            SetVisible(false); // 메시렌더러 비활성화
            // 부활 했을때
            SetVisible(true); // 메시렌더러 비활성화
            animator.SetBool(hashRespawn, true);
            
            
            yield return new WaitForSeconds(1.5f);
            Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
            int idx = Random.Range(1, points.Length);
            transform.position = points[idx].position;
            curHp = initHp;
            cc.enabled = true;
        }
    
    void SetVisible(bool visible)
    {
        foreach(Renderer renderer in renderers) { renderer.enabled = visible; }
    }
}
