using UnityEngine;

// =============================================================
// POOL HOST
// =============================================================
//
// Owns one ObjectPool and gives pooled GameObjects somewhere to
// live in the hierarchy.
//
// WHY THIS EXISTS:
//
// ObjectPool is a plain object, deliberately - the project rule
// forbids mutable state in a static class, so there is no
// global pool to reach for. Something has to own an instance,
// and that something has to be a MonoBehaviour if pooled
// GameObjects are to be parented and cleaned up with a scene.
//
// HOW TO USE IT:
//
// Drop one on an empty GameObject and point the components that
// want to share a pool at it. Components with no host assigned
// keep a private pool of their own instead - see SoundPlayer.
// Sharing a host is what makes two SoundPlayers share voices.
// =============================================================

public class PoolHost : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Where pooled GameObjects are parented. Left " +
             "empty, this object is used.")]
    private Transform poolParent;


    private readonly ObjectPool pool = new ObjectPool();


    public ObjectPool Pool => pool;

    public Transform PoolParent =>
        poolParent != null ? poolParent : transform;


    // =========================================================
    // TEARDOWN
    // =========================================================
    //
    // The pool holds references, not lifetimes. Pooled objects
    // are children of PoolParent, so Unity destroys them with
    // this host; dropping the references here stops the pool
    // from handing out corpses if it somehow outlives us.

    private void OnDestroy()
    {
        pool.ClearAll();
    }
}
