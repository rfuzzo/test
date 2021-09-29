using System;
using System.IO;
using System.Reflection;
using DirectXTexNet;

namespace ConsoleApp1
{
    class Program
    {
        const string ddsPath = @"X:\cp77\TEMP\rain_normal.dds";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fileName = "DirectXTexNetImpl.dll";
            string platform = Environment.Is64BitProcess ? "x64" : "x86";

            foreach (var filePath in new[]
            {
                Path.Combine(folder, fileName),
                Path.Combine(folder, platform, fileName),
                Path.Combine(folder, "runtimes", "win-" + platform, "native", fileName)
            })
            {
                if (File.Exists(filePath))
                {
                    DirectXTexNet.TexHelper.LoadInstanceFrom(filePath);
                }
            }



            var th = DirectXTexNet.TexHelper.Instance;

            var md = th.GetMetadataFromDDSFile(ddsPath, DDS_FLAGS.NONE);

            //var x = TexConv.Test();

            //using (var md = new TexMetadata())
            //{
            //    var x = DirectXTexSharp.Metadata.GetMetadataFromDDSFile(ddsPath, DDSFLAGS.DDS_FLAGS_NONE, md);

            //}

        }
    }
}
