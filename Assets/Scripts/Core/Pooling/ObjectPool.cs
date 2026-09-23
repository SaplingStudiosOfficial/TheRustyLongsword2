using System;
using System.Collections.Generic;

// =============================================================
// OBJECT POOL
// =============================================================
//
// One pool, many categories. Every item is filed under a
// string key from PoolKeys, and each key owns its own idle and
// live lists. Callers ask by key and get the item back already
// typed.
//
// WHY THIS EXISTS:
//
// Unity cannot give two overlapping sounds different pitches
// from one AudioSource - pitch belongs to the source, not to
// the sound. Playing varied one-shots means borrowing a spare
// source per shot, which means pooling.
//
// That problem is not specific to audio, so this pool is not
// either. Anything that churns short-lived objects can use it.
//
// WHY THIS IS NOT STATIC:
//
// A pool is mutable state, and the project rule is that mutable
// state never lives in a static class. This is an ordinary
// object that something owns - see PoolHost.
// =============================================================

public class ObjectPool
{
    // =========================================================
    // BUCKET
    // =========================================================
    //
    // One key's worth of pool. The factory is how the bucket
    // grows when every item it owns is already out on loan.

    private class Bucket
    {
        public Func<object> Factory;
        public int MaxSize;

        public readonly List<object> Idle = new List<object>();
        public readonly List<object> Live = new List<object>();
    }


    private readonly Dictionary<string, Bucket> buckets =
        new Dictionary<string, Bucket>();


    // =========================================================
    // REGISTER
    // =========================================================
    //
    // Teach the pool how to make one of these. Must be called
    // before the first Get for that key.
    //
    // maxSize of zero means the bucket grows without limit.
    // Above zero, a Get made while every item is already live
    // takes the OLDEST live item back and hands it out again -
    // which invalidates whoever was holding it. Callers that
    // cannot survive that should cap themselves and leave
    // maxSize at zero. SoundPlayer does exactly that.

    public void Register(
        string key,
        Func<object> factory,
        int maxSize = 0)
    {
        if (string.IsNullOrEmpty(key))
        {
            GameLog.Error(
                "ObjectPool.Register was given an empty key."
            );

            return;
        }

        if (factory == null)
        {
            GameLog.Error(
                "ObjectPool.Register was given no factory for key "
                + Quote(key) + "."
            );

            return;
        }

        if (!buckets.TryGetValue(key, out Bucket bucket))
        {
            bucket = new Bucket();
            buckets.Add(key, bucket);
        }

        bucket.Factory = factory;
        bucket.MaxSize = maxSize < 0 ? 0 : maxSize;
    }


    public bool IsRegistered(string key)
    {
        return
            buckets.TryGetValue(key, out Bucket bucket)
            && bucket.Factory != null;
    }


    // =========================================================
    // PREWARM
    // =========================================================
    //
    // Build items up front so the first use does not stutter.

    public void Prewarm(string key, int count)
    {
        Bucket bucket = Resolve(key);

        if (bucket == null)
        {
            return;
        }

        for (int i = 0; i < count; i++)
        {
            object made = bucket.Factory();

            if (made == null)
            {
                GameLog.Error(
                    "ObjectPool factory for key " + Quote(key)
                    + " returned nothing while prewarming."
                );

                return;
            }

            bucket.Idle.Add(made);
        }
    }


    // =========================================================
    // GET
    // =========================================================
    //
    // The pool maths - reuse an idle item, else grow, else
    // recycle the oldest live one - happens on the raw object.
    // The cast back to T happens here so call sites stay clean.

    public T Get<T>(string key) where T : class
    {
        object raw = GetRaw(key);

        if (raw == null)
        {
            return null;
        }

        T typed = raw as T;

        if (typed == null)
        {
            GameLog.Error(
                "ObjectPool key " + Quote(key) + " holds "
                + raw.GetType().Name + ", but "
                + typeof(T).Name + " was asked for. "
                + "Two callers are sharing one key."
            );

            return null;
        }

        return typed;
    }


    private object GetRaw(string key)
    {
        Bucket bucket = Resolve(key);

        if (bucket == null)
        {
            return null;
        }

        // Idle items can have been destroyed underneath us by a
        // scene unload. Drop those rather than hand back a
        // reference that throws on first use.
        while (bucket.Idle.Count > 0)
        {
            int last = bucket.Idle.Count - 1;
            object candidate = bucket.Idle[last];

            bucket.Idle.RemoveAt(last);

            if (IsDestroyed(candidate))
            {
                continue;
            }

            bucket.Live.Add(candidate);

            return candidate;
        }

        bool atCap =
            bucket.MaxSize > 0
            && bucket.Live.Count >= bucket.MaxSize;

        if (atCap)
        {
            object oldest = bucket.Live[0];

            bucket.Live.RemoveAt(0);
            bucket.Live.Add(oldest);

            GameLog.Warning(
                "ObjectPool key " + Quote(key) + " hit its cap of "
                + bucket.MaxSize + " and recycled the oldest item "
                + "while it was still in use."
            );

            return oldest;
        }

        object fresh = bucket.Factory();

        if (fresh == null)
        {
            GameLog.Error(
                "ObjectPool factory for key " + Quote(key)
                + " returned nothing."
            );

            return null;
        }

        bucket.Live.Add(fresh);

        return fresh;
    }


    // =========================================================
    // RELEASE
    // =========================================================
    //
    // Returns true when the item actually went back to idle.
    // Releasing something twice, or something this pool never
    // issued, is a no-op rather than an exception - a release
    // often runs from a coroutine that outlived its reason.

    public bool Release(string key, object instance)
    {
        if (instance == null)
        {
            return false;
        }

        if (!buckets.TryGetValue(key, out Bucket bucket))
        {
            return false;
        }

        int index = bucket.Live.IndexOf(instance);

        if (index < 0)
        {
            return false;
        }

        bucket.Live.RemoveAt(index);

        if (IsDestroyed(instance))
        {
            return false;
        }

        bucket.Idle.Add(instance);

        return true;
    }


    public void ReleaseAll(string key)
    {
        if (!buckets.TryGetValue(key, out Bucket bucket))
        {
            return;
        }

        for (int i = 0; i < bucket.Live.Count; i++)
        {
            object instance = bucket.Live[i];

            if (!IsDestroyed(instance))
            {
                bucket.Idle.Add(instance);
            }
        }

        bucket.Live.Clear();
    }


    // =========================================================
    // CLEAR
    // =========================================================
    //
    // Drops the pool's references. It does NOT destroy anything
    // - pooled GameObjects are parented under their PoolHost and
    // die with it, which keeps this class free of Unity types.

    public void Clear(string key)
    {
        if (!buckets.TryGetValue(key, out Bucket bucket))
        {
            return;
        }

        bucket.Idle.Clear();
        bucket.Live.Clear();
    }


    public void ClearAll()
    {
        foreach (KeyValuePair<string, Bucket> pair in buckets)
        {
            pair.Value.Idle.Clear();
            pair.Value.Live.Clear();
        }
    }


    // =========================================================
    // COUNTS
    // =========================================================
    //
    // For debugging. An unknown key reports zero rather than
    // throwing.

    public int IdleCount(string key)
    {
        return
            buckets.TryGetValue(key, out Bucket bucket)
                ? bucket.Idle.Count
                : 0;
    }


    public int LiveCount(string key)
    {
        return
            buckets.TryGetValue(key, out Bucket bucket)
                ? bucket.Live.Count
                : 0;
    }


    // =========================================================
    // INTERNALS
    // =========================================================

    private Bucket Resolve(string key)
    {
        if (!buckets.TryGetValue(key, out Bucket bucket))
        {
            GameLog.Warning(
                "ObjectPool was asked for key " + Quote(key)
                + ", which was never registered."
            );

            return null;
        }

        if (bucket.Factory == null)
        {
            GameLog.Warning(
                "ObjectPool key " + Quote(key)
                + " has no factory registered."
            );

            return null;
        }

        return bucket;
    }


    // A pooled item can be a UnityEngine.Object that Unity has
    // since destroyed. Only Unity's overloaded == notices that;
    // a plain null check does not.
    private static bool IsDestroyed(object instance)
    {
        if (instance is UnityEngine.Object unityObject)
        {
            return unityObject == null;
        }

        return instance == null;
    }


    private static string Quote(string value)
    {
        return "'" + value + "'";
    }
}
