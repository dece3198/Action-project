using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartButton()
    {
        LoadingController.LoadScene("Step");
    }

    public void EndButton()
    {
        Application.Quit();
    }
}
