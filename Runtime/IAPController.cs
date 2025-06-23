using System.Collections.Generic;
using System.Linq;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing;

namespace OpenIAP {
	public class IAPController : IDetailedStoreListener {

		private static IAPController _instance;

		public static IAPController Instance {
			get {
				if (_instance == null)
					InternalInitialize();
				return _instance;
			}
		}

		public static void Initialize() {
			if (_instance == null)
				InternalInitialize();
		}

		private static async void InternalInitialize() {
			Debug.Log("[IAP] Initialization start.");
			_instance = new IAPController();

			if (UnityServices.State is ServicesInitializationState.Uninitialized)
				await UnityServices.InitializeAsync();

			var module = StandardPurchasingModule.Instance();
			module.useFakeStoreUIMode = FakeStoreUIMode.Default;

			var builder = ConfigurationBuilder.Instance(module);

			IAPConfigurationHelper.PopulateConfigurationBuilder(ref builder, Catalog);
			Builder = builder;

			UnityPurchasing.Initialize(Instance, builder);
			Debug.Log("[IAP] Initialization complete.");
		}

		private readonly List<IAPItem> _codeless = new List<IAPItem>();
		private readonly List<IAPListener> _listeners = new List<IAPListener>();

		public static IStoreController Controller { get; private set; }
		public static IExtensionProvider Extensions { get; private set; }
		public static ConfigurationBuilder Builder { get; private set; }
		public static ProductCatalog Catalog { get; private set; }

		public static bool Initialized { get; private set; }

		private IAPController() {
			Catalog = ProductCatalog.LoadDefaultCatalog();
		}

		public T GetStoreConfiguration<T>() where T : IStoreConfiguration {
			return Builder.Configure<T>();
		}

		public T GetStoreExtensions<T>() where T : IStoreExtension {
			return Extensions.GetExtension<T>();
		}

		public static bool HasProductInCatalog(string productID) {
			if (Catalog == null || string.IsNullOrWhiteSpace(productID))
				return false;

			foreach (var product in Catalog.allProducts)
				if (product.id == productID)
					return true;

			return false;
		}

		public static bool TryGetProduct(string productID, out Product product) {
			if (Instance == null || Controller == null || Controller.products == null) {
				Debug.LogError("[IAP] Unable to find product. Purchasing not initialized correctly.");
				product = null;

			} else if (string.IsNullOrWhiteSpace(productID)) {
				Debug.LogError("[IAP] Unable to find product. ProductId is empty.");
				product = null;

			} else
				product = Controller.products.WithID(productID);

			return product != null;
		}

		public void AddButton(IAPItem button) {
			_codeless.Add(button);
		}

		public void RemoveButton(IAPItem button) {
			_codeless.Remove(button);
		}

		public void AddListener(IAPListener listener) {
			_listeners.Add(listener);
		}

		public void RemoveListener(IAPListener listener) {
			_listeners.Remove(listener);
		}

		public void InitiatePurchase(string productID) {
			if (Controller == null) {
				Debug.LogError("Purchase failed because Purchasing was not initialized correctly");

				SendPurchaseFailedEventsToAllButtons(productID);

				return;
			}

			Controller.InitiatePurchase(productID);
		}

		void SendPurchaseFailedEventsToAllButtons(string productID) {
			foreach (var button in _codeless.Where(button => button.productId == productID)) {
				var purchaseFailureDescription = new PurchaseFailureDescription(productID, PurchaseFailureReason.PurchasingUnavailable, "PurchasingUnavailable");
				button.OnPurchaseFailed(null, purchaseFailureDescription);
			}
		}

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
			Initialized = true;
			Controller = controller;
			Extensions = extensions;

			foreach (var iapListener in _listeners)
				iapListener.onProductsFetched.Invoke(controller.products);

			HandleOnInitForAllButtons();
		}

		void HandleOnInitForAllButtons() {
			foreach (var button in _codeless) {
				button.OnInitCompleted();
			}
		}

		public void OnInitializeFailed(InitializationFailureReason error) {
			OnInitializeFailed(error, null);
		}

		public void OnInitializeFailed(InitializationFailureReason error, string message) {
			var errorMessage = $"Purchasing failed to initialize. Reason: {error.ToString()}.";

			if (message != null) {
				errorMessage += $" More details: {message}";
			}

			Debug.LogError(errorMessage);
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e) {
			PurchaseProcessingResult result;

			var consumePurchase = false;
			var resultProcessed = false;

			foreach (var button in _codeless.Where(button => button.productId == e.purchasedProduct.definition.id)) {
				result = button.ProcessPurchase(e);

				if (result == PurchaseProcessingResult.Complete) {
					consumePurchase = true;
				}

				resultProcessed = true;
			}

			foreach (var listener in _listeners) {
				result = listener.ProcessPurchase(e);

				if (result == PurchaseProcessingResult.Complete) {
					consumePurchase = true;
				}

				resultProcessed = true;
			}

			if (!resultProcessed) {
				Debug.LogError("Purchase not correctly processed for product \"" +
					e.purchasedProduct.definition.id +
					"\". Add an active IAPButton to process this purchase, or add an IAPListener to receive any unhandled purchase events.");
			}

			return consumePurchase ? PurchaseProcessingResult.Complete : PurchaseProcessingResult.Pending;
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureReason reason) {
			var resultProcessed = false;

			if (_codeless.Exists(button => button.productId == product.definition.id)) {
				resultProcessed = true;
			}

			foreach (var listener in _listeners) {
				listener.OnPurchaseFailed(product, reason);

				resultProcessed = true;
			}

			if (!resultProcessed) {
				Debug.LogError("Failed purchase not correctly handled for product \"" + product.definition.id +
					"\". Add an active IAPButton to handle this failure, or add an IAPListener to receive any unhandled purchase failures.");
			}
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureDescription description) {
			OnPurchaseFailed(product, description.reason);

			foreach (var button in _codeless.Where(button => button.productId == product.definition.id)) {
				button.OnPurchaseFailed(product, description);
			}

			foreach (var listener in _listeners) {
				listener.OnPurchaseFailed(product, description);
			}
		}
	}
}