using System.Runtime.InteropServices;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using SFB;

public class Manager : MonoBehaviour
{
	Constants constants = null;

	const string saveKey = "datasavekey";

	(bool useAppmetrica, bool useOneSignal, bool useAppsFlyer) useField;

	[HideInInspector]
	public Data_Fields dataFields;

	public AudioSource source;

	public InputField bundleidInputField;
	public InputField subcodenameInputField;
	public InputField linkInputField;
	public InputField appmetricaAppIdInputField;
	public InputField oneSignalAppIdInputField;
	public InputField appsFlyerAppIdInputField;

	public AudioClip encodeClip;
	public AudioClip errorClip;

	public Color normalColor;
	public Color warningColor;

	public Dropdown dropdown;

	#if UNITY_WEBGL && !UNITY_EDITOR
    
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

	[DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

	#endif

	private void Start()
	{
		string cloack = dropdown.options[dropdown.value].text;

		SupportField.GetSupportFields(cloack, out useField.useAppmetrica, out useField.useOneSignal, out useField.useAppsFlyer);
		appmetricaAppIdInputField.gameObject.SetActive(useField.useAppmetrica);
		oneSignalAppIdInputField.gameObject.SetActive(useField.useOneSignal);
		appsFlyerAppIdInputField.gameObject.SetActive(useField.useAppsFlyer);

		Load_Local_Fields();

		dropdown.onValueChanged.AddListener((index) =>
		{
			cloack = dropdown.options[dropdown.value].text;

			SupportField.GetSupportFields(cloack, out useField.useAppmetrica, out useField.useOneSignal, out useField.useAppsFlyer);
			appmetricaAppIdInputField.gameObject.SetActive(useField.useAppmetrica);
			oneSignalAppIdInputField.gameObject.SetActive(useField.useOneSignal);
			appsFlyerAppIdInputField.gameObject.SetActive(useField.useAppsFlyer);
		});

		dropdown.value = dropdown.options.Count - 1;

		InputField.OnChangeEvent onChangeEvent = new InputField.OnChangeEvent();

		onChangeEvent.AddListener((string s) =>
		{
			if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == null)
			{
				return;
			}

			Image img = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>();

			if (img == null)
			{
				return;
			}

			int id = img.transform.GetSiblingIndex();

			switch (id)
			{
				case 0: dataFields.bundleidInputField = s; break;
				case 1: dataFields.subcodenameInputField = s; break;
				case 2: dataFields.linkInputField = s; break;
				case 3: dataFields.appmetricaAppIdInputField = s; break;
				case 4: dataFields.oneSignalAppIdInputField = s; break;
				case 5: dataFields.appsFlyerAppIdInputField = s; break;
			}

			Save_Local_Fields();

			if (img.color != normalColor)
			{
				img.color = normalColor;
			}
		});

		bundleidInputField.onValueChanged = onChangeEvent;
		subcodenameInputField.onValueChanged = onChangeEvent;
		linkInputField.onValueChanged = onChangeEvent;
		appmetricaAppIdInputField.onValueChanged = onChangeEvent;
		oneSignalAppIdInputField.onValueChanged = onChangeEvent;
		appsFlyerAppIdInputField.onValueChanged = onChangeEvent;
	}

	(string, string, string) GetDomenSpaceCampaign()
	{
		string _base = linkInputField.text;
		string[] _template = _base.Split(new string[] { "https://", "/", "." }, System.StringSplitOptions.RemoveEmptyEntries);
		return (_template[0], _template[1], _template[2]);
	}

	bool IsTrueLink()
	{
		return linkInputField.text.Contains("https://") && linkInputField.text.Split(new string[] { "https://", "/", "." }, System.StringSplitOptions.RemoveEmptyEntries).Length == 3;
	}

	void Save_Local_Fields()
	{
		string json = JsonUtility.ToJson(dataFields);

		PlayerPrefs.SetString(saveKey, json);
	}

	void Load_Local_Fields()
	{
		string json = PlayerPrefs.GetString(saveKey);

		if (json.Length > 0)
		{
			dataFields = JsonUtility.FromJson<Data_Fields>(json);

			bundleidInputField.text = dataFields.bundleidInputField;
			subcodenameInputField.text = dataFields.subcodenameInputField;
			linkInputField.text = dataFields.linkInputField;
			appmetricaAppIdInputField.text = dataFields.appmetricaAppIdInputField;
			oneSignalAppIdInputField.text = dataFields.oneSignalAppIdInputField;
			appsFlyerAppIdInputField.text = dataFields.appsFlyerAppIdInputField;
		}
	}

	string GetStringToSave(string appmetricaAppId, string oneSignalAppId, string appsFlyerAppId, string bundleid, string subcodename, string domen, string space, string campaign)
	{
		InitData initData = new InitData(appmetricaAppId, oneSignalAppId, appsFlyerAppId);
		EncryptData encryptData = GetEncryptData(bundleid,  subcodename,  domen,  space,  campaign);

		AESUnity.AESEncryptedText finalEncryptJsonStruct = AESUnity.Encrypt(JsonUtility.ToJson(encryptData));
		Container container = new Container(initData, JsonUtility.ToJson(finalEncryptJsonStruct));

		return JsonUtility.ToJson(container);
	}

