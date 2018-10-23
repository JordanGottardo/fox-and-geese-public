using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

	public int numberOfGeese = 13;
	public Slider slider;

	public const string confirmNumberOfGeeseNotification = "Menu.ConfirmNumberOfGeeseNotification";
	
	/** Called whenever the user changes the number on the slide */
	public void OnSliderChange() {
		int num = (int)slider.value;
		numberOfGeese = 13 + num * 2;
	}

	/** Called when the user confirms the number of geese */
	public void ConfirmNumberOfGeese() {
		this.PostNotification(confirmNumberOfGeeseNotification, numberOfGeese);
	}

}
