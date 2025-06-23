using UnityEngine;
using UnityEngine.Purchasing;

namespace OpenIAP {
	public static class IAPExtensions {
		public static string GetPrice(this Product product) {
			return $"{(Application.isEditor ? "$" : "")}{product.metadata.localizedPriceString}";
		}

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