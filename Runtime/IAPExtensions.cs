using System.Collections.Generic;
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

		public static void PopulateConfigurationBuilder(ref ConfigurationBuilder builder, ProductCatalog catalog) {
			foreach (var product in catalog.allValidProducts) {
				IDs ids = null;

				if (product.allStoreIDs.Count > 0) {
					ids = new IDs();
					foreach (var storeID in product.allStoreIDs) {
						ids.Add(storeID.id, storeID.store);
					}
				}

#if UNITY_2017_2_OR_NEWER

				var payoutDefinitions = new List<PayoutDefinition>();
				foreach (var payout in product.Payouts) {
					payoutDefinitions.Add(new PayoutDefinition(payout.typeString, payout.subtype, payout.quantity, payout.data));
				}
				builder.AddProduct(product.id, product.type, ids, payoutDefinitions.ToArray());

#else

                builder.AddProduct(product.id, product.type, ids);

#endif
			}
		}
	}
}