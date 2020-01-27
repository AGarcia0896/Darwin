import pyrealsense2 as rs
import numpy as np
import socket
import struct
import pickle
from io import BytesIO
import cv2

# Conexión al servidor
client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
client_socket.connect(("10.104.98.146", 8888))
# client_socket.connect(("localhost", 8485))
connection = client_socket.makefile("wb")

# Conexión con la camara Real Sense
pipeline = rs.pipeline()

config = rs.config()
config.enable_stream(rs.stream.depth, 640, 480, rs.format.z16, 30)

profile = pipeline.start(config)

depth_sensor = profile.get_device().first_depth_sensor()
depth_scale = depth_sensor.get_depth_scale()
# client_socket.sendall(struct.pack("!d", depth_scale))
print(depth_scale)
client_socket.sendall(str(depth_scale).encode())

align_to = rs.stream.color
align = rs.align(align_to)

# cv2.namedWindow('Align Example 1',cv2.WINDOW_AUTOSIZE)
# cv2.waitKey(1)

# Obtención de los Frames
try:
    while True:
        frames = pipeline.wait_for_frames()
        aligned_frames = align.process(frames)

        aligned_depth_frame = aligned_frames.get_depth_frame()
        depth_image = np.asanyarray(aligned_depth_frame.get_data())

        # Compresión
        f = BytesIO()
        np.savez_compressed(f, depth_image=depth_image)
        f.seek(0)
        depth_image = f.read()

        # Codificación
        data = pickle.dumps(depth_image, 0)
        # size = len(data)

        # client_socket.sendall(struct.pack(">L", size) + data)
        client_socket.sendall(data)

        key = cv2.waitKey(1)
        if key & 0xFF == ord("q") or key == 27:
            cv2.destroyAllWindows()
            break

finally:
    pipeline.stop()
    client_socket.close()
