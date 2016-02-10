using UnityEngine;
using System.Collections;

public class IntroAnimation : StateMachineBehaviour
{
    public string state = null;
    private int idleCounter = 0;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (this.state == "Walking")
            Camera.main.GetComponent<IntroAnimator>().startIntro();
        else if (this.state == "Text3")
            IntroAnimator.IntroGameObject.GetComponent<Animator>().enabled = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (this.state == "WalkingBack")
            Camera.main.GetComponent<IntroAnimator>().endIntro();
        else if (this.state == "Text3")
            GameObject.Find("__StartScreen").GetComponent<StartScreen>().setShown(false);
        else if (this.state == "Text1")
            GameObject.Find("__StartScreen").transform.FindChild("MainScreen").GetComponent<Animator>().enabled = false;
        else if (this.state == "WakingUp")
        {
            IntroAnimator.PrincessGameObject.GetComponent<SpriteRenderer>().enabled = true;
            IntroAnimator.PrincessGameObject.GetComponent<Animator>().Play("Idle");
            Destroy(IntroAnimator.IntroGameObject.GetComponent<Animator>(), 0.25f);
        }
        else if (this.state == "Idle")
        {
            idleCounter++;
            if (idleCounter > 1)
                IntroAnimator.PrincessGameObject.GetComponent<Animator>().Play("Walking");
        }
        else if (this.state == "GameOver")
            Application.LoadLevel(Application.loadedLevelName);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
