#include <windows.h>

#define VIEW_WIDTH			640					// 화면 너비 
#define VIEW_HEIGHT			480					// 화면 높이 
#define CHAR_WIDTH			64					// 캐릭터 너비
#define CHAR_HEIGHT			64					// 캐릭터 높이
#define PLAYER_VEL			10.0f				// 플레이어 속도 

float			x, y;							// 위치 


int InitCharacter(void)						// 처음에 한 번만 호출된다 
{
	x = (float)(VIEW_WIDTH - CHAR_WIDTH) / 2.0f;
	y = (float)(VIEW_HEIGHT - CHAR_HEIGHT) / 2.0f;

	return 0;
}

/*관성을 구현하려면

해당 버튼을 눌렀을 때, 1이 되는 변수 생성

LB, UB, DB, RB

position은 해당 변수 * 플레이어 속도로 변하며
ex) x += RB * PLAYER_VEL

버튼을 뗀다면 매 프레임마다 RB의 값을 0이 될 때까지 깎아주어
버튼을 누르지 않아도 X가 일정 시간만큼은 움직이게 구현.

ex) if(!VK_RIGHT && RB != 0) RB -= 0.2f; 


*/


#define ROOT2			1.41421f // 계산안하고 그냥 가정
int MoveCharacter(void)						
{
	short		bLeftKey, bRightKey;
	short		bUpKey, bDownKey;

	bLeftKey = GetAsyncKeyState(VK_LEFT);
	bRightKey = GetAsyncKeyState(VK_RIGHT);
	bUpKey = GetAsyncKeyState(VK_UP);
	bDownKey = GetAsyncKeyState(VK_DOWN);
	// 왼쪽 방향키
	if (bLeftKey) {
		if (bUpKey || bDownKey) {
			x -= PLAYER_VEL / ROOT2;
		}
		else {
			x -= PLAYER_VEL;
		}
		if (x < 0) {
			x = 0;
		}
	}
	// 오른쪽 방향키
	if (bRightKey) {
		if (bUpKey || bDownKey) {
			x += PLAYER_VEL / ROOT2;
		}
		else {
			x += PLAYER_VEL;
		}
		if (x >= (float)(VIEW_WIDTH - CHAR_WIDTH)) {
			x = (float)(VIEW_WIDTH - CHAR_WIDTH);
		}
	}
	// 위쪽 방향키
	if (bUpKey) {
		if (bLeftKey || bRightKey) {
			y -= PLAYER_VEL / ROOT2;
		}
		else {
			y -= PLAYER_VEL;
		}
		if (y < 0) {
			y = 0;
		}
	}
	// 아래쪽 방향키
	if (bDownKey) {
		if (bLeftKey || bRightKey) {
			y += PLAYER_VEL / ROOT2;
		}
		else {
			y += PLAYER_VEL;
		}
		if (y >= (float)(VIEW_HEIGHT - CHAR_HEIGHT)) {
			y = (float)(VIEW_HEIGHT - CHAR_HEIGHT);
		}
	}

	return 0;
}
