from websocket_server import WebsocketServer
import json
import sched
import time
import threading

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

# クライアント接続された時に呼び出されるやつ
def new_client(client,server):
	server.send_message_to_all("A new client has joined us")

# データ受け取った時に呼び出されるやつ
def received_message(client,server,message):
	print('received')

# ノーツ情報を送信を事前にスケジュールする
def scheduling():
	scheduler = sched.scheduler(time.time, time.sleep)
	for note in notes:
		timing = float(note["Timing"]) #そのノーツが表示される、音楽開始からの時間(ms)
		lane = note["Lane"] #そのノーツが表示されるレーン
		scheduler.enter(timing*0.001, 1, send_msg, ('generate',str(lane))) #スケジュールを登録
	scheduler.run()

# WebSocketでデータ送信するやつ
def send_msg(code,message):
	server.send_message_to_all(code + ':' + message)
	# print('WebSocket Send Message: '+code+':'+message)

# WSサーバ起動
def server_run(server):
	server.run_forever()

# 入力受付
def input_char():
	while True:
		CTRL_C = 3
		key = ord(getch())
		char = chr(key)
		if key == CTRL_C:
			break
		message = 'input, {0}'.format(chr(key))
		print(message)

		if char == 's':# 'start'の's'
			send_msg(code='reset',message='')# クライアントが持っているノーツ情報の初期化
			thread = threading.Thread(target=scheduling)# サブスレッドでスケジュール
			thread.start()

		if char >= '1' and char <= '4':# charはレーン番号を保持する文字列型
			judge(char)

		
# 判定処理
def judge(char):
	send_msg('remove',char);#簡易版なので判定無しにすぐノーツ削除 char番目のレーンにある一番近いノーツを削除

if __name__ == "__main__":
	f = open('MusicScore.json', 'r') # Json読み込み
	jsonData = json.load(f)
	f.close()
	notes = jsonData["Notes"]
	server = WebsocketServer(3000,host='127.0.0.1') # WSserver初期化
	server.set_fn_new_client(new_client)
	server.set_fn_message_received(received_message)
	thread_1 = threading.Thread(target=server_run, args=([server])) # Serverと入力受付はサブスレッド
	thread_2 = threading.Thread(target=input_char)
	thread_1.start()
	thread_2.start()


