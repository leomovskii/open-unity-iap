using UnityEngine.Purchasing;
using UnityEngine;
using UnityEngine.UI;

namespace OpenIAP {
	public abstract class IAPButtonBase : MonoBehaviour {

		public bool autoUnsubscribe;

		protected abstract void OnTransactionsRestored(bool success, string error);
		protected abstract void OnPurchaseComplete(Product purchasedProduct);

		internal abstract void OnInitCompleted();
		protected abstract void AddButtonToCodelessListener();
		protected abstract void RemoveButtonToCodelessListener();
		protected abstract Button GetPurchaseButton();

		private bool _subscribed;

		void Start() {
			var button = GetPurchaseButton();
			var productId = GetProductId();

			if (IsAPurchaseButton()) {
				if (button) {
					button.onClick.AddListener(PurchaseProduct);

					if (IAPController.TryGetProduct(productId, out var product))
						PurchaseButtonInitializedAction(product);
				}

				if (string.IsNullOrEmpty(productId)) {
					Debug.LogError("IAPButton productId is empty");
				} else if (!IAPController.HasProductInCatalog(productId!)) {
					Debug.LogWarning("The product catalog has no product with the ID \"" + productId + "\"");
				}
			} else if (IsARestoreButton()) {
				if (button) {
					button.onClick.AddListener(Restore);
				}
			}
		}

		protected virtual void PurchaseButtonInitializedAction(Product product) { }

		public abstract string GetProductId();
		public abstract bool IsAPurchaseButton();
		public abstract bool IsARestoreButton();

		void OnEnable() {
			if (_subscribed)
				return;

			if (IsAPurchaseButton()) {
				AddButtonToCodelessListener();
				if (IAPController.Initialized)
					OnInitCompleted();
				_subscribed = true;
			}
		}

		void OnDisable() {
			if (_subscribed && autoUnsubscribe && IsAPurchaseButton()) {
				RemoveButtonToCodelessListener();
				_subscribed = false;
			}
		}

		void PurchaseProduct() {
			if (IsAPurchaseButton()) {
				IAPController.Instance.InitiatePurchase(GetProductId());
			}
		}

		protected PurchaseProcessingResult ProcessPurchaseInternal(PurchaseEventArgs args) {
			OnPurchaseComplete(args.purchasedProduct);

			return PurchaseProcessingResult.Complete;
		}

		void Restore() {
			if (IsARestoreButton()) {
				if (Application.platform == RuntimePlatform.WSAPlayerX86 ||
					Application.platform == RuntimePlatform.WSAPlayerX64 ||
					Application.platform == RuntimePlatform.WSAPlayerARM) {
					IAPController.Instance.GetStoreExtensions<IMicrosoftExtensions>()
						.RestoreTransactions();
				} else if (Application.platform == RuntimePlatform.IPhonePlayer ||
						   Application.platform == RuntimePlatform.OSXPlayer ||
						   Application.platform == RuntimePlatform.tvOS
#if UNITY_VISIONOS
                         || Application.platform == RuntimePlatform.VisionOS
#endif
						   ) {
					IAPController.Instance.GetStoreExtensions<IAppleExtensions>()
						.RestoreTransactions(OnTransactionsRestored);
				} else if (Application.platform == RuntimePlatform.Android &&
						   StandardPurchasingModule.Instance().appStore == AppStore.GooglePlay) {
					IAPController.Instance.GetStoreExtensions<IGooglePlayStoreExtensions>()
						.RestoreTransactions(OnTransactionsRestored);
				} else {
					Debug.LogWarning(Application.platform +
						" is not a supported platform for the Codeless IAP restore button");
				}
			}
		}
	}
}