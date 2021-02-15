using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.IR.Declarations;
using Azoth.Tools.Bootstrap.Framework;
using Azoth.Tools.Bootstrap.IL;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IR
{
    internal class ILBuilder
    {
        private readonly Dictionary<FunctionIR, FunctionIL> functions = new Dictionary<FunctionIR, FunctionIL>();
        private readonly Dictionary<MemberIR, MemberIL> members = new Dictionary<MemberIR, MemberIL>();
        public MemberIL Build(MemberIR member)
        {
            if (members.TryGetValue(member, out var memberIL)) return memberIL;

            memberIL = member switch
            {
                MethodIR method => new MethodIL(),
                FieldIR field => new FieldIL(),
                _ => throw ExhaustiveMatch.Failed(member)
            };

            members.Add(member, memberIL);
            return memberIL;
        }

        public ClassIL Build(ClassIR @class)
        {
            return new ClassIL(@class.Members.Select(Build).ToFixedList());
        }

        public FunctionIL? Lookup(FunctionIR? function)
        {
            return function != null && functions.TryGetValue(function, out var functionIL)
                ? functionIL : null;
        }
    }
}
