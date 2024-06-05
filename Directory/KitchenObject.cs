using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSo kitchenObjectSo;

    private IKitchenObjectParent kitchenObjectParent;

    private ClearCounter clearCounter;
    private void Start()
    {
        
    }

    public KitchenObjectSo GetKitchenObjectSo()
    {
        return kitchenObjectSo; 
    }
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
         if(this.kitchenObjectParent != null) {
            this.kitchenObjectParent.ClearKitchenObject();
            //放置物品前物品已经在柜台上，要先清除这个柜台上的道具信息（把物体拿起来）
        }

        this.kitchenObjectParent = kitchenObjectParent;
        if (kitchenObjectParent.HasKitchenObject()) {
            Debug.LogError("IKitchenObjectParent already has a kitchenObject!");
        }
        kitchenObjectParent.SetKitchenObject(this);

        transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform(); 
        transform.localPosition = Vector3.zero;
    }
    public IKitchenObjectParent GetIKitchenObjectParent()
    {
        return kitchenObjectParent;
    }
    public void DestroySelf()
    {
        kitchenObjectParent.ClearKitchenObject() ;

        Destroy(gameObject);
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
        if(this is PlateKitchenObject) {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }else {
            plateKitchenObject = null;
            return false;
        }
    }

    public static KitchenObject SpwanKitchenObject(KitchenObjectSo kitchenObjectSO,IKitchenObjectParent kitchenObjectParent )
    {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
          
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);     

        return kitchenObject;

    }
}
