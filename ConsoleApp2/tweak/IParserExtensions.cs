//https://gist.github.com/atruskie/bfb7e9ee3df954a29cbc17bdf12405f9

using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Egret.Cli.Models
{
    public static class IParserExtensions
    {
        public static bool TryFindMappingEntry(this ParsingEventBuffer parser, Func<Scalar, bool> selector, out Scalar key, out ParsingEvent value)
        {
            parser.Consume<MappingStart>();
            do
            {
                // so we only want to check keys in this mapping, don't descend
                switch (parser.Current)
                {
                    case Scalar scalar:
                        // we've found a scalar, check if it's value matches one
                        // of our  predicate
                        var keyMatched = selector(scalar);

                        // move head so we can read or skip value
                        parser.MoveNext();

                        // read the value of the mapping key
                        if (keyMatched)
                        {
                            // success
                            value = parser.Current;
                            key = scalar;
                            return true;
                        }

                        // skip the value
                        parser.SkipThisAndNestedEvents();

                        break;
                    case MappingStart or SequenceStart:
                        parser.SkipThisAndNestedEvents();
                        break;
                    default:
                        // do nothing, skip to next node
                        parser.MoveNext();
                        break;
                }
            } while (parser.Current is not null);

            key = null;
            value = null;
            return false;
        }

        public static void WriteProperty(this IEmitter emitter, string propertyname, string str)
        {
            emitter.Emit(new Scalar(null, propertyname));
            emitter.Emit(new Scalar(null, str));
        }


        public static void ReadMappingStart(this IParser parser)
        {
            if (parser.Current != null && parser.Current.GetType() != typeof(MappingStart))
            {
                throw new InvalidDataException("Invalid YAML content.");
            }
            parser.MoveNext();
        }
        public static string ReadScalarValue(this IParser parser)
        {
            if (parser.Current is not Scalar scalar)
            {
                throw new InvalidDataException("Failed to retrieve scalar value.");
            }
            var val = scalar.Value;
            parser.MoveNext();
            return val;
        }
        public static string SafeReadScalarProperty(this IParser parser, string propertyName)
        {
            var nodeName = ReadScalarValue(parser);

            var nodeValue = nodeName == propertyName
                ? ReadScalarValue(parser)
                : throw new InvalidDataException(
                    $"Unexpected scalar value '{nodeName}'. Expected value: '{propertyName}'");
            return nodeValue;
        }
    }
}