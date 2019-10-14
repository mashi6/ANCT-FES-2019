class Notes:
	def __init__(self,notesType='Circle',lane=0,length=0,timing=0):
		self.notesType = notesType
		self.lane = lane
		self.length = length
		self.timing = timing
	def makeMessage(self):
		return ':'.join([str(self.lane),self.notesType,str(self.length),str(self.timing)])