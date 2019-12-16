import socket

TCP_IP = '127.0.0.1'
TCP_PORT = 5005
MESSAGE = b'Hi my friend <EOF>'

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((TCP_IP, TCP_PORT))
s.send(MESSAGE)
s.close()