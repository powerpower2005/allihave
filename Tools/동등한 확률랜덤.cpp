
//공을 위로 동등한 확률 랜덤으로 쏘아올림.

#include <stdlib.h>
#include <math.h>

#define VIEW_WIDTH			640				// 화면 너비 
#define VIEW_HEIGHT			480				// 화면 높이 
#define CHAR_WIDTH			64				// 캐릭터 너비 
#define CHAR_HEIGHT			64				// 캐릭터 높이 
#define MAX_BALL_NUM		100				// 최대 공 수 
#define VEL_WIDTH			10				// 가로방향 속도 폭 
#define VEL_HEIGHT			5				// 세로 방향 속도 폭 
#define BASE_VEL			17.0f			// 기초속도 
#define GR					0.5f			// 중력가속도 


struct BALL {
	int				bAvctive;				// 유효한가?
	float			x, y;					// 위치
	float			vx, vy;					// 속도 
};

BALL				Balls[MAX_BALL_NUM];	// 공
int					nBallNum;				// 공 수 
int					nTimeCount;				// 시간 


int InitBall( void )					// 초기화
{
	int	i;
	// 모든 공을 무효로 
	for ( i = 0; i < MAX_BALL_NUM; i++ ) {
		Balls[i].bAvctive = false;
	}
	nBallNum = 0;							
	nTimeCount = 0;
	return 0;
}


int BallShoot( void )
{
	int	i;

	// 공의 이동 
	for ( i = 0; i < MAX_BALL_NUM; i++ ) {
		if ( Balls[i].bAvctive ) {
			Balls[i].x  += Balls[i].vx;
			Balls[i].y  += Balls[i].vy;
			Balls[i].vy += GR;
			// 화면 밖으로 나가면 무효 
			if ( ( Balls[i].x < -CHAR_WIDTH )  || ( Balls[i].x > VIEW_WIDTH  ) ||
				 ( Balls[i].y < -CHAR_HEIGHT ) || ( Balls[i].y > VIEW_HEIGHT ) )
			{
				Balls[i].bAvctive = false;
			}
		}
	}

	// 공의 발생, 2프레임마다
	if ( ( nTimeCount % 2 ) == 0 ) {
		for ( i = 0; i < MAX_BALL_NUM; i++ ) {
			if ( Balls[i].bAvctive == false ) {
				Balls[i].bAvctive = true;
				Balls[i].x = ( VIEW_WIDTH - CHAR_WIDTH ) / 2.0f;	// 화면 중앙으로 
				Balls[i].y = VIEW_HEIGHT - CHAR_HEIGHT;				// 화면 하단으로 
				Balls[i].vx = ( rand() % ( VEL_WIDTH  + 1 ) )
							  - VEL_WIDTH  / 2.0f;
				Balls[i].vy = ( rand() % ( VEL_HEIGHT + 1 ) )
							  - VEL_HEIGHT / 2.0f - BASE_VEL;
				break;
			}
		}
	}
	nTimeCount++;

	return 0;
}
