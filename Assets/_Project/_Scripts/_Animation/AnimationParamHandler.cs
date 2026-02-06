using UnityEngine;

public class AnimationParamHandler : MonoBehaviour
{
    [SerializeField] private string speedParam = "speed";
    [SerializeField] private string isJumpingParam = "isJumping";

    private Animator anim;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        if (anim == null)
            Debug.LogWarning("Animator non trovato");
    }

    public void SetSpeed(float value)
    {
        if (anim == null) return;
        anim.SetFloat(speedParam, value);
    }

    public void SetIsJumping(bool value)
    {
        if (anim == null) return;
        anim.SetBool(isJumpingParam, value);
    }
}