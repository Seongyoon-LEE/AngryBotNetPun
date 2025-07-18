using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;

public class Fire : MonoBehaviourPun
{
    [SerializeField] Transform firePos;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] PhotonView pv = null;
    PlayerInput input;
    void Start()
    {
        pv = GetComponent<PhotonView>();
        input = GetComponent<PlayerInput>();
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if(input.isMouseClick && pv.IsMine) // 마우스 클릭이 있고, 내가 조종하는 플레이어인 경우
        {
            FireBullet(pv.Owner.ActorNumber);
            pv.RPC("FireBullet", RpcTarget.Others, pv.Owner.ActorNumber);
            //RPCTarget.AllVIaSorver, RpcTarget.AllBufferedViaServer
            //위의 RpcTarget 방식 중 위의 방식은 RPC호출시 동시성이 필요한 경우
            //포톤 클라우드 서버에서 접속해 있는 모든 네트워크 유저에게 동시에
            //RPC를 전송한다. 하지만 통신망 속도에 따라 천차 만별이라 근사치에 해당한다곡 생각하면 된다.
        }
    }
    [PunRPC]
    void FireBullet(int actorNumber)
    {
        if (!muzzleFlash.isPlaying) muzzleFlash.Play(); // 총구 화염 효과 재생

        GameObject bullet = Instantiate(bulletPrefab, firePos.position, firePos.rotation);
        bullet.GetComponent<Bullet>().actorNumber = actorNumber;
    }
}
