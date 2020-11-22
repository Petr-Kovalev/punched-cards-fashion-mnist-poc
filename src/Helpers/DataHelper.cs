using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PunchedCards.Helpers.FashionMNIST;

namespace PunchedCards.Helpers
{
    internal static class DataHelper
    {
        internal const int LabelsCount = 10;

        internal static IEnumerable<Tuple<BitArray, string>> ReadTrainingData()
        {
            return ReaData(FashionMnistReader.ReadTrainingData);
        }

        internal static IEnumerable<Tuple<BitArray, string>> ReadTestData()
        {
            return ReaData(FashionMnistReader.ReadTestData);
        }

        private static IEnumerable<Tuple<BitArray, string>> ReaData(Func<IEnumerable<Image>> readImagesFunction)
        {
            return readImagesFunction()
                .Select(image => new Tuple<BitArray, string>(
                    GetValueBitArray(image.Data),
                    BinaryStringsHelper.GetLabelString(image.Label, LabelsCount)));
        }

        private static BitArray GetValueBitArray(byte[,] imageData)
        {
            const byte width = 28;
            const byte height = 28;
            const int pixelRepresentationSizeInBits = 8;

            var result = new BitArray(width * height * pixelRepresentationSizeInBits);

            for (byte i = 0; i < width; i++)
            {
                for (byte j = 0; j < height; j++)
                {
                    var startIndex = (i * height + j) * pixelRepresentationSizeInBits;

                    byte bitIndex = 0;
                    foreach (var bit in GetPixelRepresentationInBits(imageData[i, j]))
                    {
                        result[startIndex + bitIndex] = bit;
                        bitIndex++;
                    }
                }
            }

            return result;
        }

        private static IEnumerable<bool> GetPixelRepresentationInBits(byte b)
        {
            yield return (b & 128) != 0;
            yield return (b & 64) != 0;
            yield return (b & 32) != 0;
            yield return (b & 16) != 0;
            yield return (b & 8) != 0;
            yield return (b & 4) != 0;
            yield return (b & 2) != 0;
            yield return (b & 1) != 0;
        }
    }
}