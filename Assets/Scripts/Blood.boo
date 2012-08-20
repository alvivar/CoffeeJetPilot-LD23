import UnityEngine


class Blood(MonoBehaviour):


	def Start():
		Invoke('suicide', 30)


	def suicide():
		Destroy(self.gameObject)