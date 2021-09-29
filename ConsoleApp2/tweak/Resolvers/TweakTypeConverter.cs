using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using ConsoleApp2;
using Egret.Cli.Models;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Egret.Cli.Serialization
{

    //public class TweakgroupConverter : IYamlTypeConverter
    //{
    //    private readonly IValueSerializer serializer = new SerializerBuilder()
    //        .WithNamingConvention(CamelCaseNamingConvention.Instance)
    //        .WithTypeConverter(new TweakTypeConverter())
    //        .DisableAliases()
    //        .BuildValueSerializer();

    //    public bool Accepts(Type type) => type == typeof(Program.Group);

    //    public object ReadYaml(IParser parser, Type type)
    //    {
    //        throw new NotImplementedException();





    //    }

    //    public void WriteYaml(IEmitter emitter, object value, Type type)
    //    {
    //        if (value is not Program.Group group)
    //        {
    //            throw new YamlException(type.ToString());
    //        }

    //        // inherits
    //        TagName tag = null;
    //        if (group.Inherits != null)
    //        {
    //            tag = new TagName(group.Inherits);
    //        }
    //        emitter.Emit(new MappingStart(null, tag, false, MappingStyle.Block));

    //        // reset of serialisation code
    //        emitter.Emit(new Scalar(null, nameof(Program.Group.Members)));

    //        serializer.SerializeValue(emitter, group.Members, typeof(Dictionary<string, Program.IType>));

    //        emitter.Emit(new MappingEnd());
    //    }

    //}

    public class TweakDtoConverter : IYamlTypeConverter
    {
        private readonly IValueSerializer serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTypeConverter(new TweakTypeConverter())
            .WithTypeConverter(new TweakRecordConverter())
            .DisableAliases()
            .BuildValueSerializer();


        public bool Accepts(Type type) => type == typeof(Program.TweakDto);

        public object ReadYaml(IParser parser, Type type)
        {
            Program.TweakDto result = new Program.TweakDto();

            parser.ReadMappingStart();

            result.Version = parser.SafeReadScalarProperty(nameof(Program.TweakDto.Version));



            parser.MoveNext(); // skip the mapping end (or crash)
            return result;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            if (value is not Program.TweakDto dto)
            {
                throw new YamlException(type.ToString());
            }

            emitter.Emit(new MappingStart(null, null, false, MappingStyle.Block));

            emitter.WriteProperty(nameof(Program.TweakDto.Version), dto.Version);

            emitter.Emit(new Scalar(null, nameof(Program.TweakDto.Flats)));
            serializer.SerializeValue(emitter, dto.Flats, typeof(Dictionary<string, Program.IType>));

            emitter.Emit(new Scalar(null, nameof(Program.TweakDto.Groups)));
            serializer.SerializeValue(emitter, dto.Groups, typeof(Dictionary<string, Program.Record>));

            emitter.Emit(new MappingEnd());
        }

    }


    public class TweakRecordConverter : IYamlTypeConverter
    {
        private readonly IValueSerializer serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTypeConverter(new TweakTypeConverter())
            //.WithTypeConverter(new TweakgroupConverter())
            .DisableAliases()
            .BuildValueSerializer();

        public bool Accepts(Type type) => type == typeof(Program.Record);

        public object ReadYaml(IParser parser, Type type)
        {
            Program.Record result = new Program.Record();

            parser.ReadMappingStart();

            // read inherits
            var e = parser.Current;

            // read Members


            parser.MoveNext(); // skip the mapping end (or crash)

            throw new NotImplementedException();

            return result;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            if (value is not Program.Record record)
            {
                throw new YamlException(type.ToString());
            }

            // write inherits
            TagName tag = null;
            if (record.Inherits != null)
            {
                tag = new TagName(record.Inherits);
            }
            emitter.Emit(new MappingStart(null, tag, false, MappingStyle.Block));

            // write Members
            emitter.Emit(new Scalar(null, nameof(Program.Record.Members)));
            serializer.SerializeValue(emitter, record.Members, typeof(Dictionary<string, Program.IType>));

            //emitter.Emit(new Scalar(null, nameof(Program.Record.Groups)));
            //serializer.SerializeValue(emitter, record.Groups, typeof(Dictionary<string, Program.Group>));

            emitter.Emit(new MappingEnd());
        }

    }


    public class TweakTypeConverter : IYamlTypeConverter
    {
        private const string ValueName = "value";
        private const string TypeName = "type";

        public bool Accepts(Type type) => typeof(Program.IType).IsAssignableFrom(type);

        public object ReadYaml(IParser parser, Type type)
        {
            Program.IType result;

            parser.ReadMappingStart();


            // read name property
            var typeName = parser.SafeReadScalarProperty(TypeName);
            
            // get type
            var nodetype = GetRedTypeFromString(typeName);
            var obj = Activator.CreateInstance(nodetype);

            // parse value
            switch (obj)
            {
                case Program.CName cname:
                    cname.Text = parser.SafeReadScalarProperty(ValueName);
                    result = cname;
                    break;
                case Program.CFloat cfloat:
                    cfloat.Value  = float.Parse(parser.SafeReadScalarProperty(ValueName));
                    result = cfloat;
                    break;
                case Program.CVector2 cVector2:

                    // read value
                    var valueName = parser.ReadScalarValue();
                    if (valueName != ValueName)
                    {
                        throw new InvalidDataException($"Unexpected scalar value '{valueName}'. Expected value: '{ValueName}'");
                    }

                    parser.ReadMappingStart();

                    cVector2.X.Value = float.Parse(parser.SafeReadScalarProperty( nameof(Program.CVector2.X)));
                    cVector2.Y.Value = float.Parse(parser.SafeReadScalarProperty( nameof(Program.CVector2.Y)));

                    parser.MoveNext(); // skip the mapping end (or crash)

                    result = cVector2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(nodetype));
            }



            parser.MoveNext(); // skip the mapping end (or crash)

            return result;
        }

        

        private static Type GetRedTypeFromString(string redtype)
        {
            return redtype switch
            {
                "CName" => typeof(Program.CName),
                "Float" => typeof(Program.CFloat),
                "Vector2" => typeof(Program.CVector2),
                _ => throw new InvalidDataException($"Unexpected type value '{redtype}'")
            };
        }

        


        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            if (value is not Program.IType itype)
            {
                throw new YamlException(type.ToString());
            }


            emitter.Emit(new MappingStart(null, null, false, MappingStyle.Block));

            emitter.WriteProperty(TypeName, itype.Name);


            switch (value)
            {
                case Program.CVector2 node:
                {
                    emitter.Emit(new Scalar(null, ValueName));

                    emitter.Emit(new MappingStart(null, null, false, MappingStyle.Block));

                    emitter.WriteProperty(nameof(Program.CVector2.X), node.X.Value.ToString(CultureInfo.InvariantCulture));
                    emitter.WriteProperty(nameof(Program.CVector2.Y), node.Y.Value.ToString(CultureInfo.InvariantCulture));

                    emitter.Emit(new MappingEnd());
                    break;
                }
                case Program.CName n:
                {
                    emitter.WriteProperty(ValueName, n.Text);
                    break;
                }
                case Program.CFloat f:
                {
                    emitter.WriteProperty(ValueName, f.Value.ToString(CultureInfo.InvariantCulture));
                    break;
                }
                default:
                    throw new YamlException(type.ToString());
            }

            emitter.Emit(new MappingEnd());
        }


    }
}