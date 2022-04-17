//정규분포의 확률로 공을 위로 쏘아올림 -> 이게 더 자연스럽고 현실에 가까움.

#include <stdlib.h>
#include <math.h>

#define PI					3.14159f			// 원주율(그냥 정의)
#define VIEW_WIDTH			640					// 화면 너비
#define VIEW_HEIGHT			480					// 화면 높이 
#define CHAR_WIDTH			64					// 캐릭터 너비 
#define CHAR_HEIGHT			64					// 캐릭터 높이 
#define MAX_BALL_NUM		100					// 최대 공 수 
#define VEL_WIDTH			3.0f				// 가로방향속도폭(표준편차)
#define VEL_HEIGHT			1.5f				// 세로방향속도폭(표준편차)
#define BASE_VEL			17.0f				// 기초속도 
#define GR					0.5f				// 중력가속도 


struct BALL {
	int				bAvctive;					// 유효성
	float			x, y;						// 위치 
	float			vx, vy;						// 속도 
};


BALL				Balls[MAX_BALL_NUM];		// 공 버퍼 
int					nBallNum;					// 공 수
int					nTimeCount;					// 시간 


int InitBall(void)						// 초기화
{
	int	i;
	// 무든 공을 무효로 
	for (i = 0; i < MAX_BALL_NUM; i++) {
		Balls[i].bAvctive = false;
	}
	nBallNum = 0;								// 공 수는 0 
	nTimeCount = 0;
	return 0;
}


int BallShoot(void)						
{
	int	i;
	float fRand_r, fRand_t;

	// 공의 이동 
	for (i = 0; i < MAX_BALL_NUM; i++) {
		if (Balls[i].bAvctive) {
			Balls[i].x += Balls[i].vx;
			Balls[i].y += Balls[i].vy;
			Balls[i].vy += GR;
			if ((Balls[i].x < -CHAR_WIDTH) || (Balls[i].x > VIEW_WIDTH) ||
				(Balls[i].y < -CHAR_HEIGHT) || (Balls[i].y > VIEW_HEIGHT))
			{
				Balls[i].bAvctive = false;
			}
		}
	}

	// 공의 발생, 2프레임마다.
	if ((nTimeCount % 2) == 0) {
		for (i = 0; i < MAX_BALL_NUM; i++) {
			if (Balls[i].bAvctive == false) {
				Balls[i].bAvctive = true;
				Balls[i].x = (VIEW_WIDTH - CHAR_WIDTH) / 2.0f;
				Balls[i].y = VIEW_HEIGHT - CHAR_HEIGHT;
				fRand_r = sqrtf(-2.0f * logf((float)(rand() + 1) / (RAND_MAX + 1)));	// √-2ln(a)
				fRand_t = 2.0f * PI * (float)rand() / RAND_MAX;								// 2πb
				Balls[i].vx = (fRand_r * cosf(fRand_t)) * VEL_WIDTH;				// Vx를 랜덤 설정 
				Balls[i].vy = (fRand_r * sinf(fRand_t)) * VEL_HEIGHT - BASE_VEL;	// Vy를 랜덤 설정 
				break;
			}
		}
	}
	nTimeCount++;

	return 0;
}
