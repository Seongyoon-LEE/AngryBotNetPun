using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController controller;
    new Transform transform;
    Animator animator;
    new Camera camera;
    PlayerInput playerInput; // PlayerInput ��ũ��Ʈ ���� ���� �߰�

    Plane plane; // ������ Plane�� ����ĳ���� �ϱ� ���� ���� 
    Ray ray;
    Vector3 hitPoint;

    [SerializeField] float moveSpeed = 8f; // �̵� �ӵ�
    [SerializeField] float turnSpeed = 720f; // ȸ�� �ӵ�
    void Start()
    {
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        camera = Camera.main;
        playerInput = GetComponent<PlayerInput>(); // PlayerInput ��ũ��Ʈ ������Ʈ ��������
    }

    void Update()
    {
        Move();
        Turn();
    }

    void Move()
    {
        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;
        cameraForward.y = 0f; // Y�� ȸ���� ����
        cameraRight.y = 0f; // Y�� ȸ���� ����
        // �̵� �� ���� ���� ��� 
        Vector3 moveDir = (cameraForward * playerInput.v) + (cameraRight * playerInput.h);
        moveDir.Set(moveDir.x,0f, moveDir.z); // Y�� ȸ���� ����
        // ���ΰ� ĳ���� �̵� ó�� ĳ���� ��Ʈ�ѷ��� �̵� 
        controller.SimpleMove(moveDir * moveSpeed); // �̵� �ӵ��� 8�� ����
        // ���ΰ� ĳ������ �ִϸ��̼� ó�� 
        float forward = Vector3.Dot(moveDir,transform.forward); // ���� ���� ����
        float strafe = Vector3.Dot(moveDir, transform.right); // �¿� ���� ����

        animator.SetFloat("Forward", forward); // ���� �ִϸ��̼�
        animator.SetFloat("Strafe", strafe); // �¿� �ִϸ��̼�
    }
    void Turn()
    {   // ���콺 2���� ��ǥ���� �̿��� 3���� ������ ���� 
        ray = camera.ScreenPointToRay(Input.mousePosition); // ���콺 ��ġ�� �������� Ray ����
        float enter = 0f;
        // ������ �ٴڿ� ���̸� �߻��� �浹�� ������ �Ÿ��� enter������ ��ȯ 
        plane.Raycast(ray, out enter); // Ray�� Plane�� ������ ���
        // ������ �ٴڿ� ���̰� �浹�� ��ǥ�� ���� 
        hitPoint = ray.GetPoint(enter); // Ray�� Plane�� ������ ������ ��ǥ�� hitPoint�� ����
        // ȸ�� �ؾ� �� ������ ���͸� ���
        Vector3 lookDir = hitPoint - transform.position; // ���� ��ġ���� hitPoint������ ���� ���
        lookDir.y = 0f; // Y�� ȸ���� ����
        // ���ΰ� ĳ������ ȸ�� �� ����
        transform.localRotation = Quaternion.LookRotation(lookDir); // lookDir �������� ȸ��
    }
}
