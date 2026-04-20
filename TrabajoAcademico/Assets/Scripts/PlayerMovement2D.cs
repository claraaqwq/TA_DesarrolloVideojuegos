using UnityEngine;
using System.Collections;

public class PlayerMovement2D : MonoBehaviour
{
    // Variables de movimiento
    [SerializeField] private float velocidad;
    [SerializeField] private float fuerzaSalto;

    // Variables de dash
    [SerializeField] private float fuerzaDash;
    [SerializeField] private float tiempoDash;

    // Variables de blink
    [SerializeField] private float distanciaBlink;
    [SerializeField] private LayerMask capaBloqueoBlink;

    // Variables para detectar suelo
    [SerializeField] private Transform detectorSuelo;
    [SerializeField] private float radioSuelo;
    [SerializeField] private LayerMask capaSuelo;

    // Variable para una caida mas rapida
    [SerializeField] private float multiplicadorCaida = 2.5f;

    // Referencias
    public PlayerInput2D inputPlayer;

    private Rigidbody2D rb;

    // Variables de control
    private float movimientoHorizontal;
    private bool enSuelo;
    private bool mirandoDerecha = true;
    private bool estaDasheando = false;
    private float gravedadInicial;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gravedadInicial = rb.gravityScale;
    }

    void Update()
    {
        // Detecto si esta en el suelo
        enSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioSuelo, capaSuelo);

        // Si esta haciendo dash, no hago el resto
        if (estaDasheando)
        {
            return;
        }

        // Obtengo el movimiento horizontal
        movimientoHorizontal = inputPlayer.ObtenerMovimientoHorizontal();

        // Giro al personaje segun a donde camina
        GirarPersonaje();

        // Salto
        if (inputPlayer.ObtenerSalto() && enSuelo)
        {
            Saltar();
        }

        // Dash
        if (inputPlayer.ObtenerDash())
        {
            StartCoroutine(HacerDash());
        }

        // Blink
        if (inputPlayer.ObtenerBlink())
        {
            HacerBlink();
        }
    }

    private void FixedUpdate()
    {
        // Si esta haciendo dash, no aplico movimiento normal
        if (estaDasheando)
        {
            return;
        }

        // Movimiento izquierda / derecha
        rb.linearVelocity = new Vector2(movimientoHorizontal * velocidad, rb.linearVelocity.y);

        // Caida mas rapida
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (multiplicadorCaida - 1) * Time.fixedDeltaTime;
        }
    }

    private void Saltar()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
    }

    private IEnumerator HacerDash()
    {
        estaDasheando = true;

        // Si no presiona izquierda o derecha, dasha hacia donde esta mirando
        float direccionDash;

        if (movimientoHorizontal != 0)
        {
            direccionDash = Mathf.Sign(movimientoHorizontal);
        }
        else
        {
            direccionDash = mirandoDerecha ? 1f : -1f;
        }

        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(direccionDash * fuerzaDash, 0f);

        yield return new WaitForSeconds(tiempoDash);

        rb.gravityScale = gravedadInicial;
        estaDasheando = false;
    }

    private void HacerBlink()
    {
        // Si no presiona izquierda o derecha, blink hacia donde esta mirando
        float direccionBlink;

        if (movimientoHorizontal != 0)
        {
            direccionBlink = Mathf.Sign(movimientoHorizontal);
        }
        else
        {
            direccionBlink = mirandoDerecha ? 1f : -1f;
        }

        Vector2 origen = transform.position;
        Vector2 direccion = Vector2.right * direccionBlink;

        // Raycast para no atravesar paredes
        RaycastHit2D hit = Physics2D.Raycast(origen, direccion, distanciaBlink, capaBloqueoBlink);

        if (hit.collider != null)
        {
            float ajuste = 0.2f;
            Vector2 destino = new Vector2(hit.point.x - (ajuste * direccionBlink), transform.position.y);
            rb.position = destino;
        }
        else
        {
            Vector2 destino = rb.position + (Vector2.right * direccionBlink * distanciaBlink);
            rb.position = destino;
        }
    }

    private void GirarPersonaje()
    {
        if (movimientoHorizontal > 0 && !mirandoDerecha)
        {
            Voltear();
        }
        else if (movimientoHorizontal < 0 && mirandoDerecha)
        {
            Voltear();
        }
    }

    private void Voltear()
    {
        mirandoDerecha = !mirandoDerecha;

        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    // Esto es opcional, solo para detectar colisiones con enemigos u otros objetos
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            Debug.Log("Choque con un enemigo");
        }
    }

    // Para ver el detector del suelo en la escena
    private void OnDrawGizmosSelected()
    {
        if (detectorSuelo != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(detectorSuelo.position, radioSuelo);
        }
    }
}
