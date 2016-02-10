using UnityEngine;

namespace PC2D
{
    /// <summary>
    /// This is a very very very simple example of how an animation system could query information from the motor to set state.
    /// This can be done to explicitly play states, as is below, or send triggers, float, or bools to the animator. Most likely this
    /// will need to be written to suit your game's needs.
    /// </summary>

    public class PlatformerAnimation2D : MonoBehaviour
    {
        public float jumpRotationSpeed;
        public GameObject visualChild;

        private PlatformerMotor2D _motor;
        private Animator _animator;
        private bool _isJumping;
        private bool _currentFacingLeft;
        private Player _player;
		
		private AudioClip jump;
		AudioSource audioSource;

        // Use this for initialization
        void Start()
		{
			audioSource = GetComponent<AudioSource>();
			jump = Resources.Load("Audio/jump") as AudioClip;
            _player = GetComponent<Player>();
            _motor = GetComponent<PlatformerMotor2D>();
            _animator = visualChild.GetComponent<Animator>();
            _animator.Play("Idle");

            _motor.onJump += SetCurrentFacingLeft;
        }

        // Update is called once per frame
        void Update()
        {
            bool currentlyJumping = isJumping();
            if (_player.CurrentState == Player.State.DRINKING && !currentlyJumping)
			{
				_isJumping = false;
                visualChild.transform.rotation = Quaternion.identity;
                _animator.Play("Drink");
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Drink") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    _player.setCurrentState(Player.State.ALIVE);
                else
                    return;
            }
            else if (_player.CurrentState == Player.State.DEAD && !currentlyJumping)
			{
				_isJumping = false;
                visualChild.transform.rotation = Quaternion.identity;
                _animator.Play("Dying");
                Destroy(this);
                return;
            }
            else if (_player.CurrentState == Player.State.PICKING && !currentlyJumping)
            {
				_isJumping = false;
                visualChild.transform.rotation = Quaternion.identity;
                _animator.Play("Pickup");
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Pickup") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    _player.setCurrentState(Player.State.ALIVE);
                else
                    return;
            }
            else if (_player.CurrentState == Player.State.HIT)
            {
                _animator.Play("Hit");
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Hit") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    _player.setCurrentState(Player.State.ALIVE);
            }

            if (currentlyJumping)
            {
				if (!_isJumping)
					audioSource.PlayOneShot(jump, 0.1F);
			
                _isJumping = true;
                //                _animator.Play("Jump");

                if (_motor.velocity.x <= -0.1f)
                {
                    _currentFacingLeft = true;
                }
                else if (_motor.velocity.x >= 0.1f)
                {
                    _currentFacingLeft = false;
                }
			
			    Vector3 rotateDir = _currentFacingLeft ? Vector3.forward : Vector3.back;
                visualChild.transform.Rotate(rotateDir, jumpRotationSpeed * Time.deltaTime);
            }
            else
            {
                _isJumping = false;
                visualChild.transform.rotation = Quaternion.identity;

                if (_motor.motorState == PlatformerMotor2D.MotorState.Falling ||
                                 _motor.motorState == PlatformerMotor2D.MotorState.FallingFast)
                {
                    //                    _animator.Play("Fall");
                }
                else if (_motor.motorState == PlatformerMotor2D.MotorState.WallSliding ||
                         _motor.motorState == PlatformerMotor2D.MotorState.WallSticking)
                {
                    //                    _animator.Play("Cling");
                }
                else if (_motor.motorState == PlatformerMotor2D.MotorState.OnCorner)
                {
                    //                    _animator.Play("On Corner");
                }
                else if (_motor.motorState == PlatformerMotor2D.MotorState.Slipping)
                {
                    //                    _animator.Play("Slip");
                }
                else if (_motor.motorState == PlatformerMotor2D.MotorState.Dashing)
                {
                    //                    _animator.Play("Dash");
                }
                else if (_player.CurrentState != Player.State.HIT)
                {
                    if (_motor.velocity.sqrMagnitude >= 0.1f * 0.1f)
                    {
                        _animator.Play("Walk");
                    }
                    else
                    {
                        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")
                            && !_animator.GetCurrentAnimatorStateInfo(0).IsName("IdleAnimation"))
                            _animator.Play("Idle");
                    }
                }
            }

            // Facing
            float valueCheck = _motor.normalizedXMovement;
            //float aimingAngle = Mathf.Abs(_player.GetComponentInChildren<Weapon>().Angle);

            if (_motor.motorState == PlatformerMotor2D.MotorState.Slipping ||
                _motor.motorState == PlatformerMotor2D.MotorState.Dashing ||
                _motor.motorState == PlatformerMotor2D.MotorState.Jumping)
            {
                valueCheck = _motor.velocity.x;
            }

            if (Mathf.Abs(valueCheck) >= 0.1f)
                _currentFacingLeft = (valueCheck >= 0.1f);

            //if (aimingAngle != 0)
            //{
            //    if (aimingAngle > 90)
            //        flipSprite(false);
            //    else if (aimingAngle < 90)
            //        flipSprite(true);
            //}
            //else
            //{
            if (valueCheck >= 0.1f)
                flipSprite(_currentFacingLeft);
            else if (valueCheck <= -0.1f)
                flipSprite(_currentFacingLeft);
            //}
        }

        private void flipSprite(bool facingLeft)
        {
            if (facingLeft)
                visualChild.transform.localScale = Vector3.one;
            else
            {
                Vector3 newScale = Vector3.one;
                newScale.x = -1;
                visualChild.transform.localScale = newScale;
            }
        }

        private bool isJumping()
        {
            return _motor.motorState == PlatformerMotor2D.MotorState.Jumping ||
                        _isJumping &&
                            (_motor.motorState == PlatformerMotor2D.MotorState.Falling ||
                                         _motor.motorState == PlatformerMotor2D.MotorState.FallingFast);
        }

        private void SetCurrentFacingLeft()
        {
            _currentFacingLeft = _motor.facingLeft;
        }
    }
}
