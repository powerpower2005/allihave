
// 2가지 방향의 힘을 동시 적용함


#define PLAYER_VEL			10.0f				// 플레이어의 속도 
#define GR				0.4f					// 중력가속도 

float			x, y;							// 위치
float			vx, vy;							// 속도


int InitCharacter(void)						// 초기화
{
	x = 0.0f;									
	vx = PLAYER_VEL;
	y = 200.0f;									
	vy = -10.0f;								

	return 0;
}


int MoveCharacter(void)						// 매 프레임 호출된다 
{
	static int		t = 0;					    // 물체의 시각 

	x = vx * t;								    // x방향의 위치 결정 
	y = 0.5f * GR * t * t + vy * t + 200.0f;	// y방향의 위치 결정 
	t++;										// t로 위치 결정 



	return 0;
}