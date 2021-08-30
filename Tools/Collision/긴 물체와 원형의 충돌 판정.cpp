

//원과 선분(긴 물체)와의 충돌 판정

//여기서 긴 물체란 기다란 직사각형 + 양 끝에 반원이 붙은 모양을 말한다.

#include <windows.h>

#define PI					3.14159265f			// 원주율 
#define VIEW_WIDTH			640					// 화면 너비
#define VIEW_HEIGHT			480					// 화면 높이 
#define CIRCLE_VEL			5.0f				// 원의 빠르기 


struct F_CIRCLE {
	float			x, y;				// 중심 위치 
	float			r;					// 반지름 
};


//벡터로 표현
struct F_RECT_CIRCLE {
	float			x, y;				// 시점위치 
	float			vx, vy;				// 벡터 
	float			r;					// 충돌 크기 
};


F_CIRCLE			crCircleA;			// 원형A
F_RECT_CIRCLE		rcRectCircleB;		// 사각형+원B (양 끝 반원)


int InitShapes(void)					// 초기화
{
	crCircleA.x = 100.0f;  crCircleA.y = 100.0f;
	crCircleA.r = 70.0f;
	rcRectCircleB.x = 250.0f;  rcRectCircleB.y = 200.0f;
	rcRectCircleB.vx = 200.0f;  rcRectCircleB.vy = 100.0f;
	rcRectCircleB.r = 60.0f;

	return 0;
}


int CheckHit(F_CIRCLE* pcrCircle, F_RECT_CIRCLE* prcRectCircle)		// 충돌 판정
{
	int				nResult = false;
	float			dx, dy;							// 위치의 차이, 델타
	float			t;								
	float			mx, my;							// 최소 거리가 되는 좌표 
	float			ar;								// 반지름을 더한 것 (긴 물체에서는 반원 또는 사각형의 절반) 

	float			fDistSqr;

	dx = pcrCircle->x - prcRectCircle->x;				// ⊿ｘ
	dy = pcrCircle->y - prcRectCircle->y;				// ⊿ｙ

	//t로 미분해서 최솟값을 가지는 t를 구함
	t = (prcRectCircle->vx * dx + prcRectCircle->vy * dy) /
		(prcRectCircle->vx * prcRectCircle->vx + prcRectCircle->vy * prcRectCircle->vy);
	if (t < 0.0f) t = 0.0f;						// t의 하한
	if (t > 1.0f) t = 1.0f;						// t의 상한 



	//t를 구해서 최소 위치가 되는 좌표를 이용해서 반지름과의 관계 이용
	mx = prcRectCircle->vx * t + prcRectCircle->x;	// 최소 위치가 되는 좌표
	my = prcRectCircle->vy * t + prcRectCircle->y;


	fDistSqr = (mx - pcrCircle->x) * (mx - pcrCircle->x) +
		(my - pcrCircle->y) * (my - pcrCircle->y);	// 거리의 제곱 
	ar = pcrCircle->r + prcRectCircle->r;
	if (fDistSqr < ar * ar) {						// 제곱인 채로 비교 
		nResult = true;
	}

	return nResult;
}


int MoveRect(void)						// 키 입력
{
	//	float			fVelocity;

		// 왼쪽
	if (GetAsyncKeyState(VK_LEFT)) {
		crCircleA.x -= CIRCLE_VEL;
		if (crCircleA.x < 0.0f) crCircleA.x = 0.0f;
	}
	// 오른쪽
	if (GetAsyncKeyState(VK_RIGHT)) {
		crCircleA.x += CIRCLE_VEL;
		if (crCircleA.x > VIEW_WIDTH) crCircleA.x = VIEW_WIDTH;
	}
	// 위
	if (GetAsyncKeyState(VK_UP)) {
		crCircleA.y -= CIRCLE_VEL;
		if (crCircleA.y < 0.0f) crCircleA.y = 0.0f;
	}
	// 아래
	if (GetAsyncKeyState(VK_DOWN)) {
		crCircleA.y += CIRCLE_VEL;
		if (crCircleA.y > VIEW_HEIGHT) crCircleA.y = VIEW_HEIGHT;
	}

	return 0;
}

