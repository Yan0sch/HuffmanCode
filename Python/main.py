# -*- coding: latin-1 -*-
import os
from huffman_tree import HuffmanTree
from time import time
import sys

if __name__ == "__main__":
    path = input("path: ")

    while not os.path.isfile(path):
        print("File doesn't exists!")
        path = input("path: ")

    t0 = time()
    with open(path, "r", encoding="latin-1") as file:
        file_content = file.read()

    print("INFO: file loaded, tooks {0:.2F} s".format(time()-t0))

    
    huffman_tree = HuffmanTree(file_content)

    t0 = time()
    huffman_tree.count_chars()
    print("INFO: file content processed, tooks {0:.2F} s".format(time()-t0))

    t0 = time()
    huffman_tree.create_tree()
    print("INFO: Huffman Tree created, tooks {0:.2F} s".format(time()-t0))

    t0 = time()
    huffman_tree.compress()
    print("INFO: file compressed, tooks {0:.2F} s".format(time()-t0))

    path = input("get new filename: ")
    if len(path.split('.')) == 1:
        path += ".huff"

    with open(path, 'w', encoding="latin-1") as file:
        file.write(huffman_tree.compressed)
    print("file writed")
    huffman_tree.uncompress()
    print(huffman_tree.uncompressed)
