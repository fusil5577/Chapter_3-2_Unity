using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float JumpPower;
    private Vector2 curMovementInput;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform camaraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;

    private Rigidbody _rigidbody;
    private Camera mainCamera;
    private Vector3 originalCameraLocalPosition;
    private bool isThirdPerson = false;
    public float thirdPersonZ = -4.0f; // 3인칭 시점의 z 위치
    public GameObject basicMotionsDummy;
    private bool hasExtraJump = false;
    public Image extraJumpGauge;

    public Action inventory;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;  // 메인 카메라 변수
        originalCameraLocalPosition = mainCamera.transform.localPosition; // 카메라 초기 위치
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        basicMotionsDummy.SetActive(isThirdPerson);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        CameraLook();
    }

    void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y;

        _rigidbody.velocity = dir;
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        camaraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) +(transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (IsGrounded())
            {
                Jump(JumpPower);
            }
            else if (hasExtraJump)
            {
                Jump(JumpPower * 1.5f);
                hasExtraJump = false;
                extraJumpGauge.fillAmount = 0f;
            }
        }
    }

    private void Jump(float power)
    {
        _rigidbody.AddForce(Vector2.up * power, ForceMode.Impulse);
    }

    public void OnItemCollected()
    {
        hasExtraJump = true;
        extraJumpGauge.fillAmount = 1f;
    }

    public void OnThirdPerson(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            isThirdPerson = !isThirdPerson; // 토글
            float targetZ = isThirdPerson ? thirdPersonZ : originalCameraLocalPosition.z;

            StartCoroutine(LerpCameraZ(targetZ, 0.5f)); // 화면 변화 시간 0.5초
        }

        IEnumerator LerpCameraZ(float targetZ, float duration)
        {
            Vector3 startPosition = mainCamera.transform.localPosition; // 현재 위치
            Vector3 endPosition = originalCameraLocalPosition + Vector3.forward * (targetZ - originalCameraLocalPosition.z); // 변경 위치

            for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime)
            {
                mainCamera.transform.localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration); // 보간을 이용한 점진적인 변화
                yield return null;
            }

            mainCamera.transform.localPosition = endPosition;
        }
    }

    public void IncreaseSpeed(float speedValue)
    {
        StartCoroutine(ModifySpeed(speedValue));
    }

    private IEnumerator ModifySpeed(float speedValue)
    {
        moveSpeed += speedValue;
        yield return new WaitForSeconds(5f);
        moveSpeed -= speedValue;
    }

    public void IncreasePower(float jumpValue)
    {
        StartCoroutine(ModifyPower(jumpValue));
    }

    private IEnumerator ModifyPower(float jumpValue)
    {
        JumpPower += jumpValue;
        yield return new WaitForSeconds(5f);
        JumpPower -= jumpValue;
    }
}

