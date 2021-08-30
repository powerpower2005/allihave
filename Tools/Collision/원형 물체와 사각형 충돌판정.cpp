// 원형과 사각형 충돌판정

#include <windows.h>

#define PI					3.14159265f		// 원주율 
#define VIEW_WIDTH			640				// 화면 너비
#define VIEW_HEIGHT			480				// 화면 높이 
#define CIRCLE_VEL			5.0f			// 원형 속도 


struct F_RECT {
	float			fLeft, fTop;			// 좌, 상 
	float			fRight, fBottom;		// 우, 하 
};

struct F_CIRCLE {
	float			x, y;					// 중심위치 
	float			r;						// 반경 
};


F_RECT				rcRect_B;				// 사각형B
F_CIRCLE			crCircleA;				// 원형A


int InitCircle(void)						// 초기화
{
	crCircleA.x = 100.0f;  crCircleA.y = 100.0f;
	crCircleA.r = 80.0f;
	rcRect_B.fLeft = 200.0f;  rcRect_B.fTop = 150.0f;
	rcRect_B.fRight = 500.0f;  rcRect_B.fBottom = 350.0f;

	return 0;
}


float DistanceSqr(float x1, float y1, float x2, float y2)	// 거리의 제곱 계산 -> 사각형의 좌표와 원의 반지름을 이용할 예정
{
	float			dx, dy;							// 위치의 차이 

	dx = x2 - x1;									// ⊿ｘ
	dy = y2 - y1;									// ⊿ｙ

	return dx * dx + dy * dy;
}


//원을 사각형 둘레를 따라 돌려서 자취를 남긴다고 생각해보자.


int CheckHit(F_RECT* prcRect1, F_CIRCLE* pcrCircle2)	// 충돌 판정
{
	int				nResult = false;
	float			ar;								// 원의 반경 

	// 큰 장방형 체크 -> 각 방향별로 r만큼 확장한 사각형에 원이 안쪽으로 들어와있다면
	if ((pcrCircle2->x > prcRect1->fLeft - pcrCircle2->r) &&
		(pcrCircle2->x < prcRect1->fRight + pcrCircle2->r) &&
		(pcrCircle2->y > prcRect1->fTop - pcrCircle2->r) &&
		(pcrCircle2->y < prcRect1->fBottom + pcrCircle2->r))
	{
		nResult = true;
		ar = pcrCircle2->r;
		// 왼쪽 끝 체크 
		if (pcrCircle2->x < prcRect1->fLeft) {
			// 좌측상단 모서리 체크 
			if ((pcrCircle2->y < prcRect1->fTop))
			{
				if ((DistanceSqr(prcRect1->fLeft, prcRect1->fTop,
					pcrCircle2->x, pcrCircle2->y) >= ar * ar)) {
					nResult = false;
				}
			}
			else {
				// 좌측하단 모서리 체크 
				if ((pcrCircle2->y > prcRect1->fBottom))
				{
					if ((DistanceSqr(prcRect1->fLeft, prcRect1->fBottom,
						pcrCircle2->x, pcrCircle2->y) >= ar * ar)) {
						nResult = false;
					}
				}
			}
		}
		else {
			// 오른쪽 끝 체크 
			if (pcrCircle2->x > prcRect1->fRight) {
				// 우측 상단 모서리 체크 
				if ((pcrCircle2->y < prcRect1->fTop))
				{
					if ((DistanceSqr(prcRect1->fRight, prcRect1->fTop,
						pcrCircle2->x, pcrCircle2->y) >= ar * ar)) {
						nResult = false;
					}
				}
				else {
					// 좌측 하단 모서리 체크 
					if ((pcrCircle2->y > prcRect1->fBottom))
					{
						if ((DistanceSqr(prcRect1->fRight, prcRect1->fBottom,
							pcrCircle2->x, pcrCircle2->y) >= ar * ar)) {
							nResult = false;
						}
					}
				}
			}
		}
	}

	return nResult;
}


int MoveCircle(void)						// 키 입력
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
	// 위로
	if (GetAsyncKeyState(VK_UP)) {
		crCircleA.y -= CIRCLE_VEL;
		if (crCircleA.y < 0.0f) crCircleA.y = 0.0f;
	}
	// 아래로
	if (GetAsyncKeyState(VK_DOWN)) {
		crCircleA.y += CIRCLE_VEL;
		if (crCircleA.y > VIEW_HEIGHT) crCircleA.y = VIEW_HEIGHT;
	}

	return 0;
}
