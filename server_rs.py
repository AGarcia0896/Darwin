import numpy as np
import socket
import cv2
import pickle
import struct
from io import BytesIO

HOST=''
PORT=8485

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

try:
    while True:

        while len(data) < payload_size:
            data += conn.recv(4096)
        
        packed_msg_size = data[:payload_size]
        data = data[payload_size:]
        msg_size = struct.unpack(">L", packed_msg_size)[0]
        while len(data) < msg_size:
            data += conn.recv(4096)
        frame_data = data[:msg_size]
        data = data[msg_size:]

        frame=pickle.loads(frame_data, fix_imports=True, encoding="bytes")

        depth_image = frame[0]
        color_image = frame[1]
        clipping_distance = frame[2]

        f = BytesIO(depth_image)
        loaded = np.load(f)
        depth_image = loaded['depth_image']

        grey_color = 153
        depth_image_3d = np.dstack((depth_image,depth_image,depth_image))
        bg_removed = np.where((depth_image_3d > clipping_distance) | (depth_image_3d <= 0), grey_color, color_image)

        depth_colormap = cv2.applyColorMap(cv2.convertScaleAbs(depth_image, alpha=0.03), cv2.COLORMAP_JET)
        images = np.hstack((bg_removed, depth_colormap))
        cv2.namedWindow('Align Example 1', cv2.WINDOW_AUTOSIZE)
        cv2.imshow('Align Example 1', images)


        key = cv2.waitKey(1)

        if key & 0xFF == ord('q') or key == 27:
            cv2.destroyAllWindows()
            break
finally:
    conn.close()