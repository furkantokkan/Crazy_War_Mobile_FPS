using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
public class IAP : MonoBehaviour, IStoreListener
{
    // Items list, configurable via inspector
    // The Unity Purchasing system
    private static IStoreController m_StoreController;

    // Bootstrap the whole thing
    private static IAP instance;

    public static IAP Instance
    {
        get
        {
            return instance;
        }
        set
        {
            instance = value;
        }
    }

    //Your products IDs. They should match the ids of your products in your store.

    private string[] productID = new string[] { "unlimited.bullet" };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public string environment = "production";

    async void Start()
    {
        try
        {
            var options = new InitializationOptions()
                .SetEnvironmentName(environment);

            await UnityServices.InitializeAsync(options);
        }
        catch (Exception exception)
        {
            // An error occurred during services initialization.
        }

        StartCoroutine(InitializePurchasing());
    }

    // This is invoked manually on Start to initialize UnityIAP
    public IEnumerator InitializePurchasing()
    {
        // If IAP is already initialized, return gently
        if (IsInitialized) yield break;
        // Create a builder for IAP service
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.GooglePlay));

        // Register each item from the catalog
        //foreach (string gold in goldProductID)
        //{
        //    builder.AddProduct(gold, ProductType.Consumable);
        //    yield return null;
        //}
        foreach (string diamond in productID)
        {
            builder.AddProduct(diamond, ProductType.Consumable);
            yield return null;
        }

        // Trigger IAP service initialization
        UnityPurchasing.Initialize(this, builder);
    }

    // We are initialized when StoreController and Extensions are set and we are logged in
    public bool IsInitialized
    {
        get
        {
            return m_StoreController != null;
        }
    }

    // This is automatically invoked automatically when IAP service is initialized
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;
    }

    // This is automatically invoked automatically when IAP service failed to initialized
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    // This is automatically invoked automatically when purchase failed
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

    // This is invoked automatically when successful purchase is ready to be processed
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        // NOTE: this code does not account for purchases that were pending and are
        // delivered on application start.
        // Production code should account for such case:
        // More: https://docs.unity3d.com/ScriptReference/Purchasing.PurchaseProcessingResult.Pending.html

        if (!IsInitialized)
        {
            return PurchaseProcessingResult.Complete;
        }

        // Test edge case where product is unknown
        if (e.purchasedProduct == null)
        {
            Debug.LogWarning("Attempted to process purchase with unknown product. Ignoring");
            return PurchaseProcessingResult.Complete;
        }

        // Test edge case where purchase has no receipt
        if (string.IsNullOrEmpty(e.purchasedProduct.receipt))
        {
            Debug.LogWarning("Attempted to process purchase with no receipt: ignoring");
            return PurchaseProcessingResult.Complete;
        }

        Debug.Log("Processing transaction: " + e.purchasedProduct.transactionID);

        // Deserialize receipt
        var googleReceipt = GooglePurchase.FromJson(e.purchasedProduct.receipt);

        // Invoke receipt validation
        // This will not only validate a receipt, but will also grant player corresponding items
        // only if receipt is valid.
        ProcessResult(e);
        return PurchaseProcessingResult.Complete;
    }
    private void ProcessResult(PurchaseEventArgs args)
    {
        PlayerPrefs.SetInt("Ammo", 1);
        GameManager.unlimitedAmmo = true;

        //var product = args.purchasedProduct;

        //List<string> productList = new List<string>();
        //productList.AddRange(goldProductID);
        //productList.AddRange(diamondProductID);

        //Add the purchased product to the players inventory
        //foreach (string item in productList)
        //{
        //    if (product.definition.id == item)
        //    {
        //        string productID = item.Substring(item.Length - 2);
        //        string value = productID.Substring(0, 1);
        //        string Index = productID.Substring(1, 1);

        //        //Gold
        //        if (value == "g")
        //        {
        //            print("Gold Sended");
        //        }
        //        //Diamond
        //        else if (value == "d")
        //        {
        //            print("Diamond Sended");
        //        }
        //    }
        //}

        Debug.Log("Validation successful!");
    }

    // This is invoked manually to initiate purchase
    public void BuyProductID(string productId)
    {
        // If IAP service has not been initialized, fail hard
        if (!IsInitialized) throw new Exception("IAP Service is not initialized!");
        // Pass in the product id to initiate purchase
        m_StoreController.InitiatePurchase(productId);
    }
}

// The following classes are used to deserialize JSON results provided by IAP Service
// Please, note that JSON fields are case-sensitive and should remain fields to support Unity Deserialization via JsonUtilities
public class JsonData
{
    // JSON Fields, ! Case-sensitive

    public string orderId;
    public string packageName;
    public string productId;
    public long purchaseTime;
    public int purchaseState;
    public string purchaseToken;
}

public class PayloadData
{
    public JsonData JsonData;

    // JSON Fields, ! Case-sensitive
    public string signature;
    public string json;

    public static PayloadData FromJson(string json)
    {
        var payload = JsonUtility.FromJson<PayloadData>(json);
        payload.JsonData = JsonUtility.FromJson<JsonData>(payload.json);
        return payload;
    }
}

public class GooglePurchase
{
    public PayloadData PayloadData;

    // JSON Fields, ! Case-sensitive
    public string Store;
    public string TransactionID;
    public string Payload;

    public static GooglePurchase FromJson(string json)
    {
        var purchase = JsonUtility.FromJson<GooglePurchase>(json);
        purchase.PayloadData = PayloadData.FromJson(purchase.Payload);
        return purchase;
    }
}