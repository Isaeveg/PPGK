using UnityEngine;

public class RootMotionRelay : MonoBehaviour
{
    public PlayerController player;

    void OnAnimatorMove() => player.AddRootMotion(GetComponent<Animator>().deltaPosition);
}
