// WARNING: Auto generated code. Modifications will be lost!
#nullable enable
namespace GlobExpressions.AST
{
    internal sealed class StringWildcard : SubSegment
    {
        public static readonly StringWildcard Default = new StringWildcard();

        private StringWildcard()
            : base(GlobNodeType.StringWildcard)
        {
        }

        public override string ToString() => "*";
    }
}

#nullable disable
