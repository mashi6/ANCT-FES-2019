from websocket_server import WebsocketServer
import json
import sched
import time
import threading
import random
import Notes
# import FootNotes
from time import sleep


# Enter押さずに即時入力できるようにするやつ
try:
	from msvcrt import getch
except ImportError:
	def getch():
		import sys
		import tty
		import termios
		fd = sys.stdin.fileno()
		old = termios.tcgetattr(fd)
		try:
			tty.setraw(fd)
			return sys.stdin.read(1)
		finally:
			termios.tcsetattr(fd, termios.TCSADRAIN, old)


Score = 0
Combo = 0
StartTime = 0.0
FootLeft = 6.0
FootRight = 9.0
notesJson = []
NotesList = []
FootNotesList = []
Id = 0

# クライアント接続された時に呼び出されるやつ
def new_client(client,server):
	# server.send_message_to_all("A new client has joined us")
	print('New client connected')

# データ受け取った時に呼び出されるやつ
def received_message(client,server,message):
	print('received')



def scheduling():
	global Id
	for note in notesJson:
		time = note["Timing"] #そのノーツが表示される、音楽開始からの時間(ms)
		lane = note["Lane"] #そのノーツが表示されるレーン
		notesType = note["NotesType"] #丸ノーツか足ノーツか田舎
		length = '0'
		try:
			length = note["Length"] #足ノーツの長さ
		except:
			pass
		speed = 0.8+random.random()
		notes = Notes.Notes(notesType=notesType,lane=int(lane)-1,length=float(length),time=float(time),id=Id,speed=speed)
		Id += 1
		generate(notes)
		sleep(0.1)

def generate(notes):
	dic = {}
	if notes.notesType == 'Circle':
		lane = int(notes.lane)
		NotesList[lane].append(notes)
		dic = notes.makeDic({'command':'new'})

	elif notes.notesType == 'Rectangle':
		lane = int(notes.lane)
		FootNotesList[lane].append(notes)
		dic = notes.makeDic({'command':'newFoot'})
	
	send_msg(json.dumps(dic));


# WebSocketでデータ送信するやつ
def send_msg(message):
	server.send_message_to_all(message)

# WSサーバ起動
def server_run(server):
	server.run_forever()

# 入力受付
def input_char():
	global Score,Combo,StartTime
	while True:
		CTRL_C = 3
		key = ord(getch())
		char = chr(key)
		if key == CTRL_C:
			break
		message = 'input, {0}'.format(chr(key))
		print(message)

		if char == 's':# 'start'の's'
			Combo = 0
			Score = 0
			StartTime = time.time()
			print(StartTime)
			dic = {'command':'start','time':int(StartTime*1000)}
			send_msg(json.dumps(dic))
			# send_msg(['reset'])# クライアントが持っているノーツ情報の初期化
			thread = threading.Thread(target=scheduling)# サブスレッドでスケジュール
			thread.start()
		
		elif char == 'e':
			dic = {'command':'end'}
			send_msg(json.dumps(dic))
			Score = 0
			Combo = 0
			StartTime = 0.0
			FootLeft = 6.0
			FootRight = 9.0
			notesJson = []
			NotesList = []
			FootNotesList = []
			Id = 0

		elif char >= '1' and char <= '4':# charはレーン番号を保持する文字列型
			judge(char)

		elif (char >= '5' and char <= '9') or char == '0':
			# pass
			moveFoot(char)

		
# 判定処理
def judge(char):
	global Score,Combo
	judgelist = ['excellent','great','good','bad']
	rdmjudge = random.choice(judgelist)
	if rdmjudge is 'bad':
		Combo = 0
	else:
		Combo += 1
		Score += 50*random.choice(range(10,20))
	if len(NotesList[int(char) - 1]) > 0:
		notes = NotesList[int(char) - 1].pop(0)
		dic = {'command':'judge','judge':rdmjudge,'combo':Combo,'score':Score,'id':notes.Id,'delete':True}
		# send_msg(['remove',char,rdmjudge,str(Combo),str(Score)]);
		send_msg(json.dumps(dic))
	#簡易版なので判定無しにすぐノーツ削除 char番目のレーンにある一番近いノーツを削除

def moveFoot(char):
	if char == '0':
		char = '10'
	global FootLeft,FootRight

	if char >= '8' or char == '10':
		FootRight = float(char)
	elif char <= '7':
		FootLeft = float(char)
	dic = {"command":"foot","positionLeft":FootLeft,"positionRight":FootRight};
	send_msg(json.dumps(dic));
	# send_msg(['move',str(FootLeft),str(FootRight)])

# def scheduleJudgeFoot():
# 	schedule.every(0.5).seconds.do(judgeFoot)

def judgeNotes():
	global FootLeft,FootRight,StartTime,Combo,Score
	arr = []
	now = time.time()
	elapseTime = (now - StartTime)*1000.0
	if len(FootNotesList[0]) > 0:
		arr.append(FootNotesList[0][0])
	if len(FootNotesList[1]) > 0:
		arr.append(FootNotesList[1][0])
	if len(FootNotesList[2]) > 0:
		arr.append(FootNotesList[2][0])
	if len(FootNotesList[3]) > 0:
		arr.append(FootNotesList[3][0])

	for notes in arr:
		lane = notes.lane
		length = notes.length
		timing = notes.time
		id = notes.Id

#横 10.3
#縦 7.8
#判定ライン -1.95
#ノーツ幅=レーン幅 1.2875
		if elapseTime >= timing+length/2:
			FootNotesList[lane].pop(0)
			Combo = 0
			dic = {'command':'judge','type':'Rectangle','lane':lane,'judge':'bad','combo':Combo,'score':Score,'id':id,'delete':True}
			# send_msg(['removeFoot',str(lane),'bad',str(Combo),str(Score)]);
			send_msg(json.dumps(dic))

		elif elapseTime >= timing-length/2:
			if  lane == FootLeft-5 or lane == FootRight-5:
				Combo += 1
				Score += 50*random.choice(range(10,20))
				dic = {'command':'judge','type':'Rectangle','judge':'good','combo':Combo,'score':Score,'id':id,'delete':False}
				# send_msg(['judgeFoot',str(lane),'good',str(Combo),str(Score)])
				send_msg(json.dumps(dic))
			else:
				Combo = 0
				dic = {'command':'judge','type':'Rectangle','judge':'bad','combo':Combo,'score':Score,'id':id,'delete':False}
				# send_msg(['judgeFoot',str(lane),'bad',str(Combo),str(Score)])
				send_msg(json.dumps(dic))
	
	arr = []
	if len(NotesList[0]) > 0:
		arr.append(NotesList[0][0])
	if len(NotesList[1]) > 0:
		arr.append(NotesList[1][0])
	if len(NotesList[2]) > 0:
		arr.append(NotesList[2][0])
	if len(NotesList[3]) > 0:
		arr.append(NotesList[3][0])
	for notes in arr:
		lane = notes.lane
		timing = notes.time
		if elapseTime >= timing+500:
			NotesList[lane].pop(0)
			Combo = 0
			id = notes.Id
			dic = {'command':'judge','type':'Circle','judge':'bad','combo':Combo,'score':Score,'id':id,'delete':True}
			# send_msg(['remove',str(lane),'bad',str(Combo),str(Score)]);
			send_msg(json.dumps(dic))
			

	threading.Timer(0.2, judgeNotes).start()

	



if __name__ == "__main__":

	f = open('MusicScore.json', 'r') # Json読み込み
	jsonData = json.load(f)
	f.close()
	notesJson = jsonData["Notes"]
	FootNotesList.append([])
	FootNotesList.append([])
	FootNotesList.append([])
	FootNotesList.append([])
	NotesList.append([])
	NotesList.append([])
	NotesList.append([])
	NotesList.append([])
	server = WebsocketServer(3000,host='127.0.0.1') # WSserver初期化
	server.set_fn_new_client(new_client)
	server.set_fn_message_received(received_message)
	thread_1 = threading.Thread(target=server_run, args=([server])) # Serverと入力受付はサブスレッド
	thread_2 = threading.Thread(target=input_char)
	thread_3 = threading.Thread(target=judgeNotes)
	
	thread_1.start()
	thread_2.start()
	thread_3.start()

