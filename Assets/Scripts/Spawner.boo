import UnityEngine


class Spawner(MonoBehaviour):
	
	
	private width as single = 0
	private height as single = 0
	
	public crawlerPrefab as GameObject
	public skinzordPrefab as GameObject
	public zomboidPrefab as GameObject


	def Start ():
		height = Camera.main.orthographicSize * 2
		width = Camera.main.aspect * height
		InvokeRepeating("Spawn", 7, 7)

				
	def Spawn():
		rX = Random.Range(width * -0.5F, width * 0.5F)
		rY = Random.Range(width * -0.5F, width * 0.5F)
		Instantiate(crawlerPrefab, Vector3(rX, rY, 0), Quaternion.identity)
		rX = Random.Range(width * -0.5F, width * 0.5F)
		rY = Random.Range(width * -0.5F, width * 0.5F)
		Instantiate(skinzordPrefab, Vector3(rX, rY, 0), Quaternion.identity)
		rX = Random.Range(width * -0.5F, width * 0.5F)
		rY = Random.Range(width * -0.5F, width * 0.5F)
		Instantiate(zomboidPrefab, Vector3(rX, rY, 0), Quaternion.identity)