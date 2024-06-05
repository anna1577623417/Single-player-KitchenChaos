using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static CuttingCounter;

public class StoveCounter : BaseCounter,IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs {
        public State state;
    }
    public enum State {
        Idle,
        Frying,
        Fried,
        Burned,
    }


    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSo[] burningRecipeSOArray;
    FryingRecipeSO fryingRecipeSO;
    private float fryingTimer;
    private float burningTimer;
    private BurningRecipeSo burningRecipeSO;
    private State state;

    private void Start() {
        state = State.Idle;
    }

    private void Update() {
        if (HasKitchenObject()) {
            switch (state) {

            case State.Idle:
                break;
            case State.Frying:
                fryingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimeMax
                    });

                    if (fryingTimer > fryingRecipeSO.fryingTimeMax) {
                    //fried
                    GetKitchenObject().DestroySelf();

                    KitchenObject.SpwanKitchenObject(fryingRecipeSO.output, this);

                    state = State.Fried;
                     burningTimer = 0f;
                     burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSo());

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                            state = state
                        });
                    }
               //Debug.Log(fryingTimer);
                break;

            case State.Fried:
                    burningTimer += Time.deltaTime;

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = burningTimer / burningRecipeSO.burningTimeMax
                    });

                    if (burningTimer > burningRecipeSO.burningTimeMax) {
                        //fried
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpwanKitchenObject(burningRecipeSO.output, this);

                        state = State.Burned;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                            state = state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                            progressNormalized = 0f
                        });

                    }
                    break;

            case State.Burned:
                break;
            }
        } else {
            state= State.Idle;
        }
    }

    //private void Start() {
    //    StartCoroutine(HandleFryTimer());
    //}
    //private IEnumerator HandleFryTimer() {
    //    yield return new WaitForSeconds(1f);
    //}

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            // There is no KitchenObject here
            if (player.HasKitchenObject()) {

                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSo())) {
                    // when player is carrying a cuttable thing,he can drop it on the cutting counter
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSo());

                    state = State.Frying;
                    fryingTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = fryingTimer/fryingRecipeSO.fryingTimeMax
                    });
                }
            }
        } else {
            //There is a KitchenObject here
            if (!player.HasKitchenObject()) {
                //player is carrying nothing
                this.GetKitchenObject().SetKitchenObjectParent(player);
                //pick up
                state = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                    state = state
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                    progressNormalized = 0f
                });
            } else {
                // player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    // Player is holding a plateKitchenObject

                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSo())) {
                        GetKitchenObject().DestroySelf();
                    }
                    state = State.Idle;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = 0f
                    });
                }
            }
        }
    }


    private bool HasRecipeWithInput(KitchenObjectSo inputKitchenObjectSO) {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray) {
            if (fryingRecipeSO.input == inputKitchenObjectSO) {
                return true;
            }
        }
        return false;
    }

    private KitchenObjectSo GetOutputForInput(KitchenObjectSo inputKitchenObjectSO) {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray) {
            if (fryingRecipeSO.input == inputKitchenObjectSO) {
                return fryingRecipeSO.output;
            }
        }
        return null;
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSo inputKitchenObjectSO) {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray) {
            if (fryingRecipeSO.input == inputKitchenObjectSO) {
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private BurningRecipeSo GetBurningRecipeSOWithInput(KitchenObjectSo inputKitchenObjectSO) {
        foreach (BurningRecipeSo burningRecipeSO in burningRecipeSOArray) {
            if (burningRecipeSO.input == inputKitchenObjectSO) {
                return burningRecipeSO;
            }
        }
        return null;
    }
    public bool IsFried() {
        return state==State.Fried;
    }
}
