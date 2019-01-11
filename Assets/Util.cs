using UnityEngine;
using System.Collections;

public class Util : MonoBehaviour {
	public static Color orange = new Color(1F, 0.5F, 0F);
	
	public static void DrawRigidbodyRay(Rigidbody rigidBody, Vector3 v1, Vector3 v2){
		Debug.DrawRay (v1 + rigidBody.velocity * Time.fixedDeltaTime, v2);
	}
		
	public static Vector3 RigidBodyPosition(Rigidbody rigidBody){
		return rigidBody.transform.position + rigidBody.velocity * Time.fixedDeltaTime;
    }

    public static void DrawRigidbodyRay(Rigidbody rigidBody, Vector3 start, Vector3 dir, Color color)
    {
        Debug.DrawRay(start + rigidBody.velocity * Time.fixedDeltaTime, dir, color);
    }


    public static void DrawRigidbodyRay(Rigidbody2D rigidBody, Vector2 v1, Vector2 v2)
    {
        Debug.DrawRay(v1 + rigidBody.velocity * Time.fixedDeltaTime, v2);
    }

    public static Vector2 RigidBodyPosition(Rigidbody2D rigidBody)
    {
        return (Vector2)rigidBody.transform.position + rigidBody.velocity * Time.fixedDeltaTime;
    }

    public static void DrawRigidbodyRay(Rigidbody2D rigidBody, Vector2 start, Vector2 dir, Color color)
    {
        Debug.DrawRay(start + rigidBody.velocity * Time.fixedDeltaTime, dir, color);
    }


    public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal) {
		Vector3 perpVector;
		float angle;

		//Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
		perpVector = Vector3.Cross(normal, referenceVector);

		//Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
		angle = Vector3.Angle(referenceVector, otherVector);
		angle *= Mathf.Sign(Vector3.Dot(perpVector, otherVector));

		return angle;
	}

	private IEnumerable WaitForAnimation(Animation animation){
		do {
			yield return null;
		} while(animation.isPlaying);
	}

	public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
		Vector3 direction = point - pivot; //get direction relative to pivot
		direction = Quaternion.Euler (angles) * direction; // rotate
		return direction + pivot; // calculate rotated point
	}

	public static float GetAxis(string axis){
		return Input.GetAxis (axis);
	}
	public static bool GetButton(string button){
		bool pressed = Input.GetButton(button);
        if (!pressed)
        {
            pressed = Input.GetAxis(button) != 0;
        }
        return pressed;
    }
	public static bool GetButtonDown(string button){
		return Input.GetButtonDown (button);
	}
	public static bool GetButtonUp(string button){
		return Input.GetButtonUp (button);
	}

	public static bool InLayerMask(int layer, LayerMask layermask) {
		return layermask == (layermask | (1 << layer));
	}

    public static float ConvertScale(float oldMin, float oldMax, float newMin, float newMax, float value)
    {
        value = Mathf.Clamp(value, oldMin, oldMax);
        return (((value - oldMin) * (newMax - newMin)) / (oldMax - oldMin)) + newMin;
    }

    public static string FormatNumber(int number)
    {
        return string.Format("{0:n0}", number);
    }

    public static int FormatPercentage(float number)
    {
        return Mathf.RoundToInt(number * 100);
    }

    public static bool CanSpawn(Vector3 position, float radius, float height, LayerMask layer)
    {
        Collider[] colliders = Physics.OverlapCapsule(position - Vector3.up * height, position + Vector3.up * height, radius, layer);
        return colliders.Length == 0;
    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    public static int GetRandomWeightedIndex(int[] weights)
    {
        // Get the total sum of all the weights.
        int weightSum = 0;
        for (int i = 0; i < weights.Length; ++i)
        {
            weightSum += weights[i];
        }

        // Step through all the possibilities, one by one, checking to see if each one is selected.
        int index = 0;
        int lastIndex = weights.Length - 1;
        while (index < lastIndex)
        {
            // Do a probability check with a likelihood of weights[index] / weightSum.
            if (Random.Range(0, weightSum) < weights[index])
            {
                return index;
            }

            // Remove the last item from the sum of total untested weights and try again.
            weightSum -= weights[index++];
        }

        // No other item was selected, so return very last index.
        return index;
    }

    public static Vector3 TouchToPointOnPlane(Vector3 touchPosition, Vector3 planePosition, Vector3 planeNormal)
    {
        // create ray from the camera and passing through the touch position:
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        // create a logical plane at this object's position
        // and perpendicular to world Y:
        Plane plane = new Plane(planeNormal, planePosition);
        float distance = 0; // this will return the distance from the camera
        if (plane.Raycast(ray, out distance))
        { // if plane hit...
            Vector3 pos = ray.GetPoint(distance); // get the point
                                                  // pos has the position in the plane you've touched
            return pos;
        }
        return Vector3.zero;
    }

    public static float GetAngleByDeviceAxis(Vector3 axis)
    {
        Quaternion deviceRotation = DeviceRotation.Get();
        Quaternion eliminationOfOthers = Quaternion.Inverse(
            Quaternion.FromToRotation(axis, deviceRotation * axis)
        );
        Vector3 filteredEuler = (eliminationOfOthers * deviceRotation).eulerAngles;

        float result = filteredEuler.z;
        if (axis == Vector3.up)
        {
            result = filteredEuler.y;
        }
        if (axis == Vector3.right)
        {
            // incorporate different euler representations.
            result = (filteredEuler.y > 90 && filteredEuler.y < 270) ? 180 - filteredEuler.x : filteredEuler.x;
        }
        return result;
    }

    public static void ResetGyro()
    {
        Input.gyro.enabled = false;
        Input.gyro.enabled = true;
    }

    public static Quaternion GyroToUnity()
    {
        Quaternion q = Input.gyro.attitude;
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    public static T[] Randomize<T>(T[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Length);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }

        return list;
    }

    public static T RandomEnumValue<T>()
    {
        var values = System.Enum.GetValues(typeof(T));
        int random = Random.Range(0, values.Length);
        return (T)values.GetValue(random);
    }

    public static T RandomEnumValue<T>(T ignoredType)
    {
        var values = System.Enum.GetValues(typeof(T));
        T value;
        do {
            int random = Random.Range(0, values.Length);
            value = (T)values.GetValue(random);
        } while (value.Equals(ignoredType));
        return value;
    }
}
