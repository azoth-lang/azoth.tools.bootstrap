using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.IL
{
    public class ClassIL
    {
        public FixedList<MemberIL> Members { get; }

        public ClassIL(FixedList<MemberIL> members)
        {
            Members = members;
        }
    }
}
