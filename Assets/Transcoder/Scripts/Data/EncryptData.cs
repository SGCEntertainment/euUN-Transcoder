[System.Serializable]
public class EncryptData
{
	public string protocol;

	public string sim_geo;

	public string bundle;

	public string amidentificator;

    public string afidentificator;

    public string googleid;

	public string subcodename;

	public string appref;

	public string dl_url;

	public string dl_url2;

	public string bundle_prop;

	public string subcodename_prop;

	public string domen_prop;

	public string space_prop;

	public string requestCampaign_prop;

    public EncryptData(string protocol, string sim_geo, string bundle, string afidentificator, string amidentificator, string googleid, string subcodename, string appref, string dl_url, string dl_url2, string bundle_prop, string subcodename_prop, string domen_prop, string space_prop, string requestCampaign_prop)
    {
        this.protocol = protocol;
        this.sim_geo = sim_geo;
        this.bundle = bundle;
        this.amidentificator = amidentificator;
        this.afidentificator = afidentificator;
        this.googleid = googleid;
        this.subcodename = subcodename;
        this.appref = appref;
        this.dl_url = dl_url;
        this.dl_url2 = dl_url2;
        this.bundle_prop = bundle_prop;
        this.subcodename_prop = subcodename_prop;
        this.domen_prop = domen_prop;
        this.space_prop = space_prop;
        this.requestCampaign_prop = requestCampaign_prop;
    }
}
