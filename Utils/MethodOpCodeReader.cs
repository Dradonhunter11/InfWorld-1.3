using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS0618 // Obsolete member

namespace InfWorld.Utils
{
    public struct MethodOpCodeReader : IEnumerator<KeyValuePair<OpCode, ResolvedOpcodeWrapper>>, IDisposable
    {
        KeyValuePair<OpCode, ResolvedOpcodeWrapper> current;
        BinaryReader reader;
        Module module;

        public KeyValuePair<OpCode, ResolvedOpcodeWrapper> Current => current;
        object IEnumerator.Current => current;

        public MethodOpCodeReader(byte[] array, Module module)
        {
            reader = new BinaryReader(new MemoryStream(array, writable: false));
            this.module = module;
            current = default;
        }

        public void Dispose()
        {
            reader?.Dispose();
            reader = null;
            module = null;
            current = default;
        }

        public bool MoveNext()
        {
            //if (reader is null || stream is null) throw new ObjectDisposedException(nameof(MethodOpCodeReader));
            var baseStream = reader.BaseStream;
            if (baseStream.Position + 2 + 4 >= baseStream.Length) // if it might read outside the stream
                return CheckedRead();

            byte b = reader.ReadByte();
            OpCode opcode = b != 254 ? OpCodeUtils.oneSizeOpcodes[b] : OpCodeUtils.twoSizeOpcodes[reader.ReadByte()];
            current = new KeyValuePair<OpCode, ResolvedOpcodeWrapper>(opcode, new ResolvedOpcodeWrapper(opcode, reader, module));

            return true;
        }

        private bool CheckedRead()
        {
            var baseStream = reader.BaseStream;
            if (baseStream.Position >= baseStream.Length)
                return false;
            byte b = reader.ReadByte();
            OpCode opCode;
            if (b != 254)
            {
                opCode = OpCodeUtils.oneSizeOpcodes[b];
            }
            else
            {
                if (baseStream.Position >= baseStream.Length) return false; // check before reading the 2nd byte
                opCode = OpCodeUtils.twoSizeOpcodes[reader.ReadByte()];
            }

            if (baseStream.Position + OpCodeUtils.ArgSize(opCode) > baseStream.Length) return false; // couldn't read the arg
            current = new KeyValuePair<OpCode, ResolvedOpcodeWrapper>(opCode, new ResolvedOpcodeWrapper(opCode, reader, module));
            return true;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }  
    }

    public struct ResolvedOpcodeWrapper
    {
        ValueTuple<OpCode, object, Module> tuple;
        bool resolved;

        public bool IsResolved => resolved;

        public ResolvedOpcodeWrapper(OpCode opcode, BinaryReader reader, Module module)
        {
            resolved = false;

            object r = Read(opcode, reader);
            tuple = new ValueTuple<OpCode, object, Module>(opcode, r, module); 
        }

        static object Read(OpCode opcode, BinaryReader reader)
        {
            switch (opcode.OperandType)
            {
                case OperandType.InlineBrTarget:
                case OperandType.InlineI:
                case OperandType.InlineTok:
                case OperandType.InlineField:
                case OperandType.InlineMethod:
                case OperandType.InlineSig:
                case OperandType.InlineString:
                case OperandType.InlineType:
                    return reader.ReadInt32();
                case OperandType.InlineI8:
                    return reader.ReadInt64();
                case OperandType.InlineVar:
                    return reader.ReadInt16();
                case OperandType.ShortInlineR:
                    return reader.ReadSingle();
                case OperandType.InlineR:
                    return reader.ReadDouble();
                case OperandType.InlineSwitch:
                    int labelCount = reader.ReadInt32();
                    int[] arr = new int[labelCount + 1];
                    arr[0] = labelCount; // first element in the array is the labels
                    for (int i = 1; i <= labelCount; i++)
                        arr[i] = reader.ReadInt32(); // read offsets
                    return arr;
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                    return reader.ReadSByte();
                case OperandType.ShortInlineVar:
                   return reader.ReadByte();
                case OperandType.InlineNone:  // no argument
                    return null;
                default:
                    throw new ArgumentException("OpCode not supported", nameof(opcode));
            }
        }

        public bool TryResolve(out object value)
        {
            return TryResolve(out value, out _);
        }

        public bool TryResolve(out object value, out Exception loadException)
        {
            try
            {
                if (!resolved)
                    Resolve();
            }
            catch (Exception e)
            {
                loadException = e;
                value = null;
                return false;
            }
            loadException = null;
            value = tuple.Item2;
            return true;
        }

        void Resolve()
        {
            var t = tuple;
            switch (t.Item1.OperandType)
            {
                case OperandType.InlineField: tuple.Item2 = t.Item3.ResolveField((int)t.Item2); break;
                case OperandType.InlineMethod: tuple.Item2 = t.Item3.ResolveMethod((int)t.Item2); break;
                case OperandType.InlineSig: tuple.Item2 = t.Item3.ResolveSignature((int)t.Item2); break;
                case OperandType.InlineString: tuple.Item2 = t.Item3.ResolveString((int)t.Item2); break;
                case OperandType.InlineType: tuple.Item2 = t.Item3.ResolveType((int)t.Item2); break;
                case OperandType.InlineTok: tuple.Item2 = t.Item3.ResolveMember((int)t.Item2); break;

                case OperandType.InlineBrTarget:
                case OperandType.InlineI:
                case OperandType.InlineI8:
                case OperandType.InlineNone:
                case OperandType.InlinePhi:
                case OperandType.InlineR:
                case OperandType.InlineSwitch:
                case OperandType.InlineVar:
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineR:
                case OperandType.ShortInlineVar:
                default: // no change
                    //value = t.Item2;
                    break;
            }
            resolved = true;
        }

        public object GetValue()
        {
            if (!resolved)
                Resolve();
            return tuple;
        }
    }
    /*private static IEnumerable<KeyValuePair<OpCode, Func<object>>> ReadOpcodes(Stream readStream, MethodInfo methodSource)
    {
        BinaryReader reader = new BinaryReader(readStream);
        while (readStream.Position < readStream.Length)
        {
            byte firstByte = reader.ReadByte();
            OpCode opcode = firstByte != 254 ? oneByteOpCodes[firstByte] : twoByteOpCodes[reader.ReadByte()];
            Func<object> getOperand = new ResolvedOpcodeWrapper(opcode, reader, methodSource).GetValue;
            yield return new KeyValuePair<OpCode, Func<object>>(opcode, getOperand);
        }
    }*/
}