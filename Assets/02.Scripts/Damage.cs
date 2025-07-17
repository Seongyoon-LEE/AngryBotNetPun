using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] Renderer[] renderers;
    [SerializeField] int initHp = 100;
    [SerializeField] int curHp = 0;
    Animator animator;
    CharacterController cc;
    WaitForSeconds wsPlayerDie;

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
    }
    private void OnCollisionEnter(Collision c)
    {
        if (curHp > 0 && c.collider.CompareTag(bulletTag))
        {
            curHp -= 20;
            curHp = Mathf.Clamp(curHp, 0, initHp);
            if (curHp <= 0)
            {
                StartCoroutine(PlayerDie());
            }
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
    }
    void SetVisible(bool visible)
    {
        foreach(Renderer renderer in renderers) { renderer.enabled = visible; }
    }
}
