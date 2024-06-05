using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClearCounter : BaseCounter,IKitchenObjectParent
{
    [SerializeField] private KitchenObjectSo kitchenObjectSo;

    private void Start()
    {

    }
     public override void Interact(Player player)
    {
        if (!HasKitchenObject()) {
            // There is no KitchenObject here
            if(player.HasKitchenObject()) {
                
                player.GetKitchenObject().SetKitchenObjectParent(this);
                //drop it
            }
        }else {
            //There is a KitchenObject here
            if(!player.HasKitchenObject()) {
                //player is carrying nothing
                this.GetKitchenObject().SetKitchenObjectParent(player);
                //pick up
            }else {
                // player is carrying something
                if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    // Player is holding a plateKitchenObject

                    //PlateKitchenObject plateKitchenObject = player.GetKitchenObject() as PlateKitchenObject;
                    //PlateKitchenObject继承了KitchenObject，用as将KitchenObject转换成PlateKitchenObject

                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSo())) {
                        GetKitchenObject().DestroySelf();
                    }
                } else {
                    //Player is not holding a Plate but something else
                    if(GetKitchenObject().TryGetPlate(out  plateKitchenObject)) {
                        //Counter is holding a Plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSo())) {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }


            }
            
        }
    }



   
}
