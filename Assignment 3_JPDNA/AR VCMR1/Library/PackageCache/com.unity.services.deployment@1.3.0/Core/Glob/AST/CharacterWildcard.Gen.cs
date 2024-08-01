// WARNING: Auto generated code. Modifications will be lost!
#nullable enable
namespace GlobExpressions.AST
{
    internal sealed class CharacterWildcard : SubSegment
    {
        public static readonly CharacterWildcard Default = new CharacterWildcard();

        private CharacterWildcard()
            : base(GlobNodeType.CharacterWildcard)
        {
        }

        public override string ToString() => "?";
    }
}

#nullable disable
