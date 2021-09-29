//https://gist.github.com/atruskie/bfb7e9ee3df954a29cbc17bdf12405f9

using System;

namespace Egret.Cli.Models
{
    public interface ITypeDiscriminator
    {
        Type BaseType { get; }

        bool TryResolve(ParsingEventBuffer buffer, out Type suggestedType);
    }
}