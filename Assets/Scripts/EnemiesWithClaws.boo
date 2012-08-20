import UnityEngine


class EnemiesWithClaws(MonoBehaviour):
	
	
	public hp as single
	public damage as single
	public speed as single
	public impact as single
	public blood as Transform
	
	private anime as tk2dAnimatedSprite
	private kill as GameObject
	
	private walking as bool = false
	private dead as bool = false
	private leftLook as bool = false
				
			
	def lookCameraLeft():
		if not leftLook:
			anime.FlipX()
			leftLook = true
		
		
	def lookCameraRight():
		if leftLook:
			anime.FlipX()
			leftLook = false
			

	def Start():
		anime = GetComponent[of tk2dAnimatedSprite]()
		kill = GameObject.FindWithTag("Hero")
		anime.FlipX()
	
	
	def Update():
		
		if hp <= 0 and not dead:
			dead = true
			heroX as Hero = kill.gameObject.GetComponent("Hero")
			heroX.kills += 1
			transform.position.z = 1
			Instantiate(blood, transform.position, transform.rotation)
			anime.FlipY()
			anime.Play("idle")

		if not walking:
			walking = true
			anime.Play("walk")
			
		if not dead and kill.transform.position.x < transform.position.x:
			lookCameraLeft()
		else:
			lookCameraRight()
			
		if not dead:
			direction = (kill.transform.position - transform.position).normalized
			transform.Translate(direction * Time.deltaTime * speed)


	def OnCollisionEnter(collision as Collision):
		if collision.gameObject.tag == "Hero":
			collision.rigidbody.AddForce(transform.position.normalized * impact)
			heroX as Hero = collision.gameObject.GetComponent("Hero")
			heroX.hp -= damage
			