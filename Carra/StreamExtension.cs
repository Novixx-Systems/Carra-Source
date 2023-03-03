// This source code should not be thought of as open source. It is on GitHub
// for learning purposes only. Do not create pull requests.
// If you find a game breaking bug, please do create an issue.
// ALSO NOTE THAT CARRA IS NO LONGER OBFUSCATED


// Copyright (c) 2023 Novixx Systems
using System;
using System.IO;

public static class StreamExtension
{
    private const Int32 BlockSize = 1024;

    public static Byte[] ReadToEnd(this Stream source)
    {
        MemoryStream buffer = new MemoryStream();
        Byte[] block = new Byte[StreamExtension.BlockSize];
        Int32 len = source.Read(block, 0, StreamExtension.BlockSize);
        while (len > 0)
        {
            buffer.Write(block, 0, len);
            len = source.Read(block, 0, StreamExtension.BlockSize);
        }

        Byte[] result = new Byte[buffer.Position];
        Array.Copy(buffer.GetBuffer(), result, result.Length);
        return result;
    }
}