public class GameResourceManager
{
    public static GameResourceManager instance = new GameResourceManager();
    private GameResourceManager() { }

    private int _food;
    private int _mineral;  //能源矿
    private int _medicalItems;  //医疗品
    private int _purifyEnergy;  //净化能量
    private int _coin;   //金币

    public int GetFood() { return _food; }
    public void SetFood(int value) { _food += value; if (_food < 0) _food = 0; }

    public int GetMineral() { return _mineral; }
    public void SetMineral(int value) { _mineral += value; if (_mineral < 0) _mineral = 0; }

    public int GetMedicalItems() { return _medicalItems; }
    public void SetMedicalItems(int value) { _medicalItems += value; if (_medicalItems < 0) _medicalItems = 0; }

    public int GetPurifyEnergy() { return _purifyEnergy; }
    public void SetPurifyEnergy(int value) { _purifyEnergy += value; if (_purifyEnergy < 0) _purifyEnergy = 0; }

    public int GetCoin() { return _coin; }
    public void SetCoin(int value) { _coin += value; if (_coin < 0) _coin = 0; }
}
