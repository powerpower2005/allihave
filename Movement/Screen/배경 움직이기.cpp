
//다중 배경으로 설정하여
//스크린 속도를 다르게 하여 원근감을 줄 수 있음

#include <windows.h>

#define VIEW_WIDTH			640					// 화면 너비
#define VIEW_HEIGHT			480					// 화면 높이 
#define PICTURE_WIDTH1		1600			    // 배경1 너비 
#define PICTURE_WIDTH2		1280			    // 배경2 너비 
#define PICTURE_WIDTH3		873					// 배경3 너비 
#define CAMERA_VEL			10.0f				// 카메라 속도 

float		Camera_x;
float		Back_x1, Back_x2, Back_x3;


int InitBack(void)							
{
	Camera_x = VIEW_WIDTH / 2.0f;				
	Back_x1 = 0.0f;							
	Back_x2 = 0.0f;
	Back_x3 = 0.0f;

	return 0;
}


int MoveBack(void)							// 매 프레임 호출된다 
{
	// 왼쪽 방향키
	if (GetAsyncKeyState(VK_LEFT)) {
		Camera_x -= CAMERA_VEL;
		if (Camera_x < VIEW_WIDTH / 2.0f) {
			Camera_x = VIEW_WIDTH / 2.0f;
		}
	}
	// 오른쪽 방향키
	if (GetAsyncKeyState(VK_RIGHT)) {
		Camera_x += CAMERA_VEL;
		if (Camera_x > (float)(PICTURE_WIDTH1 - VIEW_WIDTH / 2.0f)) {
			Camera_x = (float)(PICTURE_WIDTH1 - VIEW_WIDTH / 2.0f);
		}
	}

	//카메라 위치, 배경의 길이에 따른 식

	//x1에서는 카메라의 위치에 따라
	Back_x1 = VIEW_WIDTH / 2.0f - Camera_x;

	//x2,x3는 카메라의 위치에 따른 x1 과 x1 배경의 길이와 자신의 길이에 따른 비율대로 움직임.
	Back_x2 = (float)(PICTURE_WIDTH2 - VIEW_WIDTH) / (PICTURE_WIDTH1 - VIEW_WIDTH) * Back_x1;
	Back_x3 = (float)(PICTURE_WIDTH3 - VIEW_WIDTH) / (PICTURE_WIDTH1 - VIEW_WIDTH) * Back_x1;

	return 0;
}

