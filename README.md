## About

Open Unity API is a plugin that extends the capabilities of the standard Unity IAP, as well as stabilizing its work in some scenarios.

## Installation

- Open Package Manager
- Add package from GIT URL:
  `https://github.com/leomovskii/open-unity-iap.git`

## IAPController

This is a complete replacement for CodelessIAPStoreListener. Its work has been stabilized, and support for legacy buttons has been disabled. The plugin initializes itself on the first call to IAPController.Instance. However, you can do it manually:

```csharp
public void ExampleInit() {
	OpenIAP.IAPController.Initialize();
}
```

You can initialize services yourself if you want, but this is not necessary:

```csharp
public async void ExampleInit() {
	if (UnityServices.State is ServicesInitializationState.Uninitialized)
		await UnityServices.InitializeAsync();
	OpenIAP.IAPController.Initialize();
}
```

If you need to implement initialization in the pipeline, pass the event as an argument

```csharp
public void ExampleInit(Action callback) {
	OpenIAP.IAPController.OnInitializedEvent += callback;
	OpenIAP.IAPController.Initialize();
}
```

You can track the initialization status using:

```csharp
public void ExampleCall() {
	if (OpenIAP.IAPController.Initialized) {
		// do some stuff
	}
}
```

## IAPListener, IAPButtonBase, IAPButton

In general, they work the same way as in CodelessIAPStoreListener, but they are required if used to bypass the standard CodelessIAPStoreListener.

IAPButton additionally has a function that allows you to ignore the button off status and has the ability to display text with the product price.
If you want to use TMP_Text as price display text, add & use script, which extends :

```csharp
using OpenIAP;
using TMPro;
using UnityEngine;

public class TMPTextWrapper : MonoBehaviour, ITextComponent {

	[SerializeField] private TMP_Text _tmpText;
	public string Text {
		get => _tmpText.text;
		set => _tmpText.text = value;
	}
}
```

## IAPExtensions

```csharp
// Localized price of the product
string price = product.GetPrice();

// Manual call to restore purchases
IAPExtensions.RestorePurchases();
```
