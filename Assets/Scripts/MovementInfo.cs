using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovementInfo
{
	// Input Related
	[ReadOnly]
	public Vector2 MovementInput = Vector3.zero;

	// General Movement
	[ReadOnly]
	public bool m_IsGrounded;
	[ReadOnly]
	public bool m_IsDashGrounded; // The difference between this and regular grounded is that this grounds a bit earlier than when the player
	[ReadOnly]					  // touches the floor. This is to give a bit of coyote time for landing with the dashes.
	public Vector3 m_CacheMovDirection;
	[ReadOnly]
	public float m_CurrentMoveSpeed = 0;
	[ReadOnly]
	public bool m_IsMoving = false;
	[ReadOnly]
	public Vector3 m_DashDir;
	
	// Jump
	[ReadOnly]
	public bool m_HasDoubleJumped = false;
	[ReadOnly]
	public bool m_HasJumped = false;
	[ReadOnly]
	public float m_JumpCounter = 0;

	// Sliding
	[ReadOnly]
	public bool m_IsSliding = false;
	[ReadOnly]
	public Vector3 m_SlideDir = Vector3.zero;
	[ReadOnly]
	public Vector3 m_CacheSlideMove = Vector3.zero;
	[ReadOnly]
	public float m_SlideCounter = 0;
	[ReadOnly]
	public float m_OriginalColliderHeight; // Slides shrink the capsule so here is where we store the original values so we can raise the player back up.
	[ReadOnly]
	public Vector3 m_OriginalColliderCenter;

	// Rigidbody Controller Hackery
	// Temporary ground normal thing.
	[ReadOnly]
	public Vector3 m_GroundNormal = Vector3.zero;
	[ReadOnly]
	public Vector3 m_ModifiedRight = Vector3.zero;
	[ReadOnly]
	public Vector3 m_ModifiedForward = Vector3.zero;
	[ReadOnly]
	public Vector3 m_MoveDirection = Vector3.zero;


	//public bool IsGrounded { get; private set; }
	//public Vector3 CacheMovDir { get; private set; }

	//private bool m_HasDoubleJumped = false;

	//private bool m_HasJumped = false;

	//public bool IsSliding { get; private set; }
	//public Vector3 SlideDir { get; private set; }

	//private Vector3 m_CacheSlideMove = Vector3.zero;

	//public float SlideCounter { get; private set; }

	//// Exposed variables for debugging.
	//[ReadOnly]
	//public float m_CurrentMoveSpeed;
	//[ReadOnly]
	//public bool m_IsMoving;

	//// temporary jump deactivate cooldown. to prevent m_IsJumping from being deactivated as soon as you jump.
	//float m_JumpCounter = 0;
}
