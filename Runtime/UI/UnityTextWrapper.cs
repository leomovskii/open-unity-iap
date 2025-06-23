using UnityEngine;
using UnityEngine.UI;

namespace OpenIAP {
	public class UnityTextWrapper : ITextComponent {

		[SerializeField] private Text _text;

		public string Text {
			get => _text.text;
			set => _text.text = value;
		}
	}
}