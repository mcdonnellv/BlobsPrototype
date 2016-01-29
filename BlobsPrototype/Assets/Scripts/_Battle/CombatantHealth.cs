using UnityEngine;
using System.Collections;

public class CombatantHealth : MonoBehaviour {

	public float health = 100f;
	public float initialHealth = 100f;
	public float repeatDamagePeriod = 2f;
	public float hurtForce = 10f;
	public float damageAmount = 10f;

	SpriteRenderer healthBar;
	float lastHitTime;
	Vector3 healthScale;
	//PlayerControl playerControl;
	Animator anim;
	string canHitMeTag;


	// Use this for initialization
	void Awake () {
		//playerControl = GetComponent<PlayerControl>();
		//healthBar = GameObject.Find("HealthBar").GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();
		//healthScale = healthBar.transform.localScale;
	}


	public void SetHealth(float val) {
		initialHealth = val;
		health = val;
	}

	public void SetCanBeHitBy(string str) { 
		canHitMeTag = str; 
	}


	void OnCollisionEnter2D (Collision2D col) {
		if(col.gameObject.tag == canHitMeTag) {
			if(Time.time > lastHitTime + repeatDamagePeriod) {
				if(health > 0f) {
					TakeDamage(col.transform);
					lastHitTime = Time.time;
				}
				else 
					Death();
			}
		}
	}


	void TakeDamage(Transform enemy) {
		Vector3 hurtVector = transform.position - enemy.position + Vector3.up * 5f;
		GetComponent<Rigidbody2D>().AddForce(hurtVector * hurtForce);
		health -= damageAmount;
		//UpdateHealthBar();
	}


	void Death() {
		Collider2D[] cols= GetComponents<Collider2D>();
		foreach(Collider2D c in cols)
			c.isTrigger = true;
		SpriteRenderer[] spr = GetComponentsInChildren<SpriteRenderer>();
		foreach(SpriteRenderer s in spr)
			s.sortingLayerName = "UI";
		anim.SetTrigger("Die");
	}


	public void UpdateHealthBar() {
		healthBar.transform.localScale = new Vector3(healthScale.x * health / initialHealth, 1, 1);
	}
}
