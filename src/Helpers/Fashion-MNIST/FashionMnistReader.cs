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

            var magicNumber = imagesReader.ReadBigInt32();
            var numberOfImages = imagesReader.ReadBigInt32();
            var width = imagesReader.ReadBigInt32();
            var height = imagesReader.ReadBigInt32();

            var magicLabel = labelsReader.ReadBigInt32();
            var numberOfLabels = labelsReader.ReadBigInt32();

            for (var imageIndex = 0; imageIndex < numberOfImages; imageIndex++)
            {
                var bytes = imagesReader.ReadBytes(height * width);
                var data = new byte[height, width];
                for (var rowIndex = 0; rowIndex < height; rowIndex++)
                {
                    for (var columnIndex = 0; columnIndex < width; columnIndex++)
                    {
                        data[rowIndex, columnIndex] = bytes[rowIndex * width + columnIndex];
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