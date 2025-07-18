using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject hit_Effect; // 총알이 맞았을 때 생성할 이펙트
    public int actorNumber; // 총알을 발사한 플레이어의 고유번호

    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 1000f); // 총알이 앞으로 날아가도록 힘을 추가
        //Unity 게임 엔진에서 오브젝트에 상대적인 전방 방향으로 힘을 가하는것.
        //AddForce : 월드 좌표계 기준 (trnansform.forward 방향 기준으로 힘을 가함)
        //AddForceRelativeTo : 로컬 좌표계 기준 (오브젝트의 앞쪽 방향 기준으로 힘을 가함)
        Destroy(gameObject, 3f); // 3초 후에 총알 오브젝트를 파괴
    }

    private void OnCollisionEnter(Collision col)
    {
        var contact = col.GetContact(0); // 충돌 지점의 정보를 가져옴
        var obj = Instantiate(hit_Effect, contact.point, Quaternion.LookRotation(-contact.normal)); // 충돌 지점에 이펙트를 생성
        Destroy(obj, 2f); // 생성된 이펙트를 1초 후에 파괴
        Destroy(this.gameObject, 0.25f); // 총알 오브젝트를 0.25초 후에 파괴
    }
}
