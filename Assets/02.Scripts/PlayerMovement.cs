using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun, IPunObservable
{
    private CharacterController controller;
    private new Transform transform;
    private Animator animator;
    private new Camera camera;
    private CinemachineVirtualCamera virtualCamera;
    private PhotonView photonView = null;
    private Plane plane; // ������ Plane �� ����ĳ���� �ϱ� ���� ���� 
    private Ray ray;
    private Vector3 hitPoint;
    private PlayerInput input;

    public float moveSpeed = 8f;
    public float trunSpeed = 90f;
    private Vector3 receivePos = Vector3.zero;
    private Quaternion receiveRot = Quaternion.identity;
    private float damping = 10f;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();
        camera = Camera.main;
        // ������ �ٴ��� ���ΰ��� ��ġ�� �������� ����
        plane = new Plane(transform.up, transform.position);
        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        photonView = GetComponent<PhotonView>();

        photonView.Synchronization = ViewSynchronization.UnreliableOnChange; //������ ���� ���� ����ȭ �Ѵ�.
        photonView.ObservedComponents[0] = this;
        // ������ this�� �����ڷ� �����Ѵ�
        if (photonView != null)
        {
            if (photonView.IsMine)
            {
                virtualCamera.Follow = transform;
                virtualCamera.LookAt = transform;
            }
        }
        receivePos = transform.position;
        receiveRot = transform.rotation;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting) //���� ���ΰ� ĳ���͸� �����ϴ� ���
        {
            //���� �����ϴ� ���ΰ� ĳ������ ��ġ�� ȸ������ ����
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else //�ٸ� �÷��̾ ���ΰ� ĳ���͸� �����ϴ� ���
        {
            //�ٸ� �÷��̾ �����ϴ� ���ΰ� ĳ������ ��ġ�� ȸ������ ����
            receivePos = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();
        }
    }
    void Update()
    {
        if (photonView.IsMine) // ����
        {
            Move();
            Turn();
        }
        else // ����Ʈ 
        {
            transform.position = Vector3.Lerp(transform.position, receivePos, Time.deltaTime * damping);
            //������ �����Ӱ� ���Ź��� ����Ʈ�� �������� ��Ʈ��ũ ����ȭ �Ǹ鼭 �ε巴�� �������� �Ѵ�.
            transform.rotation = Quaternion.Slerp(transform.rotation,receiveRot, Time.deltaTime * damping);
            //������ ȸ���� ���Ź��� ����Ʈ�� ȸ���� 
        }          
    }
        void Move()
        {
            Vector3 cameraForward = camera.transform.forward;
            Vector3 cameraRight = camera.transform.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            //�̵� �� ���� ���� ���
            Vector3 moveDir = (cameraForward * input.v) + (cameraRight * input.h);
            moveDir.Set(moveDir.x, 0f, moveDir.z);
            // ���ΰ� ĳ���� �̵� ó�� ĳ���� ��Ʈ�ѷ��� �̵� 
            controller.SimpleMove(moveDir * moveSpeed);
            //���ΰ� ĳ������ �ִϸ��̼� ó��
            float forward = Vector3.Dot(moveDir, transform.forward);
            float strafe = Vector3.Dot(moveDir, transform.right);

            animator.SetFloat("Forward", forward);
            animator.SetFloat("Strafe", strafe);
        }
        void Turn()
        {
            //���콺 2���� ��ǥ���� �̿��� 3���� ������ ���� 
            ray = camera.ScreenPointToRay(Input.mousePosition);
            float enter = 0f;
            //������ �ٴڿ� ���̸� �߻��� �浹�� ������ �Ÿ��� enter������ ��ȯ
            plane.Raycast(ray, out enter);
            // ������ �ٴڿ� ���̰� �浹�� ��ǥ�� ����
            hitPoint = ray.GetPoint(enter);
            //ȸ�� �ؾ� �� ������ ���͸� ��� 
            Vector3 lookDir = hitPoint - transform.position;
            lookDir.y = 0f;
            //���ΰ� ĳ������ ȸ���� ���� 
            transform.localRotation = Quaternion.LookRotation(lookDir);

        }

}
