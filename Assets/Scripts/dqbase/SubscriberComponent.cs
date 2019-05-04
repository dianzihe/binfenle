using UnityEngine;


public class SubscriberComponent : MonoBehaviour
{
    public static EventSubscriber GetSubscriber(GameObject target)
    {
        var sbc = target.GetComponent<SubscriberComponent>();

        if (sbc == null)
        {
            sbc = target.AddComponent<SubscriberComponent>();
        }

        return sbc.subscriber;
    }

    public static void CancleAllDelegates(GameObject target)
    {
        var sbc = target.GetComponent<SubscriberComponent>();

        if (sbc != null)
        {
            sbc.subscriber.CancleAll();
        }
    }

    public static bool GetCancleAllDelegatesOnDisable(GameObject target)
    {
        var sbc = target.GetComponent<SubscriberComponent>();

        if (sbc == null)
        {
            sbc = target.AddComponent<SubscriberComponent>();
        }

        return sbc.cancleAllDelegatesOnDisable;
    }

    public static void SetCancleAllDelegatesOnDisable(GameObject target, bool yes)
    {
        var sbc = target.GetComponent<SubscriberComponent>();

        if (sbc == null)
        {
            sbc = target.AddComponent<SubscriberComponent>();
        }

        sbc.cancleAllDelegatesOnDisable = yes;
    }

    private EventSubscriber subscriber = new EventSubscriber();
    private bool cancleAllDelegatesOnDisable = true;

    private void OnDisable()
    {
        if (cancleAllDelegatesOnDisable)
        {
            subscriber.CancleAll();
        }
    }

    private void OnDestroy()
    {
        subscriber.CancleAll();
    }
}