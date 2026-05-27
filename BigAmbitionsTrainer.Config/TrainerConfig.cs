using MelonLoader;
using MelonLoader.Preferences;

namespace BigAmbitionsTrainer.Config;

public static class TrainerConfig
{
	private static MelonPreferences_Category _category;

	private static MelonPreferences_Entry<bool> _disableEnergy;

	private static MelonPreferences_Entry<bool> _disableHappiness;

	private static MelonPreferences_Entry<bool> _disableAging;

	private static MelonPreferences_Entry<bool> _disableVehicleDamage;

	private static MelonPreferences_Entry<bool> _disableVehicleFuel;

	private static MelonPreferences_Entry<bool> _allCoursesUnlocked;

	private static MelonPreferences_Entry<bool> _allContactsUnlocked;

	private static MelonPreferences_Entry<bool> _disableWholesaleImportLimits;

	private static MelonPreferences_Entry<bool> _allProductsFromImporters;

	private static MelonPreferences_Entry<bool> _disableTraffic;

	private static MelonPreferences_Entry<bool> _disableTutorial;

	private static MelonPreferences_Entry<bool> _invincibility;

	private static MelonPreferences_Entry<bool> _disableHunger;

	private static MelonPreferences_Entry<float> _gameSpeed;

	private static MelonPreferences_Entry<bool> _phoneIntegration;

	private static MelonPreferences_Entry<float> _toastDuration;

	private static MelonPreferences_Entry<float> _toastFadeStart;

	private static MelonPreferences_Entry<int> _candidateLevel;

	public static float ToastDuration => _toastDuration.Value;

	public static float ToastFadeStart => _toastFadeStart.Value;

	public static int CandidateLevel
	{
		get
		{
			return _candidateLevel.Value;
		}
		set
		{
			_candidateLevel.Value = value;
			Save();
		}
	}

	public static bool DisableEnergy
	{
		get
		{
			return _disableEnergy.Value;
		}
		set
		{
			_disableEnergy.Value = value;
			Save();
		}
	}

	public static bool DisableHappiness
	{
		get
		{
			return _disableHappiness.Value;
		}
		set
		{
			_disableHappiness.Value = value;
			Save();
		}
	}

	public static bool DisableAging
	{
		get
		{
			return _disableAging.Value;
		}
		set
		{
			_disableAging.Value = value;
			Save();
		}
	}

	public static bool DisableVehicleDamage
	{
		get
		{
			return _disableVehicleDamage.Value;
		}
		set
		{
			_disableVehicleDamage.Value = value;
			Save();
		}
	}

	public static bool DisableVehicleFuel
	{
		get
		{
			return _disableVehicleFuel.Value;
		}
		set
		{
			_disableVehicleFuel.Value = value;
			Save();
		}
	}

	public static bool AllCoursesUnlocked
	{
		get
		{
			return _allCoursesUnlocked.Value;
		}
		set
		{
			_allCoursesUnlocked.Value = value;
			Save();
		}
	}

	public static bool AllContactsUnlocked
	{
		get
		{
			return _allContactsUnlocked.Value;
		}
		set
		{
			_allContactsUnlocked.Value = value;
			Save();
		}
	}

	public static bool DisableWholesaleImportLimits
	{
		get
		{
			return _disableWholesaleImportLimits.Value;
		}
		set
		{
			_disableWholesaleImportLimits.Value = value;
			Save();
		}
	}

	public static bool AllProductsFromImporters
	{
		get
		{
			return _allProductsFromImporters.Value;
		}
		set
		{
			_allProductsFromImporters.Value = value;
			Save();
		}
	}

	public static bool DisableTraffic
	{
		get
		{
			return _disableTraffic.Value;
		}
		set
		{
			_disableTraffic.Value = value;
			Save();
		}
	}

	public static bool DisableTutorial
	{
		get
		{
			return _disableTutorial.Value;
		}
		set
		{
			_disableTutorial.Value = value;
			Save();
		}
	}

	public static bool Invincibility
	{
		get
		{
			return _invincibility.Value;
		}
		set
		{
			_invincibility.Value = value;
			Save();
		}
	}

	public static bool DisableHunger
	{
		get
		{
			return _disableHunger.Value;
		}
		set
		{
			_disableHunger.Value = value;
			Save();
		}
	}

	public static float GameSpeed
	{
		get
		{
			return _gameSpeed.Value;
		}
		set
		{
			_gameSpeed.Value = value;
			Save();
		}
	}

	public static bool PhoneIntegration
	{
		get
		{
			return _phoneIntegration.Value;
		}
		set
		{
			_phoneIntegration.Value = value;
			Save();
		}
	}

	public static void Initialize()
	{
		_category = MelonPreferences.CreateCategory("ItzRealOzoneTrainer", "ItzRealOzone Trainer");
		_disableEnergy = _category.CreateEntry<bool>("DisableEnergy", false, "Disable Energy Decay", (string)null, false, false, (ValueValidator)null, (string)null);
		_disableHappiness = _category.CreateEntry<bool>("DisableHappiness", false, "Disable Happiness Decay", (string)null, false, false, (ValueValidator)null, (string)null);
		_disableAging = _category.CreateEntry<bool>("DisableAging", false, "Disable Aging", (string)null, false, false, (ValueValidator)null, (string)null);
		_disableVehicleDamage = _category.CreateEntry<bool>("DisableVehicleDamage", false, "Disable Vehicle Damage", (string)null, false, false, (ValueValidator)null, (string)null);
		_disableVehicleFuel = _category.CreateEntry<bool>("DisableVehicleFuel", false, "Disable Vehicle Fuel", (string)null, false, false, (ValueValidator)null, (string)null);
		_allCoursesUnlocked = _category.CreateEntry<bool>("AllCoursesUnlocked", false, "Unlock All Courses", (string)null, false, false, (ValueValidator)null, (string)null);
		_allContactsUnlocked = _category.CreateEntry<bool>("AllContactsUnlocked", false, "Unlock All Contacts", (string)null, false, false, (ValueValidator)null, (string)null);
		_disableWholesaleImportLimits = _category.CreateEntry<bool>("DisableWholesaleImportLimits", false, "Disable Wholesale/Import Limits", (string)null, false, false, (ValueValidator)null, (string)null);
		_allProductsFromImporters = _category.CreateEntry<bool>("AllProductsFromImporters", false, "All Products From Importers", (string)null, false, false, (ValueValidator)null, (string)null);
		_disableTraffic = _category.CreateEntry<bool>("DisableTraffic", false, "Disable Traffic", (string)null, false, false, (ValueValidator)null, (string)null);
		_disableTutorial = _category.CreateEntry<bool>("DisableTutorial", false, "Disable Tutorial", (string)null, false, false, (ValueValidator)null, (string)null);
		_invincibility = _category.CreateEntry<bool>("Invincibility", false, "Invincibility", (string)null, false, false, (ValueValidator)null, (string)null);
		_disableHunger = _category.CreateEntry<bool>("DisableHunger", false, "Disable Hunger Decay", (string)null, false, false, (ValueValidator)null, (string)null);
		_gameSpeed = _category.CreateEntry<float>("GameSpeed", 1f, "Game Speed Multiplier", (string)null, false, false, (ValueValidator)null, (string)null);
		_phoneIntegration = _category.CreateEntry<bool>("PhoneIntegration", true, "Phone Menu Integration", (string)null, false, false, (ValueValidator)null, (string)null);
		_toastDuration = _category.CreateEntry<float>("ToastDuration", 3f, "Toast Duration (seconds)", (string)null, false, false, (ValueValidator)null, (string)null);
		_toastFadeStart = _category.CreateEntry<float>("ToastFadeStart", 2f, "Toast Fade Start (seconds)", (string)null, false, false, (ValueValidator)null, (string)null);
		_candidateLevel = _category.CreateEntry<int>("CandidateLevel", 100, "Employee Candidate Skill Level", (string)null, false, false, (ValueValidator)null, (string)null);
		MelonLogger.Msg("[Config] Preferences loaded.");
	}

	public static void Save()
	{
		_category.SaveToFile(false);
	}

	public static void Load()
	{
		_category.LoadFromFile();
	}
}
