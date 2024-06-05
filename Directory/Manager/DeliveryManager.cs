using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpwaned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList;

    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax=4f;
    private int waitingRecipesMax = 4;
    private int successfulRecipesAmount;

    private void Awake() {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update() {
        spawnRecipeTimer -= Time.deltaTime;
        //首先会生成一个然后每四秒生成一次
        if (spawnRecipeTimer < 0f ) {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (KitchenGameManager.instance.IsGamePlaying()&&waitingRecipeSOList.Count < waitingRecipesMax) {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
                //Debug.Log(waitingRecipeSO.recipeName);

                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpwaned?.Invoke(this,EventArgs.Empty);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        for (int i = 0;i< waitingRecipeSOList.Count; i++) {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if(waitingRecipeSO.kitchenObjectSOList.Count==plateKitchenObject.GetKitchenObjectSOList().Count) {
                // Has the same number of in gredients
                bool plateContentsMatchesRecipe = true;
                foreach(KitchenObjectSo recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) {
                    // Cycling through all ingredients in the recipe

                    bool ingredientFound = false;
                    foreach (KitchenObjectSo plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
                        // Cycling through all ingredients in the Plate
                        if (plateKitchenObjectSO == recipeKitchenObjectSO) {
                            // Ingredient matches!
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound ) {
                        // this Recipe ingredient was not found on the Plate
                        plateContentsMatchesRecipe = false;
                    }
                }
                if(plateContentsMatchesRecipe) {
                    // Player delivered the correct recipe!
                    Debug.Log("Player delivered the correct recipe!");
                    successfulRecipesAmount++;

                    waitingRecipeSOList.RemoveAt(i);

                    OnRecipeCompleted?.Invoke(this,EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this,EventArgs.Empty);

                    return;
                } 
            }
        }
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
        //Debug.Log("Player delivered the wrong recipe!");
        //no matchees found
        //Player did not deliever the correct recipe!
    }
    public List<RecipeSO> GetWaitingRecipeSOList() {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount() {
        return successfulRecipesAmount;
    }
}
