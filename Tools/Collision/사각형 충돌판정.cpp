//사각형 간의 충돌 판정


#include <windows.h>

#define VIEW_WIDTH			640					// 화면 너비
#define VIEW_HEIGHT			480					// 화면 높이 
#define RECT_VEL			5.0f				// 사각형 빠르기 

int			x;
int			v;


//사각형 정의
struct F_RECT {
	float			fLeft, fTop;				// 좌, 상  
	float			fRight, fBottom;			// 우, 하 
};


F_RECT			rcRect_A, rcRect_B;				// 사각 Ａ、Ｂ




int InitRect(void)						// 초기화
{
	rcRect_A.fLeft = 100.0f;  rcRect_A.fTop = 100.0f;
	rcRect_A.fRight = 250.0f;  rcRect_A.fBottom = 200.0f;
	rcRect_B.fLeft = 300.0f;  rcRect_B.fTop = 150.0f;
	rcRect_B.fRight = 400.0f;  rcRect_B.fBottom = 300.0f;

	return 0;
}


int CheckHit(F_RECT* prcRect1, F_RECT* prcRect2)		// 충돌 판정
{
	int				nResult = false;

	//좌표로 충돌 판정
	//( 사각형 A의 오른쪽 부분이 사각형 B의 왼쪽부분보다 오른쪽에 있고 && 사각형 A의 왼쪽 부분이 사각형 B의 오른쪽 부분보다는 왼쪽에 있음)
	// 반대로 생각하면 A의 오른쪽 부분이 B의 왼쪽 부분보다 왼쪽에 있다면 당연히 충돌 x || 사각형 B의 오른쪽 부분이 A의 왼쪽 부분보다 작으면 당연히 충돌 x
	if ((prcRect1->fRight > prcRect2->fLeft) &&
		(prcRect1->fLeft < prcRect2->fRight))
	{

		//horizon을 검사했으면 vertical 검사 위와 동일
		if ((prcRect1->fBottom > prcRect2->fTop) &&
			(prcRect1->fTop < prcRect2->fBottom))
		{
			nResult = true;
		}
	}

	return nResult;
}


int MoveRect(void)						// 키 입력 사각형 이동 
{
	float			fVelocity;

	// 왼쪽 방향키
	if (GetAsyncKeyState(VK_LEFT)) {
		fVelocity = rcRect_A.fLeft;
		if (fVelocity > RECT_VEL) fVelocity = RECT_VEL;
		rcRect_A.fLeft -= fVelocity;
		rcRect_A.fRight -= fVelocity;
	}
	// 오른쪽 방향키
	if (GetAsyncKeyState(VK_RIGHT)) {
		fVelocity = VIEW_WIDTH - rcRect_A.fRight;
		if (fVelocity > RECT_VEL) fVelocity = RECT_VEL;
		rcRect_A.fLeft += fVelocity;
		rcRect_A.fRight += fVelocity;
	}
	// 위쪽 방향키
	if (GetAsyncKeyState(VK_UP)) {
		fVelocity = rcRect_A.fTop;
		if (fVelocity > RECT_VEL) fVelocity = RECT_VEL;
		rcRect_A.fTop -= fVelocity;
		rcRect_A.fBottom -= fVelocity;
	}
	// 아래쪽 방향키
	if (GetAsyncKeyState(VK_DOWN)) {
		fVelocity = VIEW_HEIGHT - rcRect_A.fBottom;
		if (fVelocity > RECT_VEL) fVelocity = RECT_VEL;
		rcRect_A.fTop += fVelocity;
		rcRect_A.fBottom += fVelocity;
	}

	return 0;
}

