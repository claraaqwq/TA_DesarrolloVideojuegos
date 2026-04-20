using UnityEngine;

public class PlayerInput2D : MonoBehaviour
{
    // Metodo para obtener el movimiento horizontal
    public float ObtenerMovimientoHorizontal()
    {
        float movimiento = Input.GetAxisRaw("Horizontal");
        return movimiento;
    }

    // Metodo para saltar
    public bool ObtenerSalto()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    // Metodo para dash
    public bool ObtenerDash()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    // Metodo para blink
    public bool ObtenerBlink()
    {
        return Input.GetKeyDown(KeyCode.E);
    }
}
