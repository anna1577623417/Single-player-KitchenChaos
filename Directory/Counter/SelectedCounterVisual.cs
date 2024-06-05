using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] VisualGameObjectArray ;
    private void Start()
    {
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    private void Player_OnSelectedCounterChanged(object sender,Player.OnSelectedCounterChangedEventArgs e)
    {
        if(e.selectedCounter == baseCounter) {
            Show();
        } else {
            Hide();
        }
    }

    private void Show()
    {
        foreach (GameObject VisualGameObject in VisualGameObjectArray) {
                VisualGameObject.SetActive(true);// 因为在编辑器阶段就赋值好变量了，所以每一个理应都不是空的，无需检查
        }

    }

    private void Hide()
    {
        foreach (GameObject VisualGameObject in VisualGameObjectArray) {
                VisualGameObject.SetActive(false);
        }
    }

}
