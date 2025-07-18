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
        if(input.isMouseClick && pv.IsMine) // ���콺 Ŭ���� �ְ�, ���� �����ϴ� �÷��̾��� ���
        {
            FireBullet(pv.Owner.ActorNumber);
            pv.RPC("FireBullet", RpcTarget.Others, pv.Owner.ActorNumber);
            //RPCTarget.AllVIaSorver, RpcTarget.AllBufferedViaServer
            //���� RpcTarget ��� �� ���� ����� RPCȣ��� ���ü��� �ʿ��� ���
            //���� Ŭ���� �������� ������ �ִ� ��� ��Ʈ��ũ �������� ���ÿ�
            //RPC�� �����Ѵ�. ������ ��Ÿ� �ӵ��� ���� õ�� �����̶� �ٻ�ġ�� �ش��Ѵٰ� �����ϸ� �ȴ�.
        }
    }
    [PunRPC]
    void FireBullet(int actorNumber)
    {
        if (!muzzleFlash.isPlaying) muzzleFlash.Play(); // �ѱ� ȭ�� ȿ�� ���

        GameObject bullet = Instantiate(bulletPrefab, firePos.position, firePos.rotation);
        bullet.GetComponent<Bullet>().actorNumber = actorNumber;
    }
}
