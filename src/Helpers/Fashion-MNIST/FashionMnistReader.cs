using System;
using System.Collections.Generic;
using System.IO;

namespace PunchedCards.Helpers.FashionMNIST
{
    // https://github.com/zalandoresearch/fashion-mnist
    internal static class FashionMnistReader
    {
        private const string TrainImagesFileName = "fashion-mnist/train-images-idx3-ubyte";
        private const string TrainLabelsFileName = "fashion-mnist/train-labels-idx1-ubyte";
        private const string TestImagesFileName = "fashion-mnist/t10k-images-idx3-ubyte";
        private const string TestLabelsFileName = "fashion-mnist/t10k-labels-idx1-ubyte";

        internal static IEnumerable<Image> ReadTrainingData()
        {
            return Read(TrainImagesFileName, TrainLabelsFileName);
        }

        internal static IEnumerable<Image> ReadTestData()
        {
            return Read(TestImagesFileName, TestLabelsFileName);
        }

        private static IEnumerable<Image> Read(string imagesPath, string labelsPath)
        {
            using var labelsFileStream = File.OpenRead(labelsPath);
            using var labelsReader = new BinaryReader(labelsFileStream);
            using var imagesFileStream = File.OpenRead(imagesPath);
            using var imagesReader = new BinaryReader(imagesFileStream);

            int magicNumber = imagesReader.ReadBigInt32();
            int numberOfImages = imagesReader.ReadBigInt32();
            int width = imagesReader.ReadBigInt32();
            int height = imagesReader.ReadBigInt32();

            int magicLabel = labelsReader.ReadBigInt32();
            int numberOfLabels = labelsReader.ReadBigInt32();

            for (int imageIndex = 0; imageIndex < numberOfImages; imageIndex++)
            {
                var bytes = imagesReader.ReadBytes(width * height);
                var data = new byte[height, width];
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        data[i, j] = bytes[i * height + j];
                    }
                }

                var label = labelsReader.ReadByte();

                yield return new Image
                {
                    Data = data,
                    Label = label
                };
            }
        }

        private static int ReadBigInt32(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(int));
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToInt32(bytes, 0);
        }
    }
}