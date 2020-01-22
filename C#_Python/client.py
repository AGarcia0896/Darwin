import socket
from io import BytesIO
import struct
import pickle
import time
import cv2

TCP_IP = "10.104.98.146"
# TCP_IP = 'localhost'
TCP_PORT = 8888
MESSAGE = b"stop"
me = b"hola"
DATO = 0

cv2.namedWindow("Align Example 1", cv2.WINDOW_AUTOSIZE)
cv2.waitKey(1)

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((TCP_IP, TCP_PORT))
s.send(me)
print(s.recv(1024))
while DATO < 500:
    if DATO == 4:
        s.send(MESSAGE)
    else:
        mes = str(DATO).encode()
        s.send(mes)
    print(s.recv(1024))
    DATO = DATO + 1
    time.sleep(3)

    key = cv2.waitKey(1)

    if key & 0xFF == ord("q") or key == 27:
        cv2.destroyAllWindows()
        break
s.close()
