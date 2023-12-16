using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using Steamworks.Ugc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Canvas canvas;
    public TMP_Text sensText;
    public TMP_Text fovText;
    public TMP_Text fovSprintText;

    public TMP_Text InventoryText;

    public Inventory inventory;

    public Slider sensSlider;
    public Slider fovSlider;
    public Slider fovSprintSlider;

    public Player player;
    public GameObject[] screens;
    public int currentScreen = 0;
    public TMP_Text notificationText;
    // Start is called before the first frame update
    // Update is called once per frame
    //0-Gameplay
    //1-MainMenu
    //2-Inventory

    public void onExitMainMenuClick()
    {
        setScreen(0);
    }

    public void onExitGameClick()
    {
        Application.Quit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (currentScreen == 0 || currentScreen == 1))
        {
            setScreen((currentScreen + 1) % 2);
        }
        else if ((Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape)) && (currentScreen == 0 || currentScreen == 2))
        {
            setScreen(currentScreen == 0 ? 2 : 0);
        }
        // if (mainMenu.activeSelf != isOpen) {
        //     mainMenu.SetActive(isOpen);
        // }
        if (currentScreen == 1)
        {
            //Main menu
            Cursor.lockState = CursorLockMode.None;
            player.sensitivity = sensSlider.value;
            player.defaultFov = fovSlider.value;
            player.sprintFov = fovSprintSlider.value;
            sensText.text = player.sensitivity.ToString();
            fovText.text = player.defaultFov.ToString();
            fovSprintText.text = player.sprintFov.ToString();
        }
        else if (currentScreen == 2)
        {
            //Inventory
            Cursor.lockState = CursorLockMode.None;
            string itemString = "";
            foreach (ItemSlot item in inventory.items)
            {
                itemString += String.Format("{0}: {1}\n", item.type.ToString(), item.amount);
            }
            InventoryText.text = itemString;
        }
        else
        {
            //Gameplay
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void setScreen(int screenIndex)
    {
        foreach (GameObject screen in screens)
        {
            screen.SetActive(false);
        }
        screens[screenIndex].SetActive(true);
        currentScreen = screenIndex;
    }

    public IEnumerator showNotification(string notification, float time)
    {
        notificationText.text = notification;
        yield return new WaitForSeconds(time);
        if (notification == notificationText.text)
        {
            notificationText.text = "";
        }
    }
}
