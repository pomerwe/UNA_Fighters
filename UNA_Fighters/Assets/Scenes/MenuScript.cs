using System;
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

    private Vector2 selectedSize = new Vector2(700, 700);
    private Vector2 normalSize = new Vector2(400, 400);

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

        if (Input.anyKeyDown)
        {
            foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
{
                if (Input.GetKey(kcode))
                    Debug.Log("KeyCode down: " + kcode);
            }
        }
        

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || (Input.GetAxis("Vertical") == -1) || (Input.GetAxis("Vertical") == 1f)
            || (Input.GetAxis("p2x") == 1f) || (Input.GetAxis("p2y") == -1f))
        {
            selectedOption = selectedOption == Options.Start ? Options.Exit : Options.Start;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button9) || Input.GetKey(KeyCode.Joystick2Button9))
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