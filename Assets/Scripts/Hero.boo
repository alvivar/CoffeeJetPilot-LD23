import UnityEngine


class Hero(MonoBehaviour): 


	public hp as single
	public firePrefab as Transform

	public kills as single = 0

	private anime as tk2dAnimatedSprite
	private step as decimal = 0.01
	private leftLook as bool = false
		
		
	def stopAnimationDelegate(sprite as tk2dAnimatedSprite, clipId as int):
		anime.Play("idle")
			
			
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
		
				
	def Update():
		
		# UP
		
		if Input.GetKey(KeyCode.W):
			if not anime.isPlaying():
				anime.Play("walk")
				anime.animationCompleteDelegate = null
			transform.position.y += step

		if Input.GetKeyUp(KeyCode.W):
			anime.Play("idle")
		
		# RIGHT
		
		if Input.GetKey(KeyCode.D):
			lookCameraRight()
			if not anime.isPlaying():
				anime.Play("walk")
				anime.animationCompleteDelegate = null
			transform.position.x += step
		
		if Input.GetKeyUp(KeyCode.D):
			anime.Play("idle")
				
		# DOWN
		
		if Input.GetKey(KeyCode.S):
			if not anime.isPlaying():
				anime.Play("walk")
				anime.animationCompleteDelegate = null
			transform.position.y -= step
			
		
		if Input.GetKeyUp(KeyCode.S):
			anime.Play("idle")
		
		# LEFT
		
		if Input.GetKey(KeyCode.A):
			lookCameraLeft()
			if not anime.isPlaying():
				anime.Play("walk")
				anime.animationCompleteDelegate = null
			transform.position.x -= step
				
		if Input.GetKeyUp(KeyCode.A):
			anime.Play("idle")
			
		# MOUSE
		
		if Input.GetMouseButtonDown(0):
			
			clicPos = Camera.mainCamera.ScreenToWorldPoint(Input.mousePosition)
			
			newPosition as Vector3 = Vector3(transform.position.x, transform.position.y, 0)
			
			if clicPos.x < transform.position.x:
				newPosition.x -= 0.08
				newPosition.y -= 0.0245
				lookCameraLeft()
			else:
				newPosition.x += 0.08
				newPosition.y -= 0.0245
				lookCameraRight()
			
			fireCast as Transform = Instantiate(firePrefab, newPosition, Quaternion.identity)
			fireScript as Fire = fireCast.gameObject.GetComponent('Fire')
			fireScript.go(newPosition, Vector3(clicPos.x, clicPos.y, 0))
			
			anime.Play("cast")
			anime.animationCompleteDelegate = stopAnimationDelegate
			
			
		if Input.GetMouseButtonDown(1):
			
			clicPos = Camera.mainCamera.ScreenToWorldPoint(Input.mousePosition)
			anime.Play("teleport")
			anime.animationCompleteDelegate = stopAnimationDelegate
			transform.position = Vector3(clicPos.x, clicPos.y, 0)
	
	def OnGUI():
        GUI.Label(Rect(10, 10, 100, 20), 'hp ' + hp)
        GUI.Label(Rect(10, 30, 100, 20), 'kills ' + kills)