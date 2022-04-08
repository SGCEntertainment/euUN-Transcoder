class SupportField
{
	public const string Unsupported = "Не поддерживается";

	readonly static string[] useAppMetrica = { "UNv8", "UNv9", "UNv10" };
	readonly static string[] useOneSignal = { };
	readonly static string[] useAppsFlyer = { "UNv8", "UNv9", "UNv10" };

	static bool UseAppMetrica(string forCloack) => System.Array.IndexOf(useAppMetrica, forCloack) >= 0;
	static bool UseOneSignal(string forCloack) => System.Array.IndexOf(useOneSignal, forCloack) >= 0;
	static bool UseAppsFlyer(string forCloack) => System.Array.IndexOf(useAppsFlyer, forCloack) >= 0;

	public static void GetSupportFields(string forCloack, out bool useAppmetricaField, out bool useOneSignalField, out bool useAppsFlyerField)
    {
		useAppmetricaField = UseAppMetrica(forCloack);
		useOneSignalField = UseOneSignal(forCloack);
		useAppsFlyerField = UseAppsFlyer(forCloack);
	}
}
