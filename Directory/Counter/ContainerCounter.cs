using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter, IKitchenObjectParent
{

    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSo kitchenObjectSo;

    public override void Interact(Player player)
    {
        if(!player.HasKitchenObject()) {
            //When Player is not carrying anything,he can take something out of container
            Transform kitchenObjectTransform = Instantiate(kitchenObjectSo.prefab);
            kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(player);

            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }

   
}
