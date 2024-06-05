using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionsButton;

    private void Start() {
        KitchenGameManager.instance.OnGamePaused += KitchenGameManger_OnGamePaused;
        KitchenGameManager.instance.OnGameUnPaused += KitchenGameManger_OnGameUnPaused;
        if (gameObject.activeSelf) {
            Hide();
        }
    }
    private void Awake() {
        resumeButton.onClick.AddListener(() => { 
            KitchenGameManager.instance.TogglePauseGame();//暂停时按下按钮继续游戏，取消暂停
        });
        mainMenuButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        optionsButton.onClick.AddListener(() => {
            Hide();
            OptionsUI.instance.ShowOnCloseButton(Show);
        });
    }

    private void KitchenGameManger_OnGamePaused(object sender, System.EventArgs e) {
        Show();

    }

    private void KitchenGameManger_OnGameUnPaused(object sender, System.EventArgs e) {
        Hide();
    }

    private void Show() {
        gameObject.SetActive(true);
        resumeButton.Select();
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}