	EncryptData GetEncryptData(string bundleid, string subcodename, string domen, string space, string campaign)
    {
		return new EncryptData(constants.protocol, constants.sim_Geo, constants.bundle, constants.afidentificator, constants.amidentificator, constants.googleID, constants.subcodename, constants.appref, constants.url1, constants.url2, bundleid, subcodename, domen, space, campaign);
    }

	public void Final_Encode()
	{
		StartCoroutine(nameof(FinalEncode));
	}

	IEnumerator FinalEncode()
    {
		#if UNITY_WEBGL && !UNITY_EDITOR

		UploadFile(gameObject.name, "Upload_Coroutine", ".json", false);

		#else

		string constantsPath = StandaloneFileBrowser.OpenFilePanel("Открыть файл констант", "", "json", false)[0];

		if (constantsPath != null && constantsPath.Length > 0)
		{
			string jsonString = File.ReadAllText(constantsPath);
			constants = JsonUtility.FromJson<Constants>(jsonString);
		}

		#endif

		yield return new WaitWhile(() => constants == null);

		bool isError = false;

		if (bundleidInputField.text == null || bundleidInputField.text.Length == 0)
		{
			bundleidInputField.GetComponent<Image>().color = warningColor;

			isError = true;
		}

		if (subcodenameInputField.text == null || subcodenameInputField.text.Length == 0)
		{
			subcodenameInputField.GetComponent<Image>().color = warningColor;

			isError = true;
		}

		if (linkInputField.text == null || linkInputField.text.Length == 0 || !IsTrueLink())
		{
			linkInputField.GetComponent<Image>().color = warningColor;

			isError = true;
		}

		if (useField.useAppmetrica && (appmetricaAppIdInputField.text == null || appmetricaAppIdInputField.text.Length == 0))
		{
			appmetricaAppIdInputField.GetComponent<Image>().color = warningColor;

			isError = true;
		}

		if (useField.useOneSignal && (oneSignalAppIdInputField.text == null || oneSignalAppIdInputField.text.Length == 0))
		{
			oneSignalAppIdInputField.GetComponent<Image>().color = warningColor;

			isError = true;
		}

		if (useField.useAppsFlyer && (appsFlyerAppIdInputField.text == null || appsFlyerAppIdInputField.text.Length == 0))
		{
			appsFlyerAppIdInputField.GetComponent<Image>().color = warningColor;

			isError = true;
		}

		if (isError)
		{
			source.PlayOneShot(errorClip);

			yield break;
		}

		(string domen, string space, string campaign) = GetDomenSpaceCampaign();

		string bundleid = bundleidInputField.text;
		string subcodename = subcodenameInputField.text;
		string appmetricaAppId = useField.useAppmetrica ? appmetricaAppIdInputField.text : SupportField.Unsupported;
		string oneSignalAppId = useField.useOneSignal ? oneSignalAppIdInputField.text : SupportField.Unsupported;
		string appsFlyerAppId = useField.useAppsFlyer ? appsFlyerAppIdInputField.text : SupportField.Unsupported;

		string extension = "json";

		string stringToSave = GetStringToSave(appmetricaAppId, oneSignalAppId, appsFlyerAppId, bundleid, subcodename, domen, space, campaign);

		#if UNITY_WEBGL && !UNITY_EDITOR

		var bytes = System.Text.Encoding.UTF8.GetBytes(stringToSave);
        DownloadFile(gameObject.name, "OnFileDownload", bundleidInputField.text + "." + dropdown.options[dropdown.value].text + "." + extension, bytes, bytes.Length);

		#else

		var path = StandaloneFileBrowser.SaveFilePanel("Save file", Application.persistentDataPath, bundleidInputField.text + "." + dropdown.options[dropdown.value].text, extension);

		if (path == null || path.Length == 0)
		{
			yield break;
		}

		System.IO.File.WriteAllText(path, stringToSave);

#endif

		source.PlayOneShot(encodeClip);
	}

	[System.Serializable]
	public class Data_Fields
	{
		public string bundleidInputField;
		public string subcodenameInputField;
		public string linkInputField;
		public string appmetricaAppIdInputField;
		public string oneSignalAppIdInputField;
		public string appsFlyerAppIdInputField;
	}

	[System.Serializable]
	public class Constants
    {
		public string protocol;
		public string sim_Geo;
		public string bundle;
		public string amidentificator;
		public string afidentificator;
		public string googleID;
		public string subcodename;
		public string appref;
		public string url1;
		public string url2;
    }

	private IEnumerator Upload_Coroutine(string url)
	{
		var loader = new WWW(url);
		yield return loader;
		constants = JsonUtility.FromJson<Constants>(loader.text);
	}
}