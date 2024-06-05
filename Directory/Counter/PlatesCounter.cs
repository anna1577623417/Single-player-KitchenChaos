using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter 
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemove;

    [SerializeField] private KitchenObjectSo palateKitchenObjectSO;

    private float spawnPlateTimer;
    private float spawnPlateTimeMax=4f;
    private int plateSpawnedAmount;
    private int plateSpawnedAmountMAX = 4;

    private void Update() {
        spawnPlateTimer += Time.deltaTime;
        if(KitchenGameManager.instance.IsGamePlaying() && spawnPlateTimer > spawnPlateTimeMax ) {
            spawnPlateTimer = 0;
            if (plateSpawnedAmount < plateSpawnedAmountMAX) {
                plateSpawnedAmount++;
                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void Interact(Player player) {
        if(!player.HasKitchenObject()) {

            if(plateSpawnedAmount>0) {
                plateSpawnedAmount--;
                
                KitchenObject.SpwanKitchenObject(palateKitchenObjectSO, player);

                OnPlateRemove?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
