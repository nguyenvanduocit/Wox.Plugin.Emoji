import os
import re


path = './icons'
file_list = os.listdir(path)
reg = r'_(.+).png'  # the regex for file names
suffix = '.png'

# print(chr(0x2694))
# print(hex(ord('😁')))
for file in file_list:
    # print(file)
    old_path = os.path.join(path, file)
    code_point = re.search(reg, file)[1]
    # print(code_point)
    if len(code_point) > 5:
        # delete the file
        os.remove(old_path)
    elif len(code_point) == 5:
        filename = chr(int(code_point, 16)) + suffix
        new_path = os.path.join(path, filename)
        os.rename(old_path, new_path)
    else:  # the emoji 1.0 hell
        filename = str(int(code_point, 16)) + suffix
        new_path = os.path.join(path, filename)
        os.rename(old_path, new_path)
