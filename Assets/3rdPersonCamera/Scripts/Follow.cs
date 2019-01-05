using UnityEngine;
using System.Collections;

namespace ThirdPersonCamera
{
	public enum FOLLOW_TYPE {
		TIGHT, LOOSE, EXACT, NONE
	}
    public class Follow : MonoBehaviour
    {
		public bool adjustFollowTypeWithSpeed = false;
		public float minTightFollowSpeed = 5f;
		public FOLLOW_TYPE followType = FOLLOW_TYPE.TIGHT;
        public bool alignOnSlopes = true;

		public float minTightAngle = 1f;
		public float minAngleToTurn = 0.1f;
        public float rotationSpeed = 1.0f;
        public float rotationSpeedSlopes = 0.5f;

        public Vector3 tiltVector;
        public LayerMask layerMask;

        private Vector3 prevPosition;

        public CameraController cc;
		Rigidbody targetRb;

        void Start()
        {
//            cc = GetComponent<CameraController>();
			targetRb = cc.target.GetComponent<Rigidbody> ();
        }

		Vector3 prevTargetPosition;
		Vector3 movementDirection;

        void FixedUpdate()
		{
			movementDirection = transform.position - prevTargetPosition;
//			movementDirection = targetRb.velocity;
			prevTargetPosition = transform.position;

			RaycastHit raycastHit;
			Vector3 upVector = Vector3.up;
			Quaternion toRotation;

//			if (adjustFollowTypeWithSpeed && targetRb != null) {
			if (adjustFollowTypeWithSpeed) {
				if (movementDirection.magnitude > minTightFollowSpeed) {
					followType = FOLLOW_TYPE.TIGHT;
				} else {
					followType = FOLLOW_TYPE.LOOSE;
				}
			}

			switch(followType) {
			case FOLLOW_TYPE.TIGHT:
				cc.slerpPosition = true;

				toRotation = Quaternion.LookRotation (cc.target.forward + tiltVector, upVector);

				toRotation = Quaternion.Slerp (cc.transform.rotation, toRotation, Time.fixedDeltaTime * rotationSpeed);
				cc.transform.rotation = toRotation;

				break;

			case FOLLOW_TYPE.LOOSE:
				cc.slerpPosition = true;

				Vector3 targetDirection = cc.target.transform.position - cc.transform.position;
				if (movementDirection.magnitude > 0.1f) {
					targetDirection += Vector3.ClampMagnitude(movementDirection, 1);
				}
				targetDirection = Vector3.ProjectOnPlane (targetDirection, cc.target.transform.up);

				targetDirection += tiltVector;

				toRotation = Quaternion.LookRotation (targetDirection, upVector);

				if (alignOnSlopes) {
					if (Physics.Raycast (cc.target.transform.position, Vector3.down, out raycastHit, 25.0f, layerMask)) { // if the range of 15.0 is not enough, increase the value
						upVector = raycastHit.normal;
					}

					float angle = AngleSigned (Vector3.up, upVector, cc.target.transform.right);

					toRotation = Quaternion.Slerp (toRotation, toRotation * Quaternion.AngleAxis (angle, Vector3.right), Time.fixedDeltaTime * rotationSpeedSlopes);
				}

				toRotation = Quaternion.Slerp (cc.transform.rotation, toRotation, Time.fixedDeltaTime * rotationSpeed);
				if (Quaternion.Angle (cc.transform.rotation, toRotation) > minAngleToTurn) {
					cc.transform.rotation = toRotation;
				}

				break;
			}
        }

        public float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
		}

		public void SetFollowType(FOLLOW_TYPE newFollowType) {
			adjustFollowTypeWithSpeed = false;
			followType = newFollowType;
		}

		public void UnsetFollowType() {
			adjustFollowTypeWithSpeed = true;
		}
    }
}