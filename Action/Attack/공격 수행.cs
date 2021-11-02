using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {


	// attack_timer > 0.0f 이라면 공격 중
	protected float	attack_timer = 0.0f;

	// attack_disable_timer > 0.0f 이라면 공격할 수 없다.
	protected float	attack_disable_timer = 0.0f;

	// 공격 판정이 지속되는 시간
	protected const float	ATTACK_TIME = 0.3f;

	// 공격 판정이 지속되는 시간
	protected const float	ATTACK_DISABLE_TIME = 1.0f;



	void OnCollisionStay(Collision other)
	{
		//충돌했을 때, 무엇인가를 여기서 하면 된다.
		if (other.gameObject.tag == "Enemy") {

			do {
				
				// 공격 판정 실행 중에 실패 처리를 하지 않는다.
				if(attack_timer > 0.0f) {

					break;
				}
				

			} while (false);

		}

	}


	// 공격을 시작한 후의 경과 시간을 구한다.
	// 해당 시간을 통해 플레이어가 정확하게 맞추어쳤는지 조금 느슨하게 쳤는지 알 수 있고 
	//이를 통해 Hit의 등급을 만들 수 있음 PUMP의 perfect, great, good 같은
	public float	GetAttackTimer()
	{
		return(PlayerControl.ATTACK_TIME - attack_timer);
	}



    // 공격을 하며, 타격의 판정을 함.
	private void attack_control()
	{

		//공격을 했다면
		if(attack_timer > 0.0f) {

			// 공격 중

			attack_timer -= Time.deltaTime;

			// 공격 중 판정 끝 -> 공격 끝
			if(attack_timer <= 0.0f) {
				// 콜라이더（공격의 타격 판정）의 타격 판정을 무효로 한다 -> 해당 공격의 collider를 비활성화해서 Collision이 발생해서 공격으로 판정되지 않게함.
				collider.SetPowered(false);
			}
		
		} else {
			//공격이 끝난 후 딜레이가 있어야함.

			attack_disable_timer -= Time.deltaTime;

			if(attack_disable_timer > 0.0f) {

				// 아직 공격할 수 없다.

			} else {
				//딜레이가 끝나면 timer를 초기화 시키고 다시 공격할 수 있게 만듬.
				attack_disable_timer = 0.0f;

				if(is_attack_input()) {

					//타격 판정을 위해서 Collider다시 활성화
					collider.SetPowered(true);
		
					attack_timer         = ATTACK_TIME;
					attack_disable_timer = ATTACK_DISABLE_TIME;

				}
			}
		}
	}

	
}
