using System;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BigAmbitionsTrainer.UI.Components;

public static class ConfirmationDialog
{
	private static string _title;
	private static string _message;
	private static Action _onConfirm;
	private static Action _onCancel;
	private static bool _active;
	private static float _modalWidth = 500f;
	private static float _modalHeight = 200f;

	public static bool IsActive => _active;

	public static void Show(string title, string message, Action onConfirm, Action onCancel = null)
	{
		_title = title;
		_message = message;
		_onConfirm = onConfirm;
		_onCancel = onCancel;
		_active = true;
	}

	public static void OnGUI()
	{
		if (!_active)
		{
			return;
		}
		try
		{
			float num = ((float)Screen.width - _modalWidth) * 0.5f;
			float num2 = ((float)Screen.height - _modalHeight) * 0.5f;
			GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), GUIContent.none, _GetOverlayStyle());
			Rect rect = new Rect(num, num2, _modalWidth, _modalHeight);
			GUI.Box(rect, GUIContent.none, _GetBgStyle());
			GUIStyle val = new GUIStyle();
			val.fontSize = 18;
			val.alignment = (TextAnchor)4;
			val.normal.textColor = Color.white;
			GUI.Label(new Rect(num + 20f, num2 + 20f, _modalWidth - 40f, 40f), _title, val);
			val.fontSize = 14;
			val.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 1f);
			GUI.Label(new Rect(num + 20f, num2 + 65f, _modalWidth - 40f, 50f), _message, val);
			float num3 = num + (_modalWidth - 240f) * 0.5f;
			float num4 = num2 + _modalHeight - 60f;
			if (GUI.Button(new Rect(num3, num4, 100f, 40f), "Confirm", _GetBtnStyle(true)))
			{
				Action onConfirm = _onConfirm;
				Close();
				onConfirm?.Invoke();
			}
			if (GUI.Button(new Rect(num3 + 140f, num4, 100f, 40f), "Cancel", _GetBtnStyle(false)))
			{
				Action onCancel = _onCancel;
				Close();
				onCancel?.Invoke();
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[ConfirmationDialog] Error: " + ex.Message);
			Close();
		}
	}

	public static void Close()
	{
		_active = false;
		_title = null;
		_message = null;
		_onConfirm = null;
		_onCancel = null;
	}

	public static void Cleanup()
	{
		_active = false;
		_title = null;
		_message = null;
		_onConfirm = null;
		_onCancel = null;
		if (_overlayStyle?.normal.background != null)
		{
			Object.DestroyImmediate(_overlayStyle.normal.background);
		}
		if (_bgStyle?.normal.background != null)
		{
			Object.DestroyImmediate(_bgStyle.normal.background);
		}
		if (_btnConfirmStyle?.normal.background != null)
		{
			Object.DestroyImmediate(_btnConfirmStyle.normal.background);
		}
		if (_btnCancelStyle?.normal.background != null)
		{
			Object.DestroyImmediate(_btnCancelStyle.normal.background);
		}
		_overlayStyle = null;
		_bgStyle = null;
		_btnConfirmStyle = null;
		_btnCancelStyle = null;
	}

	private static GUIStyle _overlayStyle;
	private static GUIStyle _bgStyle;
	private static GUIStyle _btnConfirmStyle;
	private static GUIStyle _btnCancelStyle;

	private static GUIStyle _GetOverlayStyle()
	{
		if (_overlayStyle == null)
		{
			Texture2D tex = new Texture2D(1, 1);
			tex.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.6f));
			tex.Apply();
			_overlayStyle = new GUIStyle();
			_overlayStyle.normal.background = tex;
		}
		return _overlayStyle;
	}

	private static GUIStyle _GetBgStyle()
	{
		if (_bgStyle == null)
		{
			Texture2D tex = new Texture2D(1, 1);
			tex.SetPixel(0, 0, new Color(0.15f, 0.17f, 0.22f, 1f));
			tex.Apply();
			_bgStyle = new GUIStyle();
			_bgStyle.normal.background = tex;
			_bgStyle.border = new RectOffset(8, 8, 8, 8);
		}
		return _bgStyle;
	}

	private static GUIStyle _GetBtnStyle(bool confirm)
	{
		if (confirm && _btnConfirmStyle == null)
		{
			Texture2D tex = new Texture2D(1, 1);
			tex.SetPixel(0, 0, new Color(0.82f, 0.22f, 0.22f, 1f));
			tex.Apply();
			_btnConfirmStyle = new GUIStyle();
			_btnConfirmStyle.normal.background = tex;
			_btnConfirmStyle.fontSize = 14;
			_btnConfirmStyle.normal.textColor = Color.white;
			_btnConfirmStyle.alignment = (TextAnchor)4;
		}
		if (!confirm && _btnCancelStyle == null)
		{
			Texture2D tex = new Texture2D(1, 1);
			tex.SetPixel(0, 0, new Color(0.3f, 0.32f, 0.38f, 1f));
			tex.Apply();
			_btnCancelStyle = new GUIStyle();
			_btnCancelStyle.normal.background = tex;
			_btnCancelStyle.fontSize = 14;
			_btnCancelStyle.normal.textColor = Color.white;
			_btnCancelStyle.alignment = (TextAnchor)4;
		}
		return confirm ? _btnConfirmStyle : _btnCancelStyle;
	}
}
