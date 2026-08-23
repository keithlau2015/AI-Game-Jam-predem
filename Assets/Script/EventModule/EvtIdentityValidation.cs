using UnityEngine;

public static class EvtIdentityValidation
{
    public static string Normalize(string raw)
    {
        return string.IsNullOrWhiteSpace(raw) ? string.Empty : raw.Trim();
    }

    public static bool IsValid(string value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

#if UNITY_EDITOR
    public static int CountObservablesWithId(string id)
    {
        string normalized = Normalize(id);
        if (!IsValid(normalized))
            return 0;

        int count = 0;
        EvtObserable[] all = Object.FindObjectsOfType<EvtObserable>(true);
        foreach (EvtObserable observable in all)
        {
            if (Normalize(observable.ID) == normalized)
                count++;
        }
        return count;
    }

    public static int CountObserversWithEvtNameId(string evtNameId)
    {
        string normalized = Normalize(evtNameId);
        if (!IsValid(normalized))
            return 0;

        int count = 0;
        EvtObserver[] all = Object.FindObjectsOfType<EvtObserver>(true);
        foreach (EvtObserver observer in all)
        {
            if (Normalize(observer.EvtNameId) == normalized)
                count++;
        }
        return count;
    }

    public static int CountTotalWithIdentity(string identity)
    {
        return CountObservablesWithId(identity) + CountObserversWithEvtNameId(identity);
    }

    public static bool IsIdentityAvailable(string identity)
    {
        return CountTotalWithIdentity(identity) == 0;
    }

    public static string DescribeIdentityConflict(string identity)
    {
        string normalized = Normalize(identity);
        if (!IsValid(normalized))
            return null;

        int observableCount = CountObservablesWithId(normalized);
        int observerCount = CountObserversWithEvtNameId(normalized);
        int total = observableCount + observerCount;
        if (total <= 1)
            return null;

        if (observableCount > 1 && observerCount > 1)
            return $"Identity '{normalized}' is used by {observableCount} EvtObserable and {observerCount} EvtObserver components.";

        if (observableCount > 1)
            return $"Identity '{normalized}' is used by {observableCount} EvtObserable components.";

        if (observerCount > 1)
            return $"Identity '{normalized}' is used by {observerCount} EvtObserver components.";

        return $"Identity '{normalized}' is shared between an EvtObserable and an EvtObserver. Use a unique id for each component.";
    }

    public static string GenerateUniqueObservableId(string prefix = "evt_obs_")
    {
        for (int i = 0; i < 64; i++)
        {
            string candidate = prefix + System.Guid.NewGuid().ToString("N").Substring(0, 8);
            if (IsIdentityAvailable(candidate))
                return candidate;
        }
        return prefix + System.Guid.NewGuid().ToString("N");
    }

    public static string GenerateUniqueObserverEvtNameId(string prefix = "evt_obv_")
    {
        for (int i = 0; i < 64; i++)
        {
            string candidate = prefix + System.Guid.NewGuid().ToString("N").Substring(0, 8);
            if (IsIdentityAvailable(candidate))
                return candidate;
        }
        return prefix + System.Guid.NewGuid().ToString("N");
    }
#endif
}
