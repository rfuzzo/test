using Egret.Cli.Models;
using System;
using System.Collections.Generic;
using ConsoleApp2;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using Scalar = YamlDotNet.Core.Events.Scalar;

namespace Egret.Cli.Serialization
{
    public class TweakTypeResolver : ITypeDiscriminator
    {
        private readonly Dictionary<string, Type> typeLookup;

        public TweakTypeResolver(INamingConvention namingConvention)
        {
            typeLookup = new Dictionary<string, Type>() {
                { namingConvention.Apply("Float"), typeof(Program.CFloat) },
                { namingConvention.Apply("Vector2"), typeof(Program.CVector2) },
                { namingConvention.Apply("CName"), typeof(Program.CName) },
            };
        }

        public Type BaseType => typeof(Program.IType);

        public bool TryResolve(ParsingEventBuffer buffer, out Type suggestedType)
        {
            if (buffer.TryFindMappingEntry(
                scalar => typeLookup.ContainsKey(scalar.Value),
                out Scalar key,
                out ParsingEvent _))
            {
                suggestedType = typeLookup[key.Value];
                return true;
            }

            suggestedType = null;
            return false;
        }
    }
}