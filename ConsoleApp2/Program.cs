using Microsoft.Extensions.FileSystemGlobbing;
using Semver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Egret.Cli.Models;
using Egret.Cli.Serialization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Converters;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;

namespace ConsoleApp2
{
    public class Program
    {
        #region filedefs

        public enum ESearchKeys
        {
            Hash,
            Ext,
            Name,
            Limit,
        }

        public enum EWolvenKitFile
        {
            Cr2w,
            Redscript,
            Tweak
        }

        public enum ERedScriptExtension
        {
            SWIFT
        }

        public enum ETweakExtension
        {
            TWEAK
        }

        public class AddFileModel
        {
            public string Name { get; set; }

            public string Description { get; set; }

            public string Icon { get; }

            public string Extension { get; set; }

            public EWolvenKitFile Type { get; set; }
        }

        public class FileCategoryModel
        {
            public string Name { get; set; }

            public string Description { get; set; }

            public List<AddFileModel> Files { get; set; }
        }

        public class WolvenKitFileDefinitions
        {
            public WolvenKitFileDefinitions()
            {
                Categories.Add(new FileCategoryModel()
                    {
                        Name = "Redscript",
                        Files = new[]
                        {
                            new AddFileModel(){
                                Name = "Redscript file",
                                Description = "A blank redscript file",
                                Extension = ERedScriptExtension.SWIFT.ToString(),
                                Type = EWolvenKitFile.Redscript
                            },
                        }.ToList()
                    }
                );
                Categories.Add(new FileCategoryModel()
                    {
                        Name = "Tweak DB",
                        Files = new[]
                        {
                            new AddFileModel(){
                                Name = "Tweak file",
                                Description = "A blank tweak delta file",
                                Extension = ETweakExtension.TWEAK.ToString(),
                                Type = EWolvenKitFile.Tweak
                            },
                        }.ToList()
                    }
                );
            }

            public List<FileCategoryModel> Categories { get; set; } = new();
        }

        #endregion

        public interface ITweakObject
        {

        }

        

        public class CName : IType
        {
            public string Name => "CName";
            public string Text { get; set; }
        }
        public class CFloat : IType
        {
            public string Name => "Float";
            public float Value {  get; set; }
            public static implicit operator CFloat(float value) => new() { Value = value };
        }
        public class CVector2 : IType
        {
            public CFloat X { get; set; } = new();
            public CFloat Y { get; set; } = new();

            public string Name => "Vector2";

        }

        public interface IType : ITweakObject
        {
            public string Name { get; }

        }
        //public class Group : ITweakObject
        //{
        //    public string Inherits { get; set; }
        //    public Dictionary<string, IType> Members { get; set; } = new();
        //}
        public class Record : ITweakObject
        {
            public string Type { get; set; }
            public string Inherits { get; set; }
            //public Dictionary<string, Group> Groups { get; set; } = new();
            public Dictionary<string,IType> Members { get; set; } = new();
        }
        public class TweakDto
        {
            public string Version { get; set; } = "1.0";
            public Dictionary<string, IType> Flats { get; set; } = new();
            //public Dictionary<string, Group> Groups { get; set; } = new();
            public Dictionary<string, Record> Groups { get; set; } = new();
        }


        static void Main(string[] args)
        {
            TestYaml();


            //TestFileDefs();
            //TestGlob();
        }

