class Notes:
	def __init__(self,startLane=0,endLane=0,notesType='Circle',lane=0,length=0,time=0,id=0,speed=0):
		self.notesType = notesType
		self.lane = lane
		self.length = length
		self.time = time
		self.Id = id
		self.speed = speed
		self.startLane=0
		self.endLane=0
	def makeDic(self,dic):
		dic['startLane'] = self.startLane
		dic['endLane'] = self.endLane
		dic['length'] = self.length
		dic['lane'] = self.lane
		dic['speed'] = self.speed
		dic['id'] = self.Id
		dic['time'] = self.time
	
		return dic

