// WARNING: Auto generated code. Modifications will be lost!
#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace GlobExpressions.AST
{
    internal sealed class DirectorySegment : Segment
    {
        public SubSegment[] SubSegments { get; }

        public DirectorySegment(IEnumerable<SubSegment> subSegments)
            : base(GlobNodeType.DirectorySegment)
        {
            SubSegments = subSegments.ToArray();
        }

        public override string ToString()
        {
            return string.Join("", SubSegments.Select(x => x.ToString()));
        }
    }
}

#nullable disable
