using UnityEngine;
using UnityEngine.Purchasing;

namespace OpenIAP {
	/// <summary>
	/// Extension methods for Unity IAP Product and purchasing utilities.
	/// </summary>
	public static class IAPExtensions {

		/// <summary>
		/// Gets the price of the product as a string.
		/// </summary>
		/// <param name="product">Product to extract price from.</param>
		/// <param name="localize">If true, returns localized price string (e.g. "$1.99"), otherwise raw decimal string (e.g. "1.99").</param>
		/// <returns>Formatted price string.</returns>
		public static string GetPrice(this Product product, bool localize = true) {
			return localize
				? $"{(Application.isEditor ? "$" : "")}{product.metadata.localizedPriceString}"
				: product.metadata.localizedPrice.ToString();
		}

		/// <summary>
		/// Attempts to restore previously purchased non-consumable products (iOS only).
		/// </summary>
		public static void RestorePurchases() {
#if UNITY_IOS || UNITY_IPHONE
			var extensions = CodelessIAPStoreListener.Instance.GetStoreExtensions<IAppleExtensions>();
			extensions.RestoreTransactions((result, error) => {
				if (result) {
					Debug.Log("Purchases restoration completed. This does not mean anything was restored, merely that the restoration process succeeded.");
				} else {
					Debug.LogError(error);
				}
			});
#endif
		}
	}
}