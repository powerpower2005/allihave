// 캐릭터의 위치에 따라 배경 스크롤링
#include <windows.h>
#include <math.h>

#define VIEW_WIDTH			640				// 화면 너비 
#define VIEW_HEIGHT			480				// 화면 높이 
#define PICTURE_WIDTH		1280			// 배경 너비 
#define CHARACTER_WIDTH			128				// 캐릭터 너비 
#define CHARACTER_VEL			10.0f			// 캐릭터 빠르기 
#define SCROLL_DIF			150.0f			// 스크롤 시작 너비 




float		Camera_x;
float		Back_x;
float		Character_x, Character_sx;			// 화면 및 필드 상의 캐릭터 위치 


int InitBack(void)							// 초기화
{
	Camera_x = VIEW_WIDTH / 2.0f;			// 카메라 초기 위치
	Back_x = 0.0f;							// 배경 초기 위치
	Character_sx = VIEW_WIDTH / 2.0f;
	Character_x = VIEW_WIDTH / 2.0f;

	return 0;
}


int MoveCharacter(void)							
{
	// 왼쪽 방향키
	if (GetAsyncKeyState(VK_LEFT)) {
		Character_sx -= CHARACTER_VEL;
		//한계
		if (Character_sx < 0.0f) {
			Character_sx = 0.0f;
		}
	}
	// 오른쪽 방향키
	if (GetAsyncKeyState(VK_RIGHT)) {
		Chara_sx += CHARACTER_VEL;
		//한계
		if (Character_sx > (float)PICTURE_WIDTH) {
			Character_sx = (float)PICTURE_WIDTH;
		}
	}


	// 만약 캐릭터의 위치에 카메라를 고정시켜버린다면 점프 같은 행동을 했을 시 바닥이 안 보이는 경우가 있음
	// 캐릭터를 따라가되 어느 정도 여유를 두고 따라가야 한다. (SCROLL_DIF가 그 역할을 함)


	if (Camera_x < Character_sx - SCROLL_DIF) {		// 카메라가 왼쪽으로 접근하는지 체크 -> 해당 캐릭터가 SCROLL_DIF보다 멀리 있을 경우 카메라 위치가 바뀜
		Camera_x = Character_sx - SCROLL_DIF;
	}
	if (Camera_x > Character_sx + SCROLL_DIF) {		// 카메라가 오른쪽으로 접근하는지 체크 -> 해당 캐릭터가 SCROLL_DIF보다 멀리 있을 경우 카메라 위치가 바뀜
		Camera_x = Character_sx + SCROLL_DIF;
	}


	//카메라 한계

	if (Camera_x < VIEW_WIDTH / 2.0f) {						// 카메라 왼쪽 이동 한계
		Camera_x = VIEW_WIDTH / 2.0f;
	}
	if (Camera_x > PICTURE_WIDTH - VIEW_WIDTH / 2.0f) {		// 카메라 오른쪽 이동 한계
		Camera_x = PICTURE_WIDTH - VIEW_WIDTH / 2.0f;
	}


	Character_x = Character_sx - Camera_x + VIEW_WIDTH / 2.0f - CHARACTER_WIDTH / 2.0f;
	Back_x = VIEW_WIDTH / 2.0f - Camera_x;

	return 0;
}

