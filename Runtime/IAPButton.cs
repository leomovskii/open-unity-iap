using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace OpenIAP {
	public class IAPButton : IAPButtonBase {
		/// <summary>
		/// Type of event fired after a successful fetching the product information from the store.
		/// </summary>
		[Serializable]
		public class OnProductFetchedEvent : UnityEvent<Product> {
		}

		/// <summary>
		/// Type of event fired after a successful purchase of a product.
		/// </summary>
		[Serializable]
		public class OnPurchaseCompletedEvent : UnityEvent<Product> {
		}

		/// <summary>
		/// Type of event fired after a failed purchase of a product.
		/// </summary>
		[Serializable]
		public class OnPurchaseFailedEvent : UnityEvent<Product, PurchaseFailureDescription> {
		}

		/// <summary>
		/// Type of event fired after a restore transactions was completed.
		/// </summary>
		[Serializable]
		public class OnTransactionsRestoredEvent : UnityEvent<bool, string> {
		}

		/// <summary>
		/// Which product identifier to represent. Note this is not a store-specific identifier.
		/// </summary>
		[HideInInspector]
		public string productId;

		public MonoBehaviour priceText;

		/// <summary>
		/// The type of this button, can be either a purchase or a restore button.
		/// </summary>
		[Tooltip("The type of this button, can be either a purchase or a restore button.")]
		public IAPButtonType buttonType = IAPButtonType.Purchase;

		/// <summary>
		/// Consume the product immediately after a successful purchase.
		/// </summary>
		[Tooltip("Consume the product immediately after a successful purchase.")]
		public bool consumePurchase = true;

		/// <summary>
		/// Event fired after a restore transactions.
		/// </summary>
		[Tooltip("Event fired after a restore transactions.")]
		public OnTransactionsRestoredEvent onTransactionsRestored;

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
		/// Event fired after a successful fetching the product information from the store.
		/// </summary>
		[Tooltip("Event fired after a successful fetching the product information from the store.")]
		public OnProductFetchedEvent onProductFetched;

		[Tooltip("Button that triggers purchase.")]
		public Button button;

		protected override void PurchaseButtonInitializedAction(Product product) {
			base.PurchaseButtonInitializedAction(product);

			if (priceText is not null and ITextComponent textComponent)
				textComponent.Text = product.GetPrice();
		}

		public override string GetProductId() {
			return productId;
		}

		public override bool IsAPurchaseButton() {
			return buttonType == IAPButtonType.Purchase;
		}

		public override bool IsARestoreButton() {
			return buttonType == IAPButtonType.Restore;
		}

		protected override bool ShouldConsumePurchase() {
			return consumePurchase;
		}

		protected override void OnTransactionsRestored(bool success, string error) {
			onTransactionsRestored?.Invoke(success, error);
		}

		protected override void OnPurchaseComplete(Product purchasedProduct) {
			onPurchaseComplete?.Invoke(purchasedProduct);
		}

		/// <summary>
		/// Invoked on a failed purchase of the product associated with this button
		/// </summary>
		/// <param name="product">The <typeparamref name="Product"/> which failed to purchase</param>
		/// <param name="failureDescription">Information to help developers recover from this failure</param>
		public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription) {
			onPurchaseFailed?.Invoke(product, failureDescription);
		}

		protected override Button GetPurchaseButton() {
			return button;
		}

		protected override void AddButtonToCodelessListener() {
			IAPController.Instance.AddButton(this);
		}

		protected override void RemoveButtonToCodelessListener() {
			IAPController.Instance.RemoveButton(this);
		}

		internal override void OnInitCompleted() {
			if (IAPController.TryGetProduct(productId, out var product))
				onProductFetched.Invoke(product);
		}

		/// <summary>
		/// Invoke to process a successful purchase of the product associated with this button.
		/// </summary>
		/// <param name="e">The successful <c>PurchaseEventArgs</c> for the purchase event. </param>
		/// <returns>The result of the successful purchase</returns>
		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
			return ProcessPurchaseInternal(args);
		}
	}
}