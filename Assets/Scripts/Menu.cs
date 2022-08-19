using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    public void StartButton()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void BuyAmmo()
    {
        if (PlayerPrefs.GetInt("Ammo") >= 1)
        {
            return;
        }
        IAP.Instance.BuyProductID("unlimited.bullet");
    }
}
