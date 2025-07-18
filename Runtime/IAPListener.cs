using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace OpenIAP {
	public class IAPListener : MonoBehaviour {
		/// <summary>
		/// Type of event fired after a successful fetching the products from the store.
		/// </summary>
		[System.Serializable]
		public class OnProductsFetchedEvent : UnityEvent<ProductCollection> {
		};

		/// <summary>
		/// Type of event fired after a successful purchase of a product.
		/// </summary>
		[System.Serializable]
		public class OnPurchaseCompletedEvent : UnityEvent<Product> {
		};

		/// <summary>
		/// Type of event fired after a failed purchase of a product.
		/// </summary>
		[System.Serializable]
		public class OnPurchaseFailedEvent : UnityEvent<Product, PurchaseFailureReason> {
		};

		/// <summary>
		/// Type of event fired after a failed purchase of a product.
		/// </summary>
		[System.Serializable]
		public class OnPurchaseDetailedFailedEvent : UnityEvent<Product, PurchaseFailureDescription> {
		};

		/// <summary>
		/// Consume successful purchases immediately.
		/// </summary>
		[Tooltip("Consume successful purchases immediately.")]
		public bool consumePurchase = true;

		/// <summary>
		/// Preserve this GameObject when a new scene is loaded.
		/// </summary>
		[Tooltip("Preserve this GameObject when a new scene is loaded.")]
		public bool dontDestroyOnLoad = true;

		/// <summary>
		/// Event fired after a successful fetching the products from the store.
		/// </summary>
		[Tooltip("Event fired after a successful fetching the products from the store.")]
		public OnProductsFetchedEvent onProductsFetched;

		/// <summary>
		/// Event fired after a successful purchase of this product.
		/// </summary>
		[Tooltip("Event fired after a successful purchase of this product.")]
		public OnPurchaseCompletedEvent onPurchaseComplete;

		/// <summary>
		/// Event fired after a failed purchase of this product.
		/// </summary>
		[Tooltip("Event fired after a failed purchase of this product.")]
		public OnPurchaseFailedEvent onPurchaseFailed;

		/// <summary>
		/// Event fired after a failed purchase of this product.
		/// </summary>
		[Tooltip("Event fired after a failed purchase of this product.")]
		public OnPurchaseDetailedFailedEvent onPurchaseDetailedFailedEvent;

		void OnEnable() {
			if (dontDestroyOnLoad) {
				DontDestroyOnLoad(gameObject);
			}

			IAPController.Instance.AddListener(this);
		}

		void OnDisable() {
			IAPController.Instance.RemoveListener(this);
		}

		/// <summary>
		/// Invoked to process a successful purchase of the product associated with this button
		/// </summary>
		/// <param name="e">The successful <c>PurchaseEventArgs</c> for the purchase event. </param>
		/// <returns>The result of the successful purchase</returns>
		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e) {
			onPurchaseComplete.Invoke(e.purchasedProduct);

			return consumePurchase ? PurchaseProcessingResult.Complete : PurchaseProcessingResult.Pending;
		}

		/// <summary>
		/// Invoked on a failed purchase of the product associated with this button
		/// </summary>
		/// <param name="product">The <typeparamref name="Product"/> which failed to purchase</param>
		/// <param name="reason">Information to help developers recover from this failure</param>
		public void OnPurchaseFailed(Product product, PurchaseFailureReason reason) {
			onPurchaseFailed.Invoke(product, reason);
		}

		/// <summary>
		/// Invoked on a failed purchase a product
		/// </summary>
		/// <param name="product">The <typeparamref name="Product"/> which failed to purchase</param>
		/// <param name="description">Information to help developers recover from this failure</param>
		public void OnPurchaseFailed(Product product, PurchaseFailureDescription description) {
			onPurchaseDetailedFailedEvent.Invoke(product, description);
		}
	}
}