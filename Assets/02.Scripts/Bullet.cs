using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject hit_Effect; // �Ѿ��� �¾��� �� ������ ����Ʈ
    public int actorNumber; // �Ѿ��� �߻��� �÷��̾��� ������ȣ

    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 1000f); // �Ѿ��� ������ ���ư����� ���� �߰�
        //Unity ���� �������� ������Ʈ�� ������� ���� �������� ���� ���ϴ°�.
        //AddForce : ���� ��ǥ�� ���� (trnansform.forward ���� �������� ���� ����)
        //AddForceRelativeTo : ���� ��ǥ�� ���� (������Ʈ�� ���� ���� �������� ���� ����)
        Destroy(gameObject, 3f); // 3�� �Ŀ� �Ѿ� ������Ʈ�� �ı�
    }

    private void OnCollisionEnter(Collision col)
    {
        var contact = col.GetContact(0); // �浹 ������ ������ ������
        var obj = Instantiate(hit_Effect, contact.point, Quaternion.LookRotation(-contact.normal)); // �浹 ������ ����Ʈ�� ����
        Destroy(obj, 2f); // ������ ����Ʈ�� 1�� �Ŀ� �ı�
        Destroy(this.gameObject, 0.25f); // �Ѿ� ������Ʈ�� 0.25�� �Ŀ� �ı�
    }
}
