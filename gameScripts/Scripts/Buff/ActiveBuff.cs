using UnityEngine;

//运行时实例类
[System.Serializable]
public class ActiveBuff
{
    public BuffBase buff;
    public float remainingTime; //技能剩余时间
    public GameObject creator;  //buff释放者
    public bool isApply;    //该效果是否处于应用状态

    public ActiveBuff(BuffBase buff, float remainingTime, GameObject creator, bool isApply = true)
    {
        this.buff = buff;
        this.remainingTime = remainingTime;
        this.creator = creator;
        this.isApply = isApply;
    }
}
