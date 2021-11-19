using System;
using System.Reflection;
using System.Reflection.Emit;

#pragma warning disable CS0618 // Obsolete member

namespace InfWorld.Utils
{
    internal static class OpCodeUtils
    {
        internal static OpCode[] oneSizeOpcodes, twoSizeOpcodes;
        static OpCodeUtils()
        {
            oneSizeOpcodes = new OpCode[256];
            twoSizeOpcodes = new OpCode[256];
            Type t = typeof(OpCode);
            foreach (var field in typeof(OpCodes).GetFields())
            {
                if (field.FieldType != t) continue;
                OpCode opcode = (OpCode)field.GetValue(null);
                if (opcode.Size == 1)
                    oneSizeOpcodes[opcode.Value] = opcode;
                else
                    twoSizeOpcodes[opcode.Value & byte.MaxValue] = opcode;
            }
        }

        internal static int ArgSize(OpCode opcode)
        {
            switch (opcode.OperandType)
            {
                case OperandType.InlineNone:
                    return 0;
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                    return 1;
                case OperandType.InlineVar:
                    return 2;
                case OperandType.InlineBrTarget:
                case OperandType.InlineField:
                case OperandType.InlineI:
                case OperandType.InlineSig:
                case OperandType.InlineString:
                case OperandType.InlineSwitch:
                case OperandType.InlineMethod:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.ShortInlineR:
                    return 4;
                case OperandType.InlineR:
                case OperandType.InlineI8:
                    return 8;
                case OperandType.InlinePhi:
                default:
                    throw new ArgumentException("OpCode operand type is not supported");
            }
        }
    }

}

