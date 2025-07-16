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
    PlayerInput playerInput; // PlayerInput 스크립트 참조 변수 추가

    Plane plane; // 가상의 Plane에 레이캐스팅 하기 위한 변수 
    Ray ray;
    Vector3 hitPoint;

    [SerializeField] float moveSpeed = 8f; // 이동 속도
    [SerializeField] float turnSpeed = 720f; // 회전 속도
    void Start()
    {
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        camera = Camera.main;
        playerInput = GetComponent<PlayerInput>(); // PlayerInput 스크립트 컴포넌트 가져오기
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
        cameraForward.y = 0f; // Y축 회전을 방지
        cameraRight.y = 0f; // Y축 회전을 방지
        // 이동 할 방향 벡터 계산 
        Vector3 moveDir = (cameraForward * playerInput.v) + (cameraRight * playerInput.h);
        moveDir.Set(moveDir.x,0f, moveDir.z); // Y축 회전을 방지
        // 주인공 캐릭터 이동 처리 캐릭터 컨트롤러로 이동 
        controller.SimpleMove(moveDir * moveSpeed); // 이동 속도는 8로 설정
        // 주인공 캐릭터의 애니메이션 처리 
        float forward = Vector3.Dot(moveDir,transform.forward); // 전진 방향 벡터
        float strafe = Vector3.Dot(moveDir, transform.right); // 좌우 방향 벡터

        animator.SetFloat("Forward", forward); // 전진 애니메이션
        animator.SetFloat("Strafe", strafe); // 좌우 애니메이션
    }
    void Turn()
    {   // 마우스 2차원 좌표값을 이용해 3차원 광선을 생성 
        ray = camera.ScreenPointToRay(Input.mousePosition); // 마우스 위치를 기준으로 Ray 생성
        float enter = 0f;
        // 가상의 바닥에 레이를 발사해 충돌한 지점의 거리를 enter변수로 반환 
        plane.Raycast(ray, out enter); // Ray와 Plane의 교차점 계산
        // 가상의 바닥에 레이가 충돌한 좌표값 추출 
        hitPoint = ray.GetPoint(enter); // Ray가 Plane과 만나는 지점의 좌표를 hitPoint에 저장
        // 회전 해야 할 방향의 벡터를 계산
        Vector3 lookDir = hitPoint - transform.position; // 현재 위치에서 hitPoint까지의 벡터 계산
        lookDir.y = 0f; // Y축 회전을 방지
        // 주인공 캐릭터의 회전 값 지정
        transform.localRotation = Quaternion.LookRotation(lookDir); // lookDir 방향으로 회전
    }
}
