# Importar librerías
import cv2 as cv
import numpy as np

# Leer imagen
frame = cv.imread('depth_colormap.jpg',0)
#gray = cv.cvtColor(frame,cv.COLOR_BGR2GRAY)
ret, thresh = cv.threshold(frame,100,255,cv.THRESH_BINARY)

cv.imshow("Gray Frame",thresh)
cv.waitKey(0)