//마우스 포인터에 대해 자연스럽게 따라가는 레이저
//외적을 이용하여 현재 레이저의 방향벡터에 대해 마우스 포인터는 어디있는지 조사하여 해당 방향으로 일정한 만큼 회전.
//내적을 이용하면 마우스포인터에 대하여 각은 알 수 있으나 어느 방향인지 알 수 없음. 

#include <windows.h>
#include <math.h>

#define PI					3.14159265f			// 원주율 
#define VIEW_WIDTH			640					// 화면 너비 
#define VIEW_HEIGHT			480					// 화면 높이 
#define RAY_WIDTH			30.0f				// 광선 폭 
#define DIVIDE_NUM			30					// 곡선 분할수 -> 분할 수가 많아질수록 곡선으로 보임.
#define SEGMENT_LEN			30.0f				// 1세그먼트의 길이 -> 세그먼트가 작을수록 곡선으로 보임
#define ANGLE_SPEED			( 2.0f * PI / 50.0f )	// 방향회전 속도 (즉, 세그먼트가 얼마나 휘는지)

//분할은
//너무 많아도 문제인 것이 직선같아 보임.
//너무 적어도 문제인 것이 부드럽지 않음.

struct MyVector2D {
	float			x, y;
};

MyVector2D		v2Pos[DIVIDE_NUM * 4];


int InitRay(void)						// 초기화
{
	int					i;

	for (i = 0; i < DIVIDE_NUM * 4; i++) {
		v2Pos[i].x = 0.0f;  v2Pos[i].y = 0.0f;			// 정점의 초기 위치
	}

	return 0;
}


int MoveRay(void)						
{
	int					i;
	POINT			CursorPos;						//마우스 포인터
	MyVector2D		v2Present;
	MyVector2D		v2Aim;							//마우스 방향 벡터
	MyVector2D		v2Forward;						//레이저의 진행방향 벡터
	MyVector2D		v2Side1, v2Side2;				//Forward를 두 개의 성분으로 나눔
	float			fAngle;							// 각도 
	float			fLength;						// 길이 
	float			fCross;							// 외적 

	v2Present.x = 0.0f;  v2Present.y = VIEW_HEIGHT / 2;
	fAngle = 0.0f;


	// 첫번째 포워드・사이드벡터
	// 벡터의 크기는 segment의 수만큼
	// 벡터의 두께는 다시 segment를 나누고 절반만큼만
	v2Forward.x = SEGMENT_LEN * cosf(fAngle); 
	v2Forward.y = SEGMENT_LEN * sinf(fAngle);
	v2Side1.x = v2Forward.y;  v2Side1.y = -v2Forward.x;
	v2Side1.x *= RAY_WIDTH / 2.0f / SEGMENT_LEN;  v2Side1.y *= RAY_WIDTH / 2.0f / SEGMENT_LEN;




	for (i = 0; i < DIVIDE_NUM * 4; i += 4) {
		v2Forward.x = SEGMENT_LEN * cosf(fAngle);  v2Forward.y = SEGMENT_LEN * sinf(fAngle);
		v2Side2.x = v2Forward.y;  v2Side2.y = -v2Forward.x;
		v2Side2.x *= RAY_WIDTH / 2.0f / SEGMENT_LEN;  v2Side2.y *= RAY_WIDTH / 2.0f / SEGMENT_LEN;
		v2Pos[i].x = v2Present.x + v2Side1.x;
		v2Pos[i].y = v2Present.y + v2Side1.y;
		v2Pos[i + 1].x = v2Present.x + v2Forward.x + v2Side2.x;
		v2Pos[i + 1].y = v2Present.y + v2Forward.y + v2Side2.y;
		v2Pos[i + 2].x = v2Present.x - v2Side1.x;
		v2Pos[i + 2].y = v2Present.y - v2Side1.y;
		v2Pos[i + 3].x = v2Present.x + v2Forward.x - v2Side2.x;
		v2Pos[i + 3].y = v2Present.y + v2Forward.y - v2Side2.y;
		v2Present.x += v2Forward.x;
		v2Present.y += v2Forward.y;
		v2Side1 = v2Side2;
		GetCursorPos(&CursorPos);
		ScreenToClient(g_hWnd, &CursorPos);
		v2Aim.x = CursorPos.x - v2Present.x;  v2Aim.y = CursorPos.y - v2Present.y; 

		fLength = sqrtf(v2Aim.x * v2Aim.x + v2Aim.y * v2Aim.y);// 마우스 포인터에 대한 벡터를 단위벡터로 만듬
		v2Aim.x /= fLength;  v2Aim.y /= fLength;

		fCross = v2Forward.x * v2Aim.y - v2Forward.y * v2Aim.x; //포인터가 레이저가 나가는 방향의 좌측에 있는지 우측에 있는지 확인 -> 외적 z 성분 단위벡터이므로 sin값만 나옴
		fAngle += ANGLE_SPEED * fCross / SEGMENT_LEN; // fCross / SEGMENT_LEN이 sin(theta)가 됨. 사인함수로 선회속도를 정할 수 있음.
	}

	return 0;
}
