using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CVEnhancer.Utils
{
    public static class ImageByteConverter
    {
        public static byte[] defaultImage = new HttpClient().GetByteArrayAsync("https://picsum.photos/200").Result;
        public static async Task<byte[]> StreamToByteArrayAsync(Stream inputStream)
        {
            if (inputStream == null) return null;

            using (var memoryStream = new MemoryStream())
            {
                // Kopiujemy strumień pliku do pamięci RAM
                await inputStream.CopyToAsync(memoryStream);

                // Zwracamy tablicę bajtów gotową do zapisu w bazie
                return memoryStream.ToArray();
            }
        }

        public static ImageSource ByteArrayToImageSource(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null; // Lub zwróć jakiś defaultowy obrazek
            }

            // ImageSource.FromStream wymaga funkcji zwracającej strumień
            return ImageSource.FromStream(() => new MemoryStream(bytes));
        }
    }
}
