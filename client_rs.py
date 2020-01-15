import pyrealsense2 as rs
import numpy as np
import socket
import struct
import pickle
from io import BytesIO

# Conexión al servidor
client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
#client_socket.connect(('10.104.100.83', 8485))
client_socket.connect(('localhost', 8888))
connection = client_socket.makefile('wb')

# Conexión con la camara Real Sense
pipeline = rs.pipeline()

config = rs.config()
config.enable_stream(rs.stream.depth, 640, 480, rs.format.z16, 30)

profile = pipeline.start(config)

depth_sensor = profile.get_device().first_depth_sensor()
depth_scale = depth_sensor.get_depth_scale()
client_socket.sendall(struct.pack('!d', depth_scale))

align_to = rs.stream.color
align = rs.align(align_to)

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
        size = len(data)

        client_socket.sendall(struct.pack(">L", size) + data)

finally:
    pipeline.stop()
