//부채꼴 모양과의 충돌 판정

//무엇인가를 휘두를 때 부채꼴로 휘두르는 것에서 필요성

#include <windows.h>
#include <math.h>

#define PI					3.14159265f			// 원주율 
#define VIEW_WIDTH			640					// 화면 너비 
#define VIEW_HEIGHT			480					// 화면 높이 
#define CIRCLE_VEL			5.0f				// 원형 속도 


struct F_CIRCLE {
	float			x, y;				// 중심 위치 
	float			r;					// 반지름 
};

struct F_FAN {
	float			x, y;				// 중심 위치 
	float			vx1, vy1;			// 벡터1(부채꼴에서 호가 아닌 옆부분)
	float			vx2, vy2;			// 벡터2(부채꼴에서 호가 아닌 옆부분)
	float			fAngle1, fAngle2;	// 각 벡터의 각도 
	float			r;					// 반지름 
};


F_CIRCLE			crCircleA;			// 원형A
F_FAN				faFanB;				// 원형B


int InitShapes(void)					// 초기화  
{
	crCircleA.x = 100.0f;  crCircleA.y = 100.0f;
	crCircleA.r = 70.0f;
	faFanB.x = 200.0f;  faFanB.y = 200.0f;
	faFanB.fAngle1 = -PI / 6.0f;
	faFanB.fAngle2 = PI / 3.0f;
	faFanB.r = 200.0f;
	faFanB.vx1 = faFanB.r * cosf(faFanB.fAngle1);  faFanB.vy1 = faFanB.r * sinf(faFanB.fAngle1);
	faFanB.vx2 = faFanB.r * cosf(faFanB.fAngle2);  faFanB.vy2 = faFanB.r * sinf(faFanB.fAngle2);

	return 0;
}


int CheckHit(F_CIRCLE* pcrCircle, F_FAN* pfaFan)		// 충돌 판정
{
	int				nResult = false;

	float			dx, dy;							// 위치의 차이 
	float			fAlpha, fBeta;
	float			fDelta;
	float			ar;								// 부채꼴과 원의 반지름을 더한 것 
	float			fDistSqr;
	float			a, b, c;
	float			d;
	float			t;


	//부채꼴 중심이 원 안에 있으면 충돌
	dx = pcrCircle->x - pfaFan->x;  dy = pcrCircle->y - pfaFan->y;
	fDistSqr = dx * dx + dy * dy;
	if (fDistSqr < pcrCircle->r * pcrCircle->r) {
		nResult = true;
	}

	//그 외
	else {

		//원이 부채꼴을 만드는 벡터 사이에 존재할 때 ( 기저 벡터로 표현했을 때 양의 스칼라 배를 가짐)
		fDelta = pfaFan->vx1 * pfaFan->vy2 - pfaFan->vx2 * pfaFan->vy1;
		fAlpha = (dx * pfaFan->vy2 - dy * pfaFan->vx2) / fDelta;
		fBeta = (-dx * pfaFan->vy1 + dy * pfaFan->vx1) / fDelta;

		//원과 부채꼴의 반지름을 더한 관계
		if ((fAlpha >= 0.0f) && (fBeta >= 0.0f)) {
			ar = pfaFan->r + pcrCircle->r;
			if (fDistSqr <= ar * ar) {
				nResult = true;
			}
		}

		//사이에 없지만 교점을 가지는 경우
		else {
			a = pfaFan->vx1 * pfaFan->vx1 + pfaFan->vy1 * pfaFan->vy1;
			b = -(pfaFan->vx1 * dx + pfaFan->vy1 * dy);
			c = dx * dx + dy * dy - pcrCircle->r * pcrCircle->r;
			d = b * b - a * c;
			if (d >= 0.0f) {
				t = (-b - sqrtf(d)) / a;
				if ((t >= 0.0f) && (t <= 1.0f)) {
					nResult = true;
				}
			}
			a = pfaFan->vx2 * pfaFan->vx2 + pfaFan->vy2 * pfaFan->vy2;
			b = -(pfaFan->vx2 * dx + pfaFan->vy2 * dy);
			c = dx * dx + dy * dy - pcrCircle->r * pcrCircle->r;
			d = b * b - a * c;
			if (d >= 0.0f) {
				t = (-b - sqrtf(d)) / a;
				if ((t >= 0.0f) && (t <= 1.0f)) {
					nResult = true;
				}
			}
		}
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
