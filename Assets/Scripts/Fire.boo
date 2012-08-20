import UnityEngine


class Fire(MonoBehaviour): 


	public damage as single = 5
	public speed as single = 250
	
	
	def go(goFrom as Vector3, goTo as Vector3):
		direction = (goTo - goFrom).normalized
		transform.position = goFrom
		rigidbody.AddForce(direction * speed)
		
	
	def suicide():
		Destroy(self.gameObject)
		
		
	def OnBecameInvisible():
		Destroy(self.gameObject)
		
		
	def OnCollisionEnter(collision as Collision):
		if collision.gameObject.tag == "Enemy":
			enemyScript as EnemiesWithClaws = collision.gameObject.GetComponent("EnemiesWithClaws")
			enemyScript.hp -= damage
		suicide()