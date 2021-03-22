﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Project
{
    public class BMP_File
    {
        private UInt32 filesize;
        private Picture _picture;
        private List<Byte> headerInfo;

        BMP_File(string path)
        {
            headerInfo = new List<byte>();
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                reader.ReadBytes(2);//BM
                filesize = reader.ReadUInt32();
                headerInfo.AddRange(reader.ReadBytes(12));//reserved[2], headersize, infoSize
                UInt32 width = reader.ReadUInt32();
                UInt32 depth = reader.ReadUInt32();
                _picture = new Picture(width, depth);
                headerInfo.AddRange(reader.ReadBytes(28));//other info
                CreatePicture(reader);
            }
            
        }

        private void CreatePicture(BinaryReader br)
        {
            _picture.Pixels = new Pixel[_picture.Width, _picture.Depth];
            for (int i = 0; i < _picture.Width; i++)
            {
                for (int j = 0; j < _picture.Depth; j++)
                {
                    _picture.Pixels[i, j] = new Pixel(br.ReadByte(), br.ReadByte(), br.ReadByte());
                }
            }
        }

        BMP_File(string path, double numberOfTimes, BMP_File previousFile)
        {
            headerInfo = previousFile.headerInfo;
            _picture = new Picture(Convert.ToUInt32(previousFile._picture.Width * numberOfTimes),
                Convert.ToUInt32(previousFile._picture.Depth * numberOfTimes));
            _picture.Pixels = CreateNewPicture(previousFile._picture.Pixels);
            filesize = Convert.ToUInt32(previousFile.filesize * numberOfTimes);
        }

        private void Write(string path)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write('B');
                writer.Write('M');
                writer.Write(filesize);
                writer.Write(new Byte[]{0, 0});
                for (int i = 2; i < 12; i++)
                {
                    writer.Write(headerInfo[i]);
                }
                writer.Write(_picture.Width);
                writer.Write(_picture.Depth);
                for (int i = 12; i < headerInfo.Count; i++)
                {
                    writer.Write(headerInfo[i]);
                }
            }
        }
    }
}