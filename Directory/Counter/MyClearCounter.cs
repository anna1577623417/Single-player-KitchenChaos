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
            //Instantiate �������ص���ʵ���������Ϸ��������ã������ǵ����� Transform ���
            //tomatoTransform ��������ʵ���������Ϸ����� Transform ���
            //���Ԥ�����ڳ����еĳ�ʼλ���� (0, 0, 0)����ô�´�����ʵ��Ҳ������ͬ��λ�á�
            kitchenObject.localPosition = Vector3.zero;
            //��֤��������ڸ�����λ����ȷ
            IsEmpty = false;
        } else if (kitchenObject != null && !IsEmpty) {
            Destroy(kitchenObject.gameObject);
            IsEmpty = true;
        }
    }
}

}
