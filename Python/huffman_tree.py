# -*- coding: latin-1 -*-
import sys
from multiprocessing import Process, Queue, Pool


class Node:
    def __init__(self, name, prob, depth=0, child_nodes=None):
        self.name = name
        self.prob = prob
        self.depth = depth
        self.child_nodes = child_nodes

    def __repr__(self):
        return f"<{self.name}, p = {self.prob}, depth = {self.depth}>"


class HuffmanTree:
    def __init__(self, text: str):
        self.uncompressed = text
        self.compressed = ""
        self.codes = {}
        self.nodes = []
        self.tree = None

    def __count_chars_proc(self, c, queue, id):
        nodes = []
        print(f"INFO: Process {id} started")
        for i in c:
            count = self.uncompressed.count(chr(i))
            if count != 0:
                nodes.append(Node(chr(i), count/len(self.uncompressed)))
        
            sys.stdout.write(f"\rProcess {id}: {i%len(c)}/{len(c)}")


        queue.put(nodes)

    def count_chars(self):
        core_count = 8
        core_range = 256//8 + 1
        procs = []
        queue = Queue()
        print("couting chars")
        for i in range(core_count):
            proc = Process(target=self.__count_chars_proc, args=(range(i*core_range,i*core_range+core_range),queue, i))
            procs.append(proc)
            proc.start()

        for p in procs:
            p.join()
            self.nodes += queue.get()

    def create_tree(self):
        while len(self.nodes) > 1:
            new_node = [Node('', 1), Node('', 1)]
            for n in self.nodes:
                if new_node[0].prob > n.prob or (new_node[0].prob == n.prob and new_node[0].depth > n.depth):
                    new_node[0] = n
            self.nodes.remove(new_node[0])

            for n in self.nodes:
                if new_node[1].prob > n.prob or (new_node[1].prob == n.prob and new_node[1].depth > n.depth):
                    new_node[1] = n
            self.nodes.remove(new_node[1])

            self.nodes.append(Node(new_node[0].name + new_node[1].name,
                              new_node[0].prob + new_node[1].prob,
                              max(new_node[0].depth, new_node[1].depth) + 1,
                              new_node))

        self.tree = self.nodes[0]
        self.generate_codes(self.tree, "")

    def generate_codes(self, node: Node, code: str):
        if node.child_nodes is None:
            self.codes[node.name] = code
        else:
            self.generate_codes(node.child_nodes[0], code + '0')
            self.generate_codes(node.child_nodes[1], code + '1')

    def compress(self):
        self.compressed = []
        bin_string = []

        for idx, c in enumerate(self.uncompressed):
            bin_string.append(self.codes[c])
            if len(bin_string) >= 8:
                self.compressed.append(chr(int(''.join(bin_string)[:7], 2)))
                bin_string = bin_string[7:]
            if idx % 1000 == 0:
                sys.stdout.write(f"\r{idx}/{len(self.uncompressed)}")

        self.compressed = ''.join(self.compressed)
        # bin_string = ''.join(self.codes[c] for c in self.uncompressed)
        # self.compressed = ''.join(chr(int(bin_string[i*8:i*8+8],2)) for i in range(len(bin_string)//8))

    def uncompress(self):
        self.uncompressed = ""
        bin_string = []

        bin_string = ''.join(bin(ord(c))[2:].zfill(8) for c in self.compressed)

        index = 0
        curr_node = self.tree
        while index < len(bin_string):
            curr_node = curr_node.child_nodes[int(bin_string[index])]
            index += 1
            if curr_node.child_nodes is None:
                self.uncompressed = self.uncompressed.join(curr_node.name)
                curr_node = self.tree