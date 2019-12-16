import pyrealsense2 as rs
import numpy as np
import socket
import struct
import pickle
from io import BytesIO

client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
client_socket.connect(('10.104.100.83', 8485))
connection = client_socket.makefile('wb')

pipeline = rs.pipeline()

config = rs.config()
config.enable_stream(rs.stream.depth, 640, 480, rs.format.z16, 30)
config.enable_stream(rs.stream.color, 640, 480, rs.format.bgr8, 30)

profile = pipeline.start(config)

depth_sensor = profile.get_device().first_depth_sensor()
depth_scale = depth_sensor.get_depth_scale()
client_socket.sendall(struct.pack('!d', depth_scale))

clipping_distance_in_meters = 1
clipping_distance = clipping_distance_in_meters / depth_scale

align_to = rs.stream.color
align = rs.align(align_to)

try:
    while True:
        frames = pipeline.wait_for_frames()
        aligned_frames = align.process(frames)

        aligned_depth_frame = aligned_frames.get_depth_frame()
        color_frame = aligned_frames.get_color_frame()

        if not aligned_depth_frame or not color_frame:
            continue

        depth_image = np.asanyarray(aligned_depth_frame.get_data())
        color_image = np.asanyarray(color_frame.get_data())        

        f = BytesIO()
        np.savez_compressed(f, depth_image=depth_image)
        f.seek(0)
        depth_image = f.read()


        l = []
        l.append(depth_image)
        l.append(color_image)   
        l.append(clipping_distance)
        
        data = pickle.dumps(l, 0)
        size = len(data)

        client_socket.sendall(struct.pack(">L", size) + data)

finally:
    pipeline.stop()