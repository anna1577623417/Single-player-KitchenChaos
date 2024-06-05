using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CuttingCounter : BaseCounter,IHasProgress
{
    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData() {
        OnAnyCut = null;
    }

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;


    [SerializeField] private CuttingRecipeSO[] cutKitchenObjectSOArray;
    public event EventHandler Oncut;


    private int cuttingProgress;


    public override void Interact(Player player)
    {
        if (!HasKitchenObject()) {
            // There is no KitchenObject here
            if (player.HasKitchenObject()) {

                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSo())) {
                    // when player is carrying a cuttable thing,he can drop it on the cutting counter
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;

                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSo());

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized =(float) cuttingProgress/ cuttingRecipeSO.cuttingProgressMax
                    });
                        
                }
            }
        } else {
            //There is a KitchenObject here
            if (!player.HasKitchenObject()) {
                //player is carrying nothing
                this.GetKitchenObject().SetKitchenObjectParent(player);
                //pick up
            } else {
                // player is carrying something
                if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    // Player is holding a plateKitchenObject

                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSo())) {
                        GetKitchenObject().DestroySelf();
                    }
                }
            }
        }
    }
    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject()&&HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSo())) {
            //begin  to cut
            cuttingProgress++;
            
            Oncut?.Invoke(this,EventArgs.Empty);
            //Debug.Log(OnAnyCut.GetInvocationList().Length);不做clear static会导致有多个订阅者
            OnAnyCut?.Invoke(this,EventArgs.Empty);

            CuttingRecipeSO cuttingRecipeSO =  GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSo());

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
             progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
            });
            //There is a KitchenObject here And it can be cut
            if (cuttingProgress>= cuttingRecipeSO.cuttingProgressMax) {
                KitchenObjectSo outputKitchenObjectSo = GetOutputForInput(GetKitchenObject().GetKitchenObjectSo());
                GetKitchenObject().DestroySelf();
                KitchenObject.SpwanKitchenObject(outputKitchenObjectSo, this);
            }
        }
    }
    private bool HasRecipeWithInput(KitchenObjectSo inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cutKitchenObjectSOArray) {
            if (cuttingRecipeSO.input == inputKitchenObjectSO) {
                return true;
            }
        }
        return false;
    }

    private KitchenObjectSo GetOutputForInput(KitchenObjectSo inputKitchenObjectSO)
    {
        foreach(CuttingRecipeSO cuttingRecipeSO in cutKitchenObjectSOArray) {
            if(cuttingRecipeSO.input==inputKitchenObjectSO) {
                return cuttingRecipeSO.output;
            }
        }
        return null;
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSo inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cutKitchenObjectSOArray) {
            if (cuttingRecipeSO.input == inputKitchenObjectSO) {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}
