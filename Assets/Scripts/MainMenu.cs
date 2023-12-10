using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Canvas canvas;
    public bool isOpen = false;
    public TMP_Text sensText;
    public TMP_Text fovText;
    public TMP_Text fovSprintText;

    public Slider sensSlider;
    public Slider fovSlider;
    public Slider fovSprintSlider;

    public Player player;
    // Start is called before the first frame update
    // Update is called once per frame
    
    public void onExitMainMenuClick() {
        isOpen = false;
    }

    public void onExitGameClick() {
        Application.Quit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            isOpen = !isOpen;
        }
        if (canvas.enabled != isOpen) {
            canvas.enabled = isOpen;
        }
        if (isOpen) {
            Cursor.lockState = CursorLockMode.None;
            player.sensitivity = sensSlider.value;
            player.defaultFov = fovSlider.value;
            player.sprintFov = fovSprintSlider.value;
            sensText.text = player.sensitivity.ToString();
            fovText.text = player.defaultFov.ToString();
            fovSprintText.text = player.sprintFov.ToString();
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
