using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuActions : MonoBehaviour
{
    public void Jugar(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);//carga la siguiente escena en el orden del build settings
    }

    public void Salir(){
        Debug.Log("saliendo del juego...");
        Application.Quit();//cierra la aplicación, no funciona en el editor de Unity pero si en el juego compilado
    }
}
