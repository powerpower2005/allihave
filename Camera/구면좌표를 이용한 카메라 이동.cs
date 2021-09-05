using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class SphericalCamera : MonoBehaviour
{

	//확대 또는 회전 속도
	public float rotateSpeed = 1f;
	public float scrollSpeed = 200f;

	public Transform pivot; // 구면좌표의 원점을 말하면서 바라보는 대상 ( ex 캐릭터를 의미)


	//해당 구면좌표계를 카메라에 적용하기 위한 성분
	[System.Serializable]
	public class SphericalCoordinates
	{
		public float _radius, _azimuth, _elevation; // r,theta, pi 순서


		//Clamp 함수의 사용으로 사잇값으로 고정시킴
		public float radius
		{
			get { return _radius; }
			private set
			{
				_radius = Mathf.Clamp(value, _minRadius, _maxRadius);
			}
		}

		public float azimuth
		{
			get { return _azimuth; }
			private set
			{
				_azimuth = Mathf.Repeat(value, _maxAzimuth - _minAzimuth);
			}
		}

		public float elevation
		{
			get { return _elevation; }
			private set
			{
				_elevation = Mathf.Clamp(value, _minElevation, _maxElevation);
			}
		}

		//너무 가까워도 너무 멀어도 안됨
		public float _minRadius = 3f;
		public float _maxRadius = 20f;


		// 0부터 360이 가장 이상적
		public float minAzimuth = 0f;
		private float _minAzimuth;

		public float maxAzimuth = 360f;
		private float _maxAzimuth;

		public float minElevation = 0f;
		private float _minElevation;

		public float maxElevation = 90f;
		private float _maxElevation;

		public SphericalCoordinates() { }


		//위 클래스의 생성자 (직교좌표를 구면좌표계로 바꾸는 것임)
		public SphericalCoordinates(Vector3 cartesianCoordinate)
		{

			//degree를 radian으로 바꿔주는 작업이 필요함.
			_minAzimuth = Mathf.Deg2Rad * minAzimuth;
			_maxAzimuth = Mathf.Deg2Rad * maxAzimuth;

			_minElevation = Mathf.Deg2Rad * minElevation;
			_maxElevation = Mathf.Deg2Rad * maxElevation;

			//해당 거리로 바로 radius 구할 수 있음, 카메라와 원점의 거리, 나머지는 역삼각함수 이용함
			radius = cartesianCoordinate.magnitude;
			azimuth = Mathf.Atan2(cartesianCoordinate.z, cartesianCoordinate.x);
			elevation = Mathf.Asin(cartesianCoordinate.y / radius);
		}


		//구면좌표계에서 직교좌표계로 변환하는 것임.

		public Vector3 toCartesian
		{
			get
			{
				//x,z 평면에 수직으로 내렸을 때 원점과의 거리를 말함 : t
				float t = radius * Mathf.Cos(elevation);
				return new Vector3(t * Mathf.Cos(azimuth), radius * Mathf.Sin(elevation), t * Mathf.Sin(azimuth));
			}
		}

		//해당 수치만큼 theta와 pi를 변환
		public SphericalCoordinates Rotate(float newAzimuth, float newElevation)
		{
			azimuth += newAzimuth;
			elevation += newElevation;
			return this;
		}

		//해당 수치만큼 radius를 변환
		public SphericalCoordinates TranslateRadius(float x)
		{
			radius += x;
			return this;
		}
	}

	//객체를 담는 변수 생성
	public SphericalCoordinates sphericalCoordinates;



	// 객체 초기화
	void Start()
	{
		sphericalCoordinates = new SphericalCoordinates(transform.position);
		transform.position = sphericalCoordinates.toCartesian + pivot.position;
	}


	// 업데이트로 카메라 위치바꿈.
	void Update()
	{
		float kh, kv, mh, mv, h, v;
		//키보드 입력(방향키)
		kh = Input.GetAxis("Horizontal");
		kv = Input.GetAxis("Vertical");

		//마우스 입력
		bool anyMouseButton = Input.GetMouseButton(0) | Input.GetMouseButton(1) | Input.GetMouseButton(2);
		mh = anyMouseButton ? Input.GetAxis("Mouse X") : 0f;
		mv = anyMouseButton ? Input.GetAxis("Mouse Y") : 0f;

		h = kh * kh > mh * mh ? kh : mh;
		v = kv * kv > mv * mv ? kv : mv;

		//회전에 대해 
		if (h * h > Mathf.Epsilon || v * v > Mathf.Epsilon)
		{
			transform.position
				= sphericalCoordinates.Rotate(h * rotateSpeed * Time.deltaTime, v * rotateSpeed * Time.deltaTime).toCartesian + pivot.position;
		}


		//마우스 휠의 입력 (휠을 아래로 돌리면 음수, 휠을 위로 돌리면 양수)
		float sw = -Input.GetAxis("Mouse ScrollWheel");


		//부동소수형 변수가 0과 다른 값중에, 가질 수 있는 가장 작은 값이다.
		//실제로 증명할 때 앱실론은 작은 수라고 가정하는데 그거와 마찬가지다.
		//float 은 == 연산자로 비교하면 부정확하기 떄문임.
		//0은 아니면서 가장 작은값 보다 커야함. 
		if (sw * sw > Mathf.Epsilon)
		{
			transform.position = sphericalCoordinates.TranslateRadius(sw * Time.deltaTime * scrollSpeed).toCartesian + pivot.position;
		}

		//카메라는 pivot position을 바라보게 된다. 방향벡터라고 이해하면 편함. 
		//해당 위치에서 입력된 위치를 바라봄.
		transform.LookAt(pivot.position);
	}
}