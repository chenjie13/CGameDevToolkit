# CGameDevToolkit
## 描述

用于快速开发游戏的工具和框架，包含单例模式、音效播放、调试日志、简易状态机、简易对象池、全局事件系统的实现。

## AudioManager

```C# 
//创建一个循环播放的audioUnit
var audioUnit = AudioManager.Prepare(MusicClip, loop: true, persistType: AudioPersistType.Persist);
audioUnit.Play();   //开始播放
audioUnit.Pause();  //暂停
audioUnit.Resume(); //继续播放

//渐变音量，3秒内音量从0渐变到1
audioUnit.FadeTo(0, 1, 3);
audioUnit.Stop(0);  //停止播放

//播放单次音效
AudioManager.PlayOnce(SfxClip, 1);
```

## Singleton

```C# 
//继承MonoSingleton实现单例
//使用MonoSingletonPath标签指定单例实例化的路径
[MonoSingletonPath("[Manager]/AudioManager")]
public class AudioManager : MonoSingleton<AudioManager> { }

public class SinglePropertyTest : MonoBehaviour, ISingleton
{
    //若无法继承MonoSingleton，可以使用MonoSingletonProperty来实现单机属性
    private static SinglePropertyTest Instance
    {
        get { return MonoSingletonProperty<SinglePropertyTest>.Instance; }
    }

    public void OnSingletonInit(){}
}
```

## Debug 

```C# 
//将日志输出到文件中
CDebug.LogToFile("hello");
//将日志输出到屏幕上
CDebug.LogToScreen("hello");
```

## ObjectPool

```C# 
//创建对象池
var pool = new SimpleObjectPool<Transform>(() => Instantiate(prefab), Destroy);
var obj = pool.Respawn();   //获取对象
pool.Despawn(obj);          //回收对象
```

## EventManger

```C#
//创建事件类型
public struct TestEvent
{
    private static TestEvent e;
    //需要触发事件时调用该方法 TestEvent.Trigger();
    public static void Trigger()
    {
        EventManager.TriggerEvent<TestEvent>(e);
    }
}

//事件监听者需要继承IEventListener
public class TestEventListener : MonoBehaviour, IEventListener<TestEvent>
{
    private void OnEnable()
    {
        //在OnEnable中注册监听事件
        this.EventStartListening<TestEvent>();
    }

    private void OnDisable()
    {
        //在OnDisable中取消注册监听事件
        this.EventStopListening<TestEvent>();
    }

    //触发事件时调用
    public void OnEvent(TestEvent eventType)
    {
        //do something...
    }
}
```

## Feedback(Todo)

## Reference

[QFramework](http://qfdoc.liangxiegame.com/)



