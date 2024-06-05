using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour,IKitchenObjectParent
{
    public event EventHandler OnPickedSomething;

    [SerializeField] private float MoveSpeed;//SerializeField使得该变量可以在unity监视器中编辑，即使这个变量是私有的
    [SerializeField] private float RotationSpeed;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private Transform KitchenObjectHoldPoint;
    public bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    public event EventHandler <OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    //委托所携带的事件参数类型为OnSelectedCounterChangedEventArgs

    //在 C# 中，通常在定义事件时，会使用 EventArgs 类或其派生类作为事件参数的类型。
    //EventArgs 类本身并不包含任何特定的事件数据，它只是一个空的基类，用于表示事件参数。
    //开发人员可以通过继承 EventArgs 类来定义自己的事件参数类型，并添加需要的字段或属性来携带事件相关的数据
    public class OnSelectedCounterChangedEventArgs : EventArgs//将这个类作为委托的类型
    {
        public BaseCounter selectedCounter;//让其他脚本知道SelectedCounter
    }


    private static Player instance;
    public static Player Instance
    {
        get { return instance; }

        private set { instance = value; }
    }

    private void Awake()
    {

        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }
    void Start()
    {
        GameInput.instance.OnInteractAction += GameInput_OnInterAction;
        GameInput.instance.OnInteracAlternatetAction += GameInput_OnInterAlternateAction;
    }
    public bool IsWakling()
    {
        return isWalking;
    }
    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }
    private void GameInput_OnInterAlternateAction(object sender, System.EventArgs e)
    {
        if (!KitchenGameManager.instance.IsGamePlaying()) return;

        if (selectedCounter != null) {
            selectedCounter.InteractAlternate(this);
        }
    }
    private void GameInput_OnInterAction(object sender,System.EventArgs e)
    {
        if (!KitchenGameManager.instance.IsGamePlaying()) return;

        if (selectedCounter != null) {
            selectedCounter.Interact(this);
        }
    }
    private void HandleInteractions()
    {
        Vector2 inputVector = GameInput.instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, counterLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out BaseCounter basecounter)) {
                //Has ClearCounter
                if (basecounter != selectedCounter) {
                    SetSelectedCounter(basecounter);
                }
            } else {
                SetSelectedCounter(null);
            }
        } else {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = MoveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        float playerHeight = 2f;

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);
        //光线长度等于玩家与碰撞体的距离
        //如果发出的射线被碰撞体阻挡，则返回true
        //线光线由于很薄，导致还是可能导致穿模
        //所以采用其他形状的射线，例如capsule或box

        if (!canMove) {
            //Cannot move towards moveDir

            //Attempt only X movement

            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            //moveDir.x!=0
            canMove =(moveDir.x<-0.5f||moveDir.x>+0.5f)&&!Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
            if (canMove) {
                // Can move only on the X
                moveDir = moveDirX;
            }
            else {
                // Cannot move only on the x

                // Attempt only Z movement 
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                //moveDir.z!=0
                canMove = (moveDir.z < -0.5f || moveDir.z > +0.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
                if (canMove) {
                    // Can move only on the z
                    moveDir = moveDirZ;
                }
                else {
                    //Can not move in any direction
                }
            }
        }

        if (canMove) {
            this.transform.position += moveDir * moveDistance;
        }


        this.transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * RotationSpeed);

        isWalking = (moveDir != Vector3.zero);
    }
  

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;//【1】把射线返回的selectedCounter给本Player
        //这里问号的作用就是检查委托是否有订阅者，没有则不执行后续的代码，使得代码更简洁
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        //【2】将Player里的selectedCounter传给订阅者
        //Invoke函数将订阅者和委托联动，第一个参数是参数的发送者，即Player实例，并指定其他参数，这里是一个类实例
        {
            selectedCounter = selectedCounter//前者是OnSelectedCounterChangedEventArgs类中的，ClearCounter参数
        });
    }
    public Transform GetKitchenObjectFollowTransform()
    {
        return KitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null) {
            OnPickedSomething?.Invoke(this,EventArgs.Empty);
        }
    }
    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }
    public void ClearKitchenObject()
    {
        this.kitchenObject = null;
    }
    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
