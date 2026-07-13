using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow")]
    public float FollowSpeed = 8f;
    public Transform Target;

    [Header("Offset")]
    public float offsetY = 1f;

    [Header("Look Ahead")]
    public float lookAheadDistance = 2.5f;
    public float lookAheadSpeed = 5f;

    [Header("Look Down (mantener Abajo)")]
    public float lookDownDistance = 3f;
    public float lookDownSpeed = 4f;
    public float lookDownDelay = 0.35f;

    [Header("Camera Bounds")]
    public Vector2 minPosition;
    public Vector2 maxPosition;

    [Header("Shake")]
    public float shakeDuration = 0f;
    public float shakeAmount = 0.1f;
    public float decreaseFactor = 1.0f;

    private Transform camTransform;
    private float currentLookAheadX;
    private float lastTargetX;
    private float currentLookDownY;
    private float lookDownHeldTime;

    private void Awake()
    {
        Cursor.visible = false;
        camTransform = transform;

        if (Target != null)
        {
            lastTargetX = Target.position.x;
        }
    }

    private void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        // Detecta hacia dónde se está moviendo el jugador
        float movementDirection = Target.position.x - lastTargetX;

        if (Mathf.Abs(movementDirection) > 0.01f)
        {
            float targetLookAhead = Mathf.Sign(movementDirection) * lookAheadDistance;

            currentLookAheadX = Mathf.Lerp(
                currentLookAheadX,
                targetLookAhead,
                lookAheadSpeed * Time.deltaTime
            );
        }

        // Mirar hacia abajo al mantener presionado Abajo (estilo Hollow Knight)
        bool holdingDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        lookDownHeldTime = holdingDown ? lookDownHeldTime + Time.deltaTime : 0f;

        float targetLookDownY = lookDownHeldTime >= lookDownDelay ? -lookDownDistance : 0f;
        currentLookDownY = Mathf.Lerp(
            currentLookDownY,
            targetLookDownY,
            lookDownSpeed * Time.deltaTime
        );

        // Posición deseada de la cámara
        Vector3 newPosition = new Vector3(
            Target.position.x + currentLookAheadX,
            Target.position.y + offsetY + currentLookDownY,
            -10f
        );

        // Límites de la cámara
        newPosition.x = Mathf.Clamp(newPosition.x, minPosition.x, maxPosition.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minPosition.y, maxPosition.y);

        // Seguimiento suave
        transform.position = Vector3.Lerp(
            transform.position,
            newPosition,
            FollowSpeed * Time.deltaTime
        );

        lastTargetX = Target.position.x;

        // Efecto de temblor de cámara
        if (shakeDuration > 0)
        {
            Vector2 shakeOffset = UnityEngine.Random.insideUnitCircle * shakeAmount;

            camTransform.position = new Vector3(
                transform.position.x + shakeOffset.x,
                transform.position.y + shakeOffset.y,
                transform.position.z
            );

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
    }

    public void ShakeCamera()
    {
        shakeDuration = 0.2f;
    }
}