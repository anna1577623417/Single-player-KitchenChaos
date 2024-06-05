using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour,IKitchenObjectParent
{
    public event EventHandler OnPickedSomething;

    [SerializeField] private float MoveSpeed;//SerializeFieldʹ�øñ���������unity�������б༭����ʹ���������˽�е�
    [SerializeField] private float RotationSpeed;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private Transform KitchenObjectHoldPoint;
    public bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    public event EventHandler <OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    //ί����Я�����¼���������ΪOnSelectedCounterChangedEventArgs

    //�� C# �У�ͨ���ڶ����¼�ʱ����ʹ�� EventArgs �������������Ϊ�¼����������͡�
    //EventArgs �౾���������κ��ض����¼����ݣ���ֻ��һ���յĻ��࣬���ڱ�ʾ�¼�������
    //������Ա����ͨ���̳� EventArgs ���������Լ����¼��������ͣ��������Ҫ���ֶλ�������Я���¼���ص�����
    public class OnSelectedCounterChangedEventArgs : EventArgs//���������Ϊί�е�����
    {
        public BaseCounter selectedCounter;//�������ű�֪��SelectedCounter
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
        //���߳��ȵ����������ײ��ľ���
        //������������߱���ײ���赲���򷵻�true
        //�߹������ںܱ������»��ǿ��ܵ��´�ģ
        //���Բ���������״�����ߣ�����capsule��box

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
        this.selectedCounter = selectedCounter;//��1�������߷��ص�selectedCounter����Player
        //�����ʺŵ����þ��Ǽ��ί���Ƿ��ж����ߣ�û����ִ�к����Ĵ��룬ʹ�ô�������
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        //��2����Player���selectedCounter����������
        //Invoke�����������ߺ�ί����������һ�������ǲ����ķ����ߣ���Playerʵ������ָ������������������һ����ʵ��
        {
            selectedCounter = selectedCounter//ǰ����OnSelectedCounterChangedEventArgs���еģ�ClearCounter����
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
