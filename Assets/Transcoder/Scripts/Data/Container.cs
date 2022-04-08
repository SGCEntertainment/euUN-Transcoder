[System.Serializable]
public class Container
{
    public InitData initData;
    public string encryptString;

    public Container(InitData initData, string encryptString)
    {
        this.initData = initData;
        this.encryptString = encryptString;
    }
}
