using UnityEngine;
using System.Collections;

/** AI controller specifically made for the spider robot.
 * The spider robot (or mine-bot) which is got from the Unity Example Project
 * can have this script attached to be able to pathfind around with animations working properly.\n
 * This script should be attached to a parent GameObject however since the original bot has Z+ as up.
 * This component requires Z+ to be forward and Y+ to be up.\n
 * 
 * It overrides the AIPath class, see that class's documentation for more information on most variables.\n
 * Animation is handled by this component. The Animation component refered to in #anim should have animations named "awake" and "forward".
 * The forward animation will have it's speed modified by the velocity and scaled by #animationSpeed to adjust it to look good.
 * The awake animation will only be sampled at the end frame and will not play.\n
 * When the end of path is reached, if the #endOfPathEffect is not null, it will be instantiated at the current position. However a check will be
 * done so that it won't spawn effects too close to the previous spawn-point.
 * \shadowimage{mine-bot.png}
 */
[RequireComponent(typeof(Seeker))]
public class BotAI : AIPath {
	
	/** Animation component.
	 * Should hold animations "awake" and "forward"
	 */
    public string animationName = "run";
    public Animation anim;
	
	/** Minimum velocity for moving */
	public float sleepVelocity = 0.4F;
	
	/** Speed relative to velocity with which to play animations */
	public float animationSpeed = 0.2F;
	
	/** Effect which will be instantiated when end of path is reached.
	 * \see OnTargetReached */
	public GameObject endOfPathEffect;
	
	public new void Start () {
		
		//Prioritize the walking animation
        anim[animationName].layer = 10;
		
		//Play all animations
        anim.Play(animationName);
		anim.Play (animationName);
		
		//Setup awake animations properties
        /*
        anim[animationName].wrapMode = WrapMode.Clamp;
        anim[animationName].speed = 0;
        anim[animationName].normalizedTime = 1F;
		*/
		//Call Start in base script (AIPath)
		base.Start ();
	}
	
	/** Point for the last spawn of #endOfPathEffect */
	protected Vector3 lastTarget;
	
	/**
	 * Called when the end of path has been reached.
	 * An effect (#endOfPathEffect) is spawned when this function is called
	 * However, since paths are recalculated quite often, we only spawn the effect
	 * when the current position is some distance away from the previous spawn-point
	*/
	public override void OnTargetReached () {
		
		if (endOfPathEffect != null && Vector3.Distance (tr.position, lastTarget) > 1) {
			GameObject.Instantiate (endOfPathEffect,tr.position,tr.rotation);
			lastTarget = tr.position;
		}
	}
	
	public override Vector3 GetFeetPosition ()
	{
		return tr.position;
	}
	
	protected new void Update () {
		
		//Get velocity in world-space
		Vector3 velocity;
		if (canMove) {
		
			//Calculate desired velocity
			Vector3 dir = CalculateVelocity (GetFeetPosition());
			
			//Rotate towards targetDirection (filled in by CalculateVelocity)
			if (targetDirection != Vector3.zero) {
				RotateTowards (targetDirection);
			}
			
			if (dir.sqrMagnitude > sleepVelocity*sleepVelocity) {
				//If the velocity is large enough, move
			} else {
				//Otherwise, just stand still (this ensures gravity is applied)
				dir = Vector3.zero;
			}
			
			if (navController != null)
				navController.SimpleMove (GetFeetPosition(), dir);
			else if (controller != null)
				controller.SimpleMove (dir);
			else
				Debug.LogWarning ("No NavmeshController or CharacterController attached to GameObject");
			
			velocity = controller.velocity;
		} else {
			velocity = Vector3.zero;
		}
		
		
		//Animation
		
		//Calculate the velocity relative to this transform's orientation
		Vector3 relVelocity = tr.InverseTransformDirection (velocity);
		
		if (velocity.sqrMagnitude <= sleepVelocity*sleepVelocity) {
			//Fade out walking animation
            anim.Blend(animationName, 0, 0F);
		} else {
			//Fade in walking animation
            anim.Blend(animationName, 1, 0F);
			
			//Modify animation speed to match velocity
            AnimationState state = anim[animationName];
			
			float speed = relVelocity.z;
			state.speed = speed*animationSpeed;
		}
	}
}
