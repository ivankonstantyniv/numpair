using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener
{
    IStoreController m_StoreController;
    [SerializeField] private GameObject priceValuesGroup;

    private string donate1 = "com.piwpaw.numpair.donate1";
    private string donate2 = "com.piwpaw.numpair.donate2";
    private string donate3 = "com.piwpaw.numpair.donate3";
    private string donate4 = "com.piwpaw.numpair.donate4";
    private string donate5 = "com.piwpaw.numpair.donate5";

    private void Start()
    {
        InitializePurchasing();
    }

    private int CheckForActive()
    {
        int childCount = priceValuesGroup.transform.childCount;
        int activeChild = 0;

        for (int val = 0; val < childCount; val++)
        {
            var child = priceValuesGroup.transform.GetChild(val).gameObject;
            bool isChildActive = child.activeSelf;

            if (isChildActive)
            {
                activeChild = val;
            }
        }

        return activeChild + 1;
    }

    void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(donate1, ProductType.Consumable);
        builder.AddProduct(donate2, ProductType.Consumable);
        builder.AddProduct(donate3, ProductType.Consumable);
        builder.AddProduct(donate4, ProductType.Consumable);
        builder.AddProduct(donate5, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void BuyProduct()
    {
        GlobalSounds.Instance.PlaySound("button");
        string productName = $"com.piwpaw.numpair.donate{CheckForActive().ToString()}";

        m_StoreController.InitiatePurchase(productName);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("In-App Purchasing successfully initialized");
        m_StoreController = controller;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"In-App Purchasing initialize failed: {error}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        var product = args.purchasedProduct;

        Debug.Log("Purchase Completed, Product: " + product.definition.id);

        Global.Instance.CloseDonateWindow();
        Global.Instance.ShowPurchasedWindow();

        return PurchaseProcessingResult.Complete;
    }
}
