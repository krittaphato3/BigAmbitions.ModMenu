using System;
using MelonLoader;

namespace BigAmbitionsTrainer.Modules;

public static class UndoSystem
{
	private static Action _undoAction;

	public static string LastActionDescription { get; private set; } = "";

	public static bool HasUndo => _undoAction != null;

	public static void RegisterUndo(string description, Action undoAction)
	{
		LastActionDescription = description;
		_undoAction = undoAction;
	}

	public static void Undo()
	{
		if (_undoAction == null)
		{
			MelonLogger.Msg("[UndoSystem] No undo action available.");
			return;
		}
		try
		{
			MelonLogger.Msg("[UndoSystem] Undoing: " + LastActionDescription);
			_undoAction();
			MelonLogger.Msg("[UndoSystem] Undo completed.");
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[UndoSystem] Undo failed: " + ex.Message);
		}
		finally
		{
			Clear();
		}
	}

	public static void Clear()
	{
		_undoAction = null;
		LastActionDescription = "";
	}
}
