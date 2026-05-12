using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour   //“动态可用池”+“依赖计数器”
{
    public static EventManager instance;

    public List<EventData> allEventPool = new List<EventData>();    //存储所有的随机事件
    private Dictionary<EventType, List<EventData>> availableEvents = new Dictionary<EventType, List<EventData>>();   //存储各事件类型的可用事件
    private Dictionary<EventType, List<EventData>> unavailableEvents = new Dictionary<EventType, List<EventData>>(); //存储各事件类型的不可用事件

    private Dictionary<string, List<EventData>> waitingMap = new Dictionary<string, List<EventData>>(); //记录该事件关联的后继结点
    private Dictionary<string, int> remainingWaitCounts = new Dictionary<string, int>();    //记录每个结点的剩余前置节点数    

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Init();
    }

    void Update()
    {

    }

    private void Init() 
    {
        availableEvents[EventType.PreBattleEvent] = new List<EventData>();
        availableEvents[EventType.SceneChangeEvent] = new List<EventData>();
        unavailableEvents[EventType.PreBattleEvent] = new List<EventData>();
        unavailableEvents[EventType.SceneChangeEvent] = new List<EventData>();
        foreach (var eve in allEventPool) 
        {
            //如果事件有前置事件要求则对其前置条件进行处理，并将其放入不可用事件列表中
            if (eve.prerequisites.Count > 0)
            {
                unavailableEvents[eve.eventType].Add(eve);
                foreach (var e in eve.prerequisites) //遍历其前置事件列表，将其中的每一项的后继事件列表中都添加上当前的eve事件
                {
                    if (!waitingMap.ContainsKey(e.eventID))
                        waitingMap[e.eventID] = new List<EventData>();
                    waitingMap[e.eventID].Add(eve);
                }
                remainingWaitCounts[eve.eventID] = eve.prerequisites.Count;
            }
            //如果事件没有前置事件要求，则直接将其放入对应类型的可用事件列表中
            else 
            {
                availableEvents[eve.eventType].Add(eve);
            }
        }
    }

    //当某个事件完成时调用
    public void CompletedEvent(EventData eve) 
    {
        //判断该事件是否为其他事件的前置事件
        if (waitingMap.ContainsKey(eve.eventID))
        {
            foreach (var e in waitingMap[eve.eventID]) 
            {
                remainingWaitCounts[e.eventID]--;
                //如果子事件的前置事件数量小于等于0，代表子事件可以使用
                if (remainingWaitCounts[e.eventID] <= 0)
                {
                    if (unavailableEvents[e.eventType].Contains(e))
                        unavailableEvents[e.eventType].Remove(e);
                    availableEvents[e.eventType].Add(e);    //将其放入可用列表中
                    remainingWaitCounts.Remove(e.eventID);  //将其从remainingWaitCounts中移除
                }
            }
            //对当前eve事件的所有子事件进行处理后，将其从可用列表和记录后继结点列表中移除
            availableEvents[eve.eventType].Remove(eve);
            waitingMap.Remove(eve.eventID);
        }
        //如果不是直接从相应的可选用列表中移除
        else 
        {
            availableEvents[eve.eventType].Remove(eve);
        }
    }

    /// <summary>
    /// 随机获取一个目标类型的随机事件，按权重获取
    /// </summary>
    /// <param name="events"></param>
    /// <returns></returns>
    public EventData GetRandomEvent(EventType type)
    {
        List<EventData> pool = availableEvents[type];
        if (pool == null || pool.Count == 0) return null;

        EventData result = null;
        float totalWeight = pool.Sum(e => e.weight);
        float randomValue = Random.Range(0, totalWeight);
        float curSum = 0;
        foreach (var e in pool)
        {
            curSum += e.weight;
            if (curSum > randomValue) { result = e; break; }
        }

        return result;
    }
}
