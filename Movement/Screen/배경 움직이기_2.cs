using UnityEngine;
using System.Collections;
/// <summary>
/// 부분의 이미지를 계속 생성하는 것이 아닌 카메라에서 사라지면 뒤에 다시 배치하여 무한 배경을 만듦
/// </summary>
public class FloorControl : MonoBehaviour {

	// 카메라
	private GameObject main_camera = null;

	// 초기 위치
	private Vector3	initial_position;

	// 바닥의 폭(X방향) -> 이미지의 너비
	public	const float	WIDTH = 10.0f*4.0f;

	// 이용할 배경의 수
	public const int		MODEL_NUM = 3;

	// ================================================================ //

	void	Start() 
	{
		// 각종 초기화 값, 인스턴스 할당
		this.main_camera = GameObject.FindGameObjectWithTag("MainCamera");
		this.initial_position = this.transform.position;
		this.GetComponent<Renderer>().enabled = SceneControl.IS_DRAW_DEBUG_FLOOR_MODEL;

	}
	
	void	Update()
	{
		// 지속적으로 바닥의 이동이 반복되도록 설정한다.

#if true
		// 바닥이 화면 밖으로 나가게 되면 플레이어의 앞이나 뒤쪽으로 이동한다.


		// 배경 전체(모델 전체가 배치된 경우)의 폭 -> 이미지의 너비 * 이미지의 개수 
		float	total_width = FloorControl.WIDTH*FloorControl.MODEL_NUM;

		// 배경 위치
		Vector3	floor_position = this.transform.position;

		// 카메라 위치
		Vector3	camera_position = this.main_camera.transform.position;

		// 카메라는 플레이어에 고정되어있다고 한다면
		// 플레이어가 해당 배경의 중간을 넘어서 해당 floor의 중간지점을 지난다면
		if(floor_position.x + total_width/2.0f < camera_position.x) {
			// 배경이 오른쪽으로 이동.
			floor_position.x += total_width;
			this.transform.position = floor_position;
		}
		//반대로 아니라면
		if(camera_position.x < floor_position.x - total_width/2.0f) {

			//배경이 왼쪽으로 이동
			floor_position.x -= total_width;
			this.transform.position = floor_position;
		}
#else
		// 플레이어 이동 시 대응 방법

		float		total_width = FloorControl.WIDTH*FloorControl.MODEL_NUM;

		Vector3		camera_position = this.main_camera.transform.position;

		//플레이어의 이동거리라고 생각하면 된다. -> camera_position.x는 갱신되지만 this.initial_position.x는 그대로
		float		dist = camera_position.x - this.initial_position.x;

		// 모델은 total_width 의 정수의 배수 위치에 표시된다
		// 초기 위치의 거리를 배경 전체의 너비로 나누고 반올림한다 -> 이동거리에 따라 해당 배경이 위치할 공간을 정해줌
		// 부분 배경들의 폭도 일정해야 가능한 방법
		int	n = Mathf.RoundToInt(dist/total_width);

		Vector3	position = this.initial_position;

		position.x += n*total_width;

		this.transform.position = position;
#endif
	}
}
