using System.Collections.Generic;
using BigAmbitionsTrainer.Config;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BigAmbitionsTrainer.UI.Components;

public static class ToastNotification
{
	private class Toast
	{
		public string Message;

		public bool IsSuccess;

		public float SpawnTime;
	}

	private const float ToastWidth = 320f;

	private const float ToastHeight = 40f;

	private const float ToastMargin = 10f;

	private const float TopOffset = 60f;

	private const float AccentBarWidth = 4f;

	private const int MaxToasts = 8;

	private static readonly List<Toast> _toasts = new List<Toast>();

	private static GUIStyle _toastBgStyle;

	private static GUIStyle _successTextStyle;

	private static GUIStyle _errorTextStyle;

	private static GUIStyle _fillStyle;

	private static Texture2D _fillTex;

	private static bool _stylesInitialized;

	public static void Show(string message, bool success = true)
	{
		_toasts.Add(new Toast
		{
			Message = message,
			IsSuccess = success,
			SpawnTime = Time.unscaledTime
		});
		while (_toasts.Count > MaxToasts)
		{
			_toasts.RemoveAt(0);
		}
	}

	public static void DrawToasts()
	{
		if (_toasts.Count == 0)
		{
			return;
		}
		try
		{
			EnsureStyles();
			float unscaledTime = Time.unscaledTime;
			float totalDuration = TrainerConfig.ToastDuration;
			float fadeStart = TrainerConfig.ToastFadeStart;
			for (int num = _toasts.Count - 1; num >= 0; num--)
			{
				if (unscaledTime - _toasts[num].SpawnTime >= totalDuration)
				{
					_toasts.RemoveAt(num);
				}
			}
			if (_toasts.Count == 0)
			{
				return;
			}
			float num2 = (float)Screen.width - ToastWidth - ToastMargin;
			float num3 = TopOffset;
			for (int i = 0; i < _toasts.Count; i++)
			{
				Toast toast = _toasts[i];
				float num4 = unscaledTime - toast.SpawnTime;
				float num5 = 1f;
				if (num4 > fadeStart)
				{
					num5 = 1f - (num4 - fadeStart) / (totalDuration - fadeStart);
					num5 = Mathf.Clamp01(num5);
				}
				Color color = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, num5);
				Rect val = new Rect(num2, num3, ToastWidth, ToastHeight);
				FillRect(new Rect(num2 + 2f, num3 + 2f, ToastWidth, ToastHeight), new Color(0f, 0f, 0f, 0.08f * num5));
				GUI.Label(val, GUIContent.none, _toastBgStyle);
				FillRect(color: toast.IsSuccess ? new Color(0.2f, 0.69f, 0.42f, num5) : new Color(0.82f, 0.24f, 0.24f, num5), rect: new Rect(num2, num3, AccentBarWidth, ToastHeight));
				Rect val2 = new Rect(num2 + AccentBarWidth + ToastMargin, num3, ToastWidth - AccentBarWidth - ToastMargin, ToastHeight);
				GUIStyle val3 = (toast.IsSuccess ? _successTextStyle : _errorTextStyle);
				GUI.Label(val2, toast.Message, val3);
				GUI.color = color;
				num3 += 46f;
			}
		}
		catch
		{
		}
	}

	private static void FillRect(Rect rect, Color color)
	{
		Color color2 = GUI.color;
		GUI.color = color;
		GUI.Box(rect, GUIContent.none, _fillStyle);
		GUI.color = color2;
	}

	private static Texture2D MakeTex(Color color)
	{
		Texture2D val = new Texture2D(1, 1);
		val.SetPixel(0, 0, color);
		val.Apply();
		return val;
	}

	public static void Cleanup()
	{
		_toasts.Clear();
		if (_fillTex != null)
		{
			Object.DestroyImmediate(_fillTex);
			_fillTex = null;
		}
		_toastBgStyle = null;
		_successTextStyle = null;
		_errorTextStyle = null;
		_fillStyle = null;
		_stylesInitialized = false;
	}

	private static void EnsureStyles()
	{
		if (!_stylesInitialized)
		{
			_toastBgStyle = new GUIStyle();
			GUIStyleState val = new GUIStyleState();
			val.background = MakeTex(new Color(1f, 1f, 1f, 0.96f));
			_toastBgStyle.normal = val;
			_successTextStyle = new GUIStyle();
			GUIStyleState val2 = new GUIStyleState();
			val2.textColor = new Color(0.12f, 0.14f, 0.18f, 1f);
			_successTextStyle.normal = val2;
			_successTextStyle.fontSize = 13;
			_successTextStyle.alignment = (TextAnchor)3;
			_successTextStyle.padding = new RectOffset(0, 0, 0, 0);
			_successTextStyle.wordWrap = true;
			_errorTextStyle = new GUIStyle();
			GUIStyleState val3 = new GUIStyleState();
			val3.textColor = new Color(0.72f, 0.18f, 0.18f, 1f);
			_errorTextStyle.normal = val3;
			_errorTextStyle.fontSize = 13;
			_errorTextStyle.alignment = (TextAnchor)3;
			_errorTextStyle.padding = new RectOffset(0, 0, 0, 0);
			_errorTextStyle.wordWrap = true;
			_fillTex = new Texture2D(1, 1);
			_fillTex.SetPixel(0, 0, Color.white);
			_fillTex.Apply();
			_fillStyle = new GUIStyle();
			GUIStyleState fillState = new GUIStyleState();
			fillState.background = _fillTex;
			_fillStyle.normal = fillState;
			_stylesInitialized = true;
		}
	}
}
