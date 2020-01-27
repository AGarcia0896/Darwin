import numpy as np
import socket
import cv2
import pickle
import struct
from io import BytesIO


# Definición de la conexión
HOST=''
PORT=8888

s=socket.socket(socket.AF_INET,socket.SOCK_STREAM)
print('Socket created')

s.bind((HOST,PORT))
print('Socket bind complete')
s.listen(10)
print('Socket now listening')

conn,addr=s.accept()

data = b""
payload_size = struct.calcsize(">L")

depth_scale = conn.recv(4096)
depth_scale = struct.unpack('!d', depth_scale)[0]
print("Depth Scale is: ", depth_scale)


# Lectura de Datos
try:
    while True:

        # Cabecera
        while len(data) < payload_size:
            data += conn.recv(4096)
        
        packed_msg_size = data[:payload_size]
        data = data[payload_size:]
        msg_size = struct.unpack(">L", packed_msg_size)[0]

        # Mensaje
        while len(data) < msg_size:
            data += conn.recv(4096)
        frame_data = data[:msg_size]
        data = data[msg_size:]

        # Decodificado
        depth_image = pickle.loads(frame_data, fix_imports=True, encoding="bytes")

        # Descompresión
        f = BytesIO(depth_image)
        loaded = np.load(f)
        depth_image = loaded['depth_image']

        # Mapa de color
        depth_colormap = cv2.applyColorMap(cv2.convertScaleAbs(depth_image, alpha=0.03), cv2.COLORMAP_JET)
        cv2.namedWindow('Align Example 1', cv2.WINDOW_AUTOSIZE)
        cv2.imshow('Align Example 1', depth_colormap)


        key = cv2.waitKey(1)

        if key & 0xFF == ord('q') or key == 27:
            cv2.destroyAllWindows()
            break
finally:
    conn.close()
