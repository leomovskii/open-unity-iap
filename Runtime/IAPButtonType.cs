namespace OpenIAP {
	/// <summary>
	/// The type of a <c>IAPButtonType</c>, can be either a purchase or a restore button.
	/// </summary>
	public enum IAPButtonType {
		/// <summary>
		/// This button will display localized product title and price. Clicking will trigger a purchase.
		/// </summary>
		Purchase,
		/// <summary>
		/// This button will display a static string for restoring previously purchased non-consumable
		/// and subscriptions. Clicking will trigger this restoration process, on supported app stores.
		/// </summary>
		Restore
	}
}
