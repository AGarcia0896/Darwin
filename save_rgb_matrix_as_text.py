# Read the image file.
import imageio
import cv2 as cv
data = imageio.imread('depth_colormap.jpg')

for i in range(3):
    cv.imshow("Channel {}" .format(i), data[:,:,i])
    cv.waitKey(0)

# Make a string with the format you want.
for i in range(3):
    text = ''
    for row in data:
        for e in row:
            #text += '({}, {}, {}) '.format(e[0], e[1], e[2])
            text += '({}) '.format(e[i])
        text += '\n'

    # Write the string to a file.
    with open('chanel{}.txt' .format(i), 'w') as f:
        f.write(text)