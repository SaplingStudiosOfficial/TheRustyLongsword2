using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody player;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        AnimationsControls();
    }
    private void AnimationsControls()
    {
        if (player.velocity.x > 0 || player.velocity.x < 0 || player.velocity.z < 0 || player.velocity.z > 0)
        {
            anim.SetInteger("state", 1);
        }
        if (player.velocity.x == 0 && player.velocity.y == 0 && player.velocity.z == 0)
        {
            anim.SetInteger("state", 0);
        }
    }
}
