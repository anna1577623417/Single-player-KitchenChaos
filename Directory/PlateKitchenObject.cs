using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs {
        public KitchenObjectSo KitchenObjectSo;
    }


    [SerializeField] private List<KitchenObjectSo> vaildKitchenObjectSOList;
    private List<KitchenObjectSo> kitchenObjectSOList;

    private void Awake() {
        kitchenObjectSOList = new List<KitchenObjectSo>();
    }
    public bool TryAddIngredient(KitchenObjectSo kitchenObjectSo) {
        if(!vaildKitchenObjectSOList.Contains(kitchenObjectSo)) {
            //Not a vaild ingredient
            return false;
        }

        if (kitchenObjectSOList.Contains(kitchenObjectSo)) {
            //Already has this type
            return false;
        }else {
            kitchenObjectSOList.Add(kitchenObjectSo);

            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs {
                KitchenObjectSo = kitchenObjectSo
            }) ;
            return true;
        }
        
    }

    public List<KitchenObjectSo> GetKitchenObjectSOList() {
        return kitchenObjectSOList;
    }
}
