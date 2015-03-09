using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// A class for a simple button.
/// </summary>
public class UIButton : MonoBehaviour
{
	public event Action onClick;

	private Button _button;
	private Text _text;

	private bool isActive = true;
	private bool hasText = true;

	public void setText(string text)
	{
		this.text.text = text;
	}

	public void setActive(bool isActive)
	{
		this.isActive = isActive;
		button.interactable = isActive;
	}

	public void onButtonClick()
	{
		if (onClick != null) onClick();
	}

	private Button button
	{
		get
		{
			if (_button == null) _button = GetComponent<Button>();
			return _button;
		}
		
		set
		{
			if (_button == null) _button = GetComponent<Button>();
			_button = value;
		}
	}

	private Text text
	{
		get
		{
			if (_text == null && hasText)
			{
				_text = GetComponentInChildren<Text>();
				if (_text == null) hasText = false;
			}

			return _text;
		}

		set
		{
			if (_text == null && hasText)
			{
				_text = GetComponentInChildren<Text>();
				if (_text == null) hasText = false;
			}

			_text = value;
		}
	}
}