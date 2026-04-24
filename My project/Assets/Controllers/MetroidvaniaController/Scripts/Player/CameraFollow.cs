using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float FollowSpeed = 2f;
    public Transform Target;

    [Header("Camera Bounds")]
    public Vector2 minPosition; // El límite inferior izquierdo
    public Vector2 maxPosition; // El límite superior derecho

    private Transform camTransform;
    public float shakeDuration = 0f;
    public float shakeAmount = 0.1f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;

    void Awake()
    {
        Cursor.visible = false;
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    // Cambiado a LateUpdate para un seguimiento más suave
    private void LateUpdate()
    {
        if (Target != null)
        {
            Vector3 newPosition = Target.position;
            newPosition.z = -10;

            // --- AQUI APLICAMOS LOS LÍMITES ---
            // Evitamos que la cámara pase de las posiciones mínimas y máximas
            newPosition.x = Mathf.Clamp(newPosition.x, minPosition.x, maxPosition.x);
            newPosition.y = Mathf.Clamp(newPosition.y, minPosition.y, maxPosition.y);

            transform.position = Vector3.Lerp(transform.position, newPosition, FollowSpeed * Time.deltaTime);
        }

        if (shakeDuration > 0)
        {
            camTransform.localPosition = transform.position + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
    }

    public void ShakeCamera()
    {
        shakeDuration = 0.2f;
    }
}