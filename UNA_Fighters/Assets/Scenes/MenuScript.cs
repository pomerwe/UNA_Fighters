using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    private Options selectedOption;

    public Image startButton;
    public RectTransform startRect;
    public Image exitButton;
    public RectTransform exitRect;

    private Vector2 selectedSize = new Vector2(500, 500);
    private Vector2 normalSize = new Vector2(385, 385);

    private void Start()
    {
        selectedOption = Options.Start;
    }

    private void Update()
    {
        switch (selectedOption)
        {
            case Options.Start:
                {
                    startRect.sizeDelta = selectedSize;
                    exitRect.sizeDelta = normalSize;

                    startButton.color = Color.yellow;
                    exitButton.color = Color.white;
                    break;
                }
            case Options.Exit:
                {
                    startRect.sizeDelta = normalSize;
                    exitRect.sizeDelta = selectedSize;

                    startButton.color = Color.white;
                    exitButton.color = Color.yellow;
                    break;
                }
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKey(KeyCode.Joystick1Button13)
            || Input.GetKeyDown(KeyCode.Joystick2Button0) || Input.GetKey(KeyCode.Joystick2Button13))
        {
            selectedOption = selectedOption == Options.Start ? Options.Exit : Options.Start;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(selectedOption == Options.Start)
            {
                SceneManager.LoadScene("MainScene");
            }
            else
            {
                Application.Quit();
            }
        }
    }
}

public enum Options
{
    Start,
    Exit
}