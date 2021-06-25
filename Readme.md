# Huffman Code
This program is an implementation of the huffman code in C#.
If you want to know how it works see [Huffman coding](https://en.wikipedia.org/wiki/Huffman_coding) or read the LiaScript..

## Short introduction
1. Create a new object Tree, and give it a message as string. Note: you can't change the message after you set it
2. Call tree.encode from this object, so the message gets encoded.
3. You can get the encoded string from tree.encodedMessage.
4. Decode the message with tree.decode.
5. You can get the decoded string from tree.message.

The last two step doesn't make sense yet, because I haven't managed it yet to implement store and load methods.
