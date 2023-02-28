using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    // Start is called before the first frame update
   public void StartMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void ControlesM(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

     public void VolverM(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    // Update is called once per frame
    public void Salir()
    {
        Application.Quit();
    }
}