        private static void TestYaml()
        {

            var cname = new CName() { Text = "test" };
            CFloat cfloat = 3.8f;
            var cvector = new CVector2() { X =  1.2f, Y = 2.3f };

            var flats = new Dictionary<string, IType>()
            {
                {"a", cname},
                {"b", cfloat},
                {"c", cvector}
            };

            //var g = new Group()
            //{
            //    Inherits = "parentgroup1",
            //    Members = flats
            //};
            //var g2 = new Group()
            //{
            //    Members = flats
            //};

            //var groups = new Dictionary<string, Group>()
            //{
            //    { "group1", g },
            //    { "group2", g2 }
            //};

            var r = new Record()
            {
                Type = "recordType",
                Inherits = "parentRecord1",
                Members = flats,
                //Groups = groups
            };

            var dto = new TweakDto()
            {
                Flats = flats,
                Groups = new Dictionary<string, Record>()
                {
                    { "record1", r }
                },
                //Groups = groups
            };



            var yaml = SerializeYaml(dto);

            Console.WriteLine(yaml);

            var newDict = DeSerializeYaml(yaml);




            static string SerializeYaml(TweakDto dto)
            {
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .WithTypeConverter(new TweakTypeConverter())
                    //.WithTypeConverter(new TweakgroupConverter())
                    .WithTypeConverter(new TweakRecordConverter())
                    .WithTypeConverter(new TweakDtoConverter())
                    .DisableAliases()
                    .Build();
                return serializer.Serialize(dto);
            }


            static TweakDto DeSerializeYaml(string yaml)
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .WithTypeConverter(new TweakTypeConverter())
                    .WithTypeConverter(new TweakDtoConverter())
                    //.WithTypeConverter(new TweakgroupConverter())
                    
                    .WithTypeConverter(new TweakRecordConverter())
                    .Build();

                return deserializer.Deserialize<TweakDto>(yaml);
            }
        }


        private static void TestFileDefs()
        {
            var def = new WolvenKitFileDefinitions();

            var serializer = new XmlSerializer(typeof(WolvenKitFileDefinitions));

            using (var sw = new StringWriter())
            {
                //doc.Save(sw);
                serializer.Serialize(sw, def);

                var xml = sw.ToString();
                File.WriteAllText(@"WolvenKitFileDefinitions.xml", xml);

                // test loading 
                using (var sr = new StringReader(xml))
                {
                    var newdef = (WolvenKitFileDefinitions)serializer.Deserialize(sr);
                }
            }
            

        }


        private static void TestGlob()
        {
            Dictionary<ESearchKeys, List<string>> KeyDict =
                Enum.GetValues<ESearchKeys>().ToDictionary(key => key, key => new List<string>());
            // defaults
            var hashes = KeyDict[ESearchKeys.Hash]
                .Where(x => ulong.TryParse(x, out _))
                .Select(ulong.Parse);

            var allfilesList = new List<string>()
            {
                "judy.mesh",
                "_judy_.ent",
                "whatever",
                "whatever/est/test/ee.xbm",
                "base/textures/characters/judy/file.ent",
                "base/textures/characters/judyxxoxoxox/file.ent",
                "base/textures/characters/noice/judy_file.ent",
                "base/textures/characters/noice/judy_file.xbm",
                "base\\textures\\characters\\judy\\file.ent",
                "base\\textures\\characters\\judyxxoxoxox\\file.ent",
                "base\\textures\\characters\\noice\\judy_file.ent",
                "base\\textures\\characters\\noice\\judy_file.mesh",
                @"base\localization\common\vo\civ_mid_m_85_mex_30_mt_vista_del_rey_f_1ed3f72f92559000.wem",
                "base\\localization\\common\\vo\\civ_mid_m_85_mex_30_mt_vista_del_rey_f_1ed3f72f92559000.wem"
            };

            var list = File.ReadAllLines(@"X:\cp77\archivehashes.txt");

            //var mainsearch = @"judy";
            //var options = @"*.ent";
            //var builtmatch = $"{}";

            var matcher = new Matcher();

            //matcher.AddInclude(@"**/*judy*.ent");
            //matcher.AddInclude("**/*.wem");
            matcher.AddInclude("base/localization/**/*");
            //matcher.AddInclude("/*judy*."); // without the point, all extensions get added
            //matcher.AddInclude("**/*judy*/*");

            var sw = new Stopwatch();
            sw.Start();
            var res1 = matcher.Match(list).Files.Select(x => x.Path).ToList();
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            Console.WriteLine(res1.Count);

            sw.Reset();
            sw.Start();
            var res2 = list.Where(x => matcher.Match(x).HasMatches).ToList();
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            Console.WriteLine(res2.Count);
        }
    }
}
