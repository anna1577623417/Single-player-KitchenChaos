using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyClearCounter : MonoBehaviour
{


public class ClearCounter : MonoBehaviour
{
    [SerializeField] private KitchenObjectSo kitchenObjectSo;
    [SerializeField] private Transform CounterTopPoint;
    Transform kitchenObject;
    private bool IsEmpty;

    private void Start()
    {
        IsEmpty = true;
    }
    public void Interact()
    {
        if (kitchenObjectSo != null && IsEmpty) {
            kitchenObject = Instantiate(kitchenObjectSo.prefab, CounterTopPoint);
            //Instantiate 函数返回的是实例化后的游戏对象的引用，而不是单纯的 Transform 组件
            //tomatoTransform 将会引用实例化后的游戏对象的 Transform 组件
            //如果预制体在场景中的初始位置是 (0, 0, 0)，那么新创建的实例也会在相同的位置。
            kitchenObject.localPosition = Vector3.zero;
            //保证物体相对于父物体位置正确
            IsEmpty = false;
        } else if (kitchenObject != null && !IsEmpty) {
            Destroy(kitchenObject.gameObject);
            IsEmpty = true;
        }
    }
}

}
