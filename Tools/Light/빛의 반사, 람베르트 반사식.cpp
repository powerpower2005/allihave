
// 구에 대해서 빛의 밝기 결정하기(빛의 반사)

#include <math.h>

#define VIEW_WIDTH			640					// 화면너비
#define VIEW_HEIGHT			480					// 화면높이
#define RAD					180					// 구의 반지름


// 구를 그리는 함수
int RenderScanLine(void)
{
	int					x, y;
	float				dx, dy, dz;				// 화면 중심을 기준으로 한 좌표 (구 상의 좌표)
	float				dzsq;					// z 제곱, dz square
	int					nBright;
	float				nx, ny, nz;				// 구면상의 법선 벡터
	float				lx = -1.0f / 1.732f;	// 광원의 방향 벡터 (주어지는 값)
	float				ly = -1.0f / 1.732f;
	float				lz = 1.0f / 1.732f;
	float				fAmbient = 128.0f;		// 환경광 (주어지는 값)
	float				fDirect = 128.0f;		// 직사광 (주어지는 값)
	float				fDot;					// 내적 

	for (y = 0; y < VIEW_HEIGHT; y++) {		// y 방향 루프
		for (x = 0; x < VIEW_WIDTH; x++) {	// x 방향 루프
			dx = x - VIEW_WIDTH / 2.0f;		// 상대 x 좌표
			dy = y - VIEW_HEIGHT / 2.0f;		// 상대 y 좌표
			dzsq = RAD * RAD - dx * dx - dy * dy;	// z의 제곱 계산 -> 이렇게 하는 이유 2차원에 렌더링을 함. 
			

			//x,y의 값이 범위 밖으로 벗어나는 것을 막음
			if (dzsq > 0.0f) {
				dz = sqrtf(dzsq);

				nx = dx / RAD;  ny = dy / RAD;  nz = dz / RAD;	// 법선 벡터 (구에 접하는 평면의)
				fDot = nx * lx + ny * ly + nz * lz;				// 내적 (성분 곱의 합)

				//람베르트 반사식
				if (fDot >= 0.0f) {			// 광원으로부터 빛이 닿는다
					nBright = (int)(fDot * fDirect + fAmbient);
				}
				else {							// 광원으로부터 빛이 닿지 않는다 
					nBright = (int)fAmbient;
				}
				 //DrawPoints(x, y, nBright, nBright, nBright); 렌더링
			}
		}
		//FlushDrawingPictures(); 렌더링
	}

	return 0;
}
