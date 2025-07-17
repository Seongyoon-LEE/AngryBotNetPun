using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    private CharacterController controller;
    private new Transform transform;
    private Animator animator;
    private new Camera camera;
    private CinemachineVirtualCamera virtualCamera;
    private PhotonView photonView = null;
    private Plane plane; // 가상의 Plane 에 레이캐스팅 하기 위한 변수 
    private Ray ray;
    private Vector3 hitPoint;
    private PlayerInput input;



    public float moveSpeed = 8f;
    public float trunSpeed = 90f;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();
        camera = Camera.main;
        // 가상의 바닥을 주인공의 위치를 기준으로 생성
        plane = new Plane(transform.up, transform.position);
        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        photonView = GetComponent<PhotonView>();

        if (photonView != null)
        {
            if (photonView.IsMine)
            {
                virtualCamera.Follow = transform;
                virtualCamera.LookAt = transform;
            }
        }
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
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            //이동 할 방향 벡터 계산
            Vector3 moveDir = (cameraForward * input.v) + (cameraRight * input.h);
            moveDir.Set(moveDir.x, 0f, moveDir.z);
            // 주인공 캐릭터 이동 처리 캐릭터 컨트롤러로 이동 
            controller.SimpleMove(moveDir * moveSpeed);
            //주인공 캐릭터의 애니메이션 처리
            float forward = Vector3.Dot(moveDir, transform.forward);
            float strafe = Vector3.Dot(moveDir, transform.right);

            animator.SetFloat("Forward", forward);
            animator.SetFloat("Strafe", strafe);
        }
        void Turn()
        {
            //마우스 2차원 좌표값을 이용해 3차원 광선을 생성 
            ray = camera.ScreenPointToRay(Input.mousePosition);
            float enter = 0f;
            //가상의 바닥에 레이를 발사해 충돌한 지점의 거리를 enter변수로 반환
            plane.Raycast(ray, out enter);
            // 가상의 바닥에 레이가 충돌한 좌표값 추출
            hitPoint = ray.GetPoint(enter);
            //회전 해야 할 방향의 벡터를 계산 
            Vector3 lookDir = hitPoint - transform.position;
            lookDir.y = 0f;
            //주인공 캐릭터의 회전값 지정 
            transform.localRotation = Quaternion.LookRotation(lookDir);

        }
    }
