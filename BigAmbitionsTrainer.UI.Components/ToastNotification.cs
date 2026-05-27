using System.Collections.Generic;
using UnityEngine;

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

	private const float FadeStartTime = 2f;

	private const float TotalDuration = 3f;

	private const float TopOffset = 60f;

	private const float AccentBarWidth = 4f;

	private static readonly List<Toast> _toasts = new List<Toast>();

	private static GUIStyle _toastBgStyle;

	private static GUIStyle _successTextStyle;

	private static GUIStyle _errorTextStyle;

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
		while (_toasts.Count > 8)
		{
			_toasts.RemoveAt(0);
		}
	}

	public static void DrawToasts()
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		if (_toasts.Count == 0)
		{
			return;
		}
		EnsureStyles();
		float unscaledTime = Time.unscaledTime;
		for (int num = _toasts.Count - 1; num >= 0; num--)
		{
			if (unscaledTime - _toasts[num].SpawnTime >= 3f)
			{
				_toasts.RemoveAt(num);
			}
		}
		float num2 = (float)Screen.width - 320f - 10f;
		float num3 = 60f;
		for (int i = 0; i < _toasts.Count; i++)
		{
			Toast toast = _toasts[i];
			float num4 = unscaledTime - toast.SpawnTime;
			float num5 = 1f;
			if (num4 > 2f)
			{
				num5 = 1f - (num4 - 2f) / 1f;
				num5 = Mathf.Clamp01(num5);
			}
			Color color = GUI.color;
			GUI.color = new Color(1f, 1f, 1f, num5);
			Rect val = new Rect(num2, num3, 320f, 40f);
			FillRect(new Rect(num2 + 2f, num3 + 2f, 320f, 40f), new Color(0f, 0f, 0f, 0.08f * num5));
			GUI.Label(val, GUIContent.none, _toastBgStyle);
			FillRect(color: toast.IsSuccess ? new Color(0.2f, 0.69f, 0.42f, num5) : new Color(0.82f, 0.24f, 0.24f, num5), rect: new Rect(num2, num3, 4f, 40f));
			Rect val2 = new Rect(num2 + 4f + 10f, num3, 296f, 40f);
			GUIStyle val3 = (toast.IsSuccess ? _successTextStyle : _errorTextStyle);
			GUI.Label(val2, toast.Message, val3);
			GUI.color = color;
			num3 += 46f;
		}
	}

	private static void FillRect(Rect rect, Color color)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_fillTex == (Object)null)
		{
			_fillTex = new Texture2D(1, 1);
			_fillTex.SetPixel(0, 0, Color.white);
			_fillTex.Apply();
		}
		Color color2 = GUI.color;
		GUI.color = color;
		GUI.DrawTexture(rect, (Texture)(object)_fillTex);
		GUI.color = color2;
	}

	private static Texture2D MakeTex(Color color)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		Texture2D val = new Texture2D(1, 1);
		val.SetPixel(0, 0, color);
		val.Apply();
		return val;
	}

	private static void EnsureStyles()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
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
			_stylesInitialized = true;
		}
	}
}
