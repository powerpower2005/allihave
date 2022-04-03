using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


//IStoreLinster는 구매와 관련된 이벤트를 관리함.
public class IAPManager : MonoBehaviour, IStoreListener
{

    private IStoreController controller;
    private IExtensionProvider extensions;


    //하나만 존재함.
    public void Awake()
    {
        //싱글톤으로 관리
        if (!G.Manager.server.IsLoaded) return;
        if (G.Manager.iap != null)
        {
            Destroy(gameObject);
            return;
        }
        G.Manager.iap = this;
        DontDestroyOnLoad(gameObject);


    }

    void Start()
    {
        //초기화 되었으면 다시 실행하지 않음.(Reload)같은 경우
        if (IsInitialized()) return;
        Debug.Log($"<color=#ff0>Try Initialize IAP</color>");
        InitializePurchasing();

        productData =  //TODO :: 등록할 상품을 받아옴.


    }



    #region Purchasing 초기화
    //1. 초기화 메서드 -> 모든 것에 앞서서 실행되어야 함.
    public void InitializePurchasing()
    {
        //이 빌더는 제품 정의를 위한 필수
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());


        //데이터 -> 스토어에 따른 정보를 가지고 있음.
        var data = //TODO::

        //제품 정의 등록
        for (int i = 0; i < data.Length; i++)
        {
            //제품을 추가할 때 앱스토어 또는 구글스토어에 대한 id를 같이 넣어줌
            builder.AddProduct(data[i].key.ToString(), ProductType.Consumable); // -> 스토어별 ID 넣어줘야함.
        }

        //진행
        UnityPurchasing.Initialize(this, builder);
    }



    //2-1. 초기화가 완료되었을 때 실행됨
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        this.extensions = extensions;
        Debug.Log($"<color=#ff0>Success to Initialize Puchasing Information</color>");


        var allItems = GetProducts();
        foreach(var item in allItems)
        {
            Debug.Log($"<color=#ff0>{item.definition.id}</color>");
            
        }
    }


    //2-2. 초기화가 실패되었을 때 실행됨
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"<color=#ff0>Fail to Initialize Purchasing Information</color>");
    }

    private bool IsInitialized()
    {
        //둘다 null이 아니라면 초기화 정상적으로 된 것임
        return controller != null && extensions != null;
    }

    #endregion


    #region 구매


    // 1. 구매버튼을 클릭한다면 구매하는 함수 실행
    // (string : productId) -> 상품 이름 필요
    //#1. 이 함수를 실행하는 클라이언트 작업.
    public void StartPurchase(string productId)
    {
        if (!IsInitialized())
        {
            Debug.Log("초기화 되지 않았음");
            return; //초기화되었을 때만 살 수 있음.
        }

        Debug.Log($"<color=#ff0>Try to purchase {productId}</color>");
        controller.InitiatePurchase(productId); //Unity IAP #2. Initiate Purchase
        //id가 아닌 Product 클래스로 실행할 수 있음.
    }


  
    //2. 실제로 구매 시도 -> 초기화 이후에 실행되어야 함.
    //#3. Purchase Succeeded 이후 #4.ProcessPurchase
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var id = purchaseEvent.purchasedProduct.definition.id;

        foreach(var data in productData)
        {
            if(data.key.ToString() == id)
            {
                keyProduct = data.key;
                break;
            }
        }
        var server = G.Manager.server;

        //구매처리
        Debug.Log($"<color=#ff0>Try to purchase {keyProduct.ToString()}</color>");

        var storeKey = StoreKey.GooglePlay;
#if UNITY_EDITOR
        storeKey = StoreKey.FakeStore;
#elif UNITY_ANDROID
        storeKey = StoreKey.GooglePlay;
#else
        storeKey = StoreKey.AppleAppStore;
#endif


        //#6. 비동기로 영수증을 서버에 보냄
        G.Manager.reqManager.Enqueue(server.PurchaseInApp(purchaseEvent.purchasedProduct.definition.id, storeKey, purchaseEvent.purchasedProduct.receipt))
            .GetAwaiter()
            .OnCompleted(() =>
        {
            Debug.Log($"<color=#ff0> Get ACK Sign from Server </color>");
            //#7. 서버로 영수증이 전달 되었다면 Pending을 완료해줌. 
            ConfirmPendingPurchase(purchaseEvent.purchasedProduct, keyProduct);
        });
        Debug.Log($"<color=#ff0>Sending Receipt to Server</color>");

        //Pending을 걸고 서버에서 작업을 할 것임 -> InGameMail을 통해 지급.
        //#5. PurchaseProcessingResult.Pending
        return PurchaseProcessingResult.Pending;
    }




    //#8.ConfirmPendingPurchase
    public void ConfirmPendingPurchase(Product pendedProduct, ProductKey pendedKey)
    {
        Debug.Log($"<color=#ff0> Purchase was pended is processed</color>");

        controller.ConfirmPendingPurchase(pendedProduct);

        //#9 아이템 지급 - Finish Transaction -> 서버에 메일로 요청함
        G.Manager.reqManager.Enqueue(G.Manager.server.PurchaseInAppReward()).Forget();
        //TODO:: 메일함 업데이트


    }

    //3. 구매가 실패했을 때 실행됨. (구매 취소도 실패임)
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"{product.definition.id} 구매과정에서 {failureReason} 발생");
    }

    // *. 구매 되돌리기
    public void RestorePurchases()
    {
        //초기화 이후 가능.
        if (!IsInitialized()) return;


        // 앱스토어에 대한 작동.
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            
            Debug.Log("RestorePurchases started ...");

            // 스토어 서브 시스템에 접근.
            var apple = extensions.GetExtension<IAppleExtensions>();


            // 비동기작업으로 진행
            // result에 대해서 비동기 작업이 완료되면 실행될 콜백함수 (무명메서드로 동작해도 괜찮음) -> 여기서 구입한 항목 되돌리는데 쓰임.
            apple.RestoreTransactions((result) => {
                // The first phase of restoration.
                //If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        //다른 플랫폼에서 진행한다면 Restore는 진행되지 않음.
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

#endregion


#region 상품 정보

    //특정 상품(ID)
    public Product GetProduct(string productID)
    {
        return controller.products.WithID(productID);
    }

    //Initialize한 모든 상품
    public Product[] GetProducts()
    {
        return controller.products.all;
    }

    //현지 가격 구하기
    public string GetLocalPrice() {
        //현지 가격 + 현지 가격 단위로 변환.
        return $"{GetProduct("특정 ID").metadata.localizedPrice} {GetProduct("특정 ID").metadata.isoCurrencyCode}"; 
        
    }


#endregion
}


