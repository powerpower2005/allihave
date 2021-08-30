//원형 물체 간 충돌 판정
//수학 시간에 배운 두 원과의 관계에서 배운 반지름에 따른 위치관계를 이용

#include <windows.h>

#define PI					3.14159265f			// 원주율 파이
#define VIEW_WIDTH			640					// 화면 너비 
#define VIEW_HEIGHT			480					// 화면 높이 
#define CIRCLE_VEL			5.0f				// 원형 속도 

struct F_CIRCLE {
	float			x, y;				// 원 중심 위치 
	float			r;					// 반지름 
};


F_CIRCLE			crCircleA, crCircleB;	// 원형Ａ, Ｂ


int InitCircle(void)						// 초기화
{
	crCircleA.x = 100.0f;  crCircleA.y = 100.0f;
	crCircleA.r = 80.0f;
	crCircleB.x = 300.0f;  crCircleB.y = 270.0f;
	crCircleB.r = 120.0f;

	return 0;
}


int CheckHit(F_CIRCLE* pcrCircle1, F_CIRCLE* pcrCircle2)		// 충돌 판정
{
	int				nResult = false;
	float			dx, dy;							// 위치의 차이 
	float			ar;								// 두 원들의 반지름을 더한 것 
	float			fDistSqr;

	dx = pcrCircle1->x - pcrCircle2->x;				// ⊿ｘ
	dy = pcrCircle1->y - pcrCircle2->y;				// ⊿ｙ
	fDistSqr = dx * dx + dy * dy;					// 거리의 제곱 
	ar = pcrCircle1->r + pcrCircle2->r;
	if (fDistSqr < ar * ar) {						// 제곱으로 비교 -> 어차피 루트 씌우나 안 씌우나 판정 가능
		nResult = true;
	}

	return nResult;
}


int MoveRect(void)						// 키 입력
{
	//	float			fVelocity;

		// 왼쪽 방향키
	if (GetAsyncKeyState(VK_LEFT)) {
		crCircleA.x -= CIRCLE_VEL;
		if (crCircleA.x < 0.0f) crCircleA.x = 0.0f;
	}
	// 오른쪽 방향키
	if (GetAsyncKeyState(VK_RIGHT)) {
		crCircleA.x += CIRCLE_VEL;
		if (crCircleA.x > VIEW_WIDTH) crCircleA.x = VIEW_WIDTH;
	}
	// 위 방향키
	if (GetAsyncKeyState(VK_UP)) {
		crCircleA.y -= CIRCLE_VEL;
		if (crCircleA.y < 0.0f) crCircleA.y = 0.0f;
	}
	// 아래 방향키
	if (GetAsyncKeyState(VK_DOWN)) {
		crCircleA.y += CIRCLE_VEL;
		if (crCircleA.y > VIEW_HEIGHT) crCircleA.y = VIEW_HEIGHT;
	}

	return 0;
}
