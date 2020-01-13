#import the Image module of PIL into the shell:
from PIL import Image
#create an image object and open the image for reading mode:
im = Image.open('depth_colormap.jpg','r')
pix_val = list(im.getdata())
print(len(pix_val))
#print(type(pix_val))
# for i in range(20):
#     print(pix_val[i])
#Write image pixel value to a file
#MyFile = open('data.txt','w')
#MyFile.writelines("%s" % pix_val[0][0])
""" for item in pix_val:
    MyFile.write(item) """
#print(pix_val)