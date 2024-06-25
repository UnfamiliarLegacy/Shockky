using Shockky.IO;
using Shockky.Lingo.AbstractSyntaxTree;
using Shockky.Lingo.AbstractSyntaxTree.Nodes;
using Shockky.Lingo.AbstractSyntaxTree.Nodes.Expressions;
using Shockky.Lingo.AbstractSyntaxTree.Nodes.Label;
using Shockky.Lingo.AbstractSyntaxTree.Nodes.Loop;
using Shockky.Lingo.AbstractSyntaxTree.Nodes.Statement;
using Shockky.Lingo.Enums;

// ReSharper disable NegativeEqualityExpression

namespace Shockky.Lingo;

public class LingoHandler
{
    private readonly LingoDecompiler _decompiler;
    private readonly LingoFunction _function;
    private readonly Stack<Node> _stack;

    private readonly List<string> _argumentNames;
    private readonly List<string> _localNames;
    private readonly List<string> _globalNames;
    private readonly List<Bytecode> _bytecode;
    private readonly Dictionary<uint, int> _bytecodePosMap;

    public LingoHandler(LingoDecompiler decompiler, LingoFunction function)
    {
        _decompiler = decompiler;
        _function = function;
        _stack = new Stack<Node>();
        _argumentNames = new List<string>(function.Arguments.Count);
        _localNames = new List<string>(function.Locals.Count);
        _globalNames = new List<string>(function.Globals.Count);
        _bytecode = new List<Bytecode>();
        _bytecodePosMap = new Dictionary<uint, int>();
    }

    public string? Name { get; private set; }
    public IReadOnlyList<Bytecode> Bytecode => _bytecode;
    public IReadOnlyList<string> ArgumentNames => _argumentNames;
    public IReadOnlyList<string> LocalNames => _localNames;
    public IReadOnlyList<string> GlobalNames => _globalNames;
    public bool IsGenericEvent { get; internal set; }

    private int VariableMultiplier()
    {
        if (_decompiler.Version >= 850) return 1;
        if (_decompiler.Version >= 500) return 8;
        return 6;
    }

    private Node Pop()
    {
        if (_stack.Count == 0)
        {
            return new ErrorNode();
        }

        return _stack.Pop();
    }

    private void ParseBytecode(ReadOnlySpan<byte> bytecode)
    {
        var reader = new ShockwaveReader(bytecode);

        while (reader.IsDataAvailable)
        {
            var pos = (uint)reader.Position;
            var op = reader.ReadByte();
            var opCode = (LingoOpCode)(op >= 0x40 ? 0x40 + op % 0x40 : op);
            int obj = 0;
            if (op >= 0xC0)
            {
                obj = reader.ReadInt32BigEndian();
            } 
            else if (op >= 0x80)
            {
                if (opCode == LingoOpCode.kOpPushInt16 || opCode == LingoOpCode.kOpPushInt8)
                {
                    // treat pushint's arg as signed
                    // pushint8 may be used to push a 16-bit int in older Lingo
                    obj = reader.ReadInt16BigEndian();
                }
                else
                {
                    obj = reader.ReadUInt16BigEndian();
                }
            } 
            else if (op >= 0x40)
            {
                if (opCode == LingoOpCode.kOpPushInt8)
                {
                    obj = (sbyte) reader.ReadByte();
                }
                else
                {
                    obj = reader.ReadByte();
                }
            }
        
            _bytecode.Add(new Bytecode(op, obj, pos));
            _bytecodePosMap[pos] = _bytecode.Count - 1;
        }
    }

    private void TagLoops()
    {
        // Tag any jmpifz which is a loop with the loop type
        // (kTagRepeatWhile, kTagRepeatWithIn, kTagRepeatWithTo, kTagRepeatWithDownTo).
        // Tag the instruction which `next repeat` jumps to with kTagNextRepeatTarget.
        // Tag any instructions which are internal loop logic with kTagSkip, so that
        // they will be skipped during translation.

        for (var startIndex = 0; startIndex < _bytecode.Count; startIndex++)
        {
            // All loops begin with jmpifz...
            var jmpifz = _bytecode[startIndex];
            if (jmpifz.OpCode != LingoOpCode.kOpJmpIfZ)
            {
                continue;
            }
            
            // ...and end with endrepeat.
            var jmpPos = (uint) (jmpifz.Pos + jmpifz.Obj);
            var endIndex = _bytecodePosMap[jmpPos];
            var endRepeat = _bytecode[endIndex - 1];
            if (endRepeat.OpCode != LingoOpCode.kOpEndRepeat || (endRepeat.Pos - endRepeat.Obj) > jmpifz.Pos)
            {
                continue;
            }

            var loopType = IdentifyLoop(startIndex, endIndex);
            _bytecode[startIndex].Tag = loopType;

            if (loopType == BytecodeTag.kTagRepeatWithIn) {
                for (int i = startIndex - 7, end = startIndex - 1; i <= end; i++)
                    _bytecode[i].Tag = BytecodeTag.kTagSkip;
                for (int i = startIndex + 1, end = startIndex + 5; i <= end; i++)
                    _bytecode[i].Tag = BytecodeTag.kTagSkip;
                _bytecode[endIndex - 3].Tag = BytecodeTag.kTagNextRepeatTarget; // pushint8 1
                _bytecode[endIndex - 3].OwnerLoop = startIndex;
                _bytecode[endIndex - 2].Tag = BytecodeTag.kTagSkip; // add
                _bytecode[endIndex - 1].Tag = BytecodeTag.kTagSkip; // endrepeat
                _bytecode[endIndex - 1].OwnerLoop = startIndex;
                _bytecode[endIndex].Tag = BytecodeTag.kTagSkip; // pop 3
            } else if (loopType == BytecodeTag.kTagRepeatWithTo || loopType == BytecodeTag.kTagRepeatWithDownTo) {
                int conditionStartIndex = _bytecodePosMap[(uint)(endRepeat.Pos - endRepeat.Obj)];
                _bytecode[conditionStartIndex - 1].Tag = BytecodeTag.kTagSkip; // set
                _bytecode[conditionStartIndex].Tag = BytecodeTag.kTagSkip; // get
                _bytecode[startIndex - 1].Tag = BytecodeTag.kTagSkip; // lteq / gteq
                _bytecode[endIndex - 5].Tag = BytecodeTag.kTagNextRepeatTarget; // pushint8 1 / pushint8 -1
                _bytecode[endIndex - 5].OwnerLoop = startIndex;
                _bytecode[endIndex - 4].Tag = BytecodeTag.kTagSkip; // get
                _bytecode[endIndex - 3].Tag = BytecodeTag.kTagSkip; // add
                _bytecode[endIndex - 2].Tag = BytecodeTag.kTagSkip; // set
                _bytecode[endIndex - 1].Tag = BytecodeTag.kTagSkip; // endrepeat
                _bytecode[endIndex - 1].OwnerLoop = startIndex;
            } else if (loopType == BytecodeTag.kTagRepeatWhile) {
                _bytecode[endIndex - 1].Tag = BytecodeTag.kTagNextRepeatTarget; // endrepeat
                _bytecode[endIndex - 1].OwnerLoop = startIndex;
            }
        }
    }

    private BytecodeTag IdentifyLoop(int startIndex, int endIndex)
    {
        if (IsRepeatWithin(startIndex, endIndex))
        {
            return BytecodeTag.kTagRepeatWithIn;
        }

        if (startIndex < 1)
        {
            return BytecodeTag.kTagRepeatWhile;
        }

        bool up;
        switch (_bytecode[startIndex - 1].OpCode)
        {
            case LingoOpCode.kOpLtEq:
                up = true;
                break;
            case LingoOpCode.kOpGtEq:
                up = true;
                break;
            default:
                return BytecodeTag.kTagRepeatWhile;
        }

        var endRepeat = _bytecode[endIndex - 1];
        var conditionStartIndex = _bytecodePosMap[(uint)(endRepeat.Pos - endRepeat.Obj)];
        if (conditionStartIndex < 1)
        {
            return BytecodeTag.kTagRepeatWhile;
        }

        LingoOpCode getOp;
        switch (_bytecode[conditionStartIndex - 1].OpCode)
        {
            case LingoOpCode.kOpSetGlobal:
                getOp = LingoOpCode.kOpGetGlobal;
                break;
            case LingoOpCode.kOpSetGlobal2:
                getOp = LingoOpCode.kOpGetGlobal2;
                break;
            case LingoOpCode.kOpSetProp:
                getOp = LingoOpCode.kOpGetProp;
                break;
            case LingoOpCode.kOpSetParam:
                getOp = LingoOpCode.kOpGetParam;
                break;
            case LingoOpCode.kOpSetLocal:
                getOp = LingoOpCode.kOpGetLocal;
                break;
            default:
                return BytecodeTag.kTagRepeatWhile;
        }

        var setOp = _bytecode[conditionStartIndex - 1].OpCode;
        var varId = _bytecode[conditionStartIndex - 1].Obj;

        if (!(_bytecode[conditionStartIndex].OpCode == getOp && _bytecode[conditionStartIndex].Obj == varId))
        {
            return BytecodeTag.kTagRepeatWhile;
        }

        if (endIndex < 5)
            return BytecodeTag.kTagRepeatWhile;
        if (up) {
            if (!(_bytecode[endIndex - 5].OpCode == LingoOpCode.kOpPushInt8 && _bytecode[endIndex - 5].Obj == 1))
                return BytecodeTag.kTagRepeatWhile;
        } else {
            if (!(_bytecode[endIndex - 5].OpCode == LingoOpCode.kOpPushInt8 && _bytecode[endIndex - 5].Obj == -1))
                return BytecodeTag.kTagRepeatWhile;
        }
        if (!(_bytecode[endIndex - 4].OpCode == getOp && _bytecode[endIndex - 4].Obj == varId))
            return BytecodeTag.kTagRepeatWhile;
        if (!(_bytecode[endIndex - 3].OpCode == LingoOpCode.kOpAdd))
            return BytecodeTag.kTagRepeatWhile;
        if (!(_bytecode[endIndex - 2].OpCode == setOp && _bytecode[endIndex - 2].Obj == varId))
            return BytecodeTag.kTagRepeatWhile;

        return up ? BytecodeTag.kTagRepeatWithTo : BytecodeTag.kTagRepeatWithDownTo;
    }

    internal bool IsRepeatWithin(int startIndex, int endIndex)
    {
        if (startIndex < 7 || startIndex > _bytecode.Count - 6)
		    return false;
	    if (!(_bytecode[startIndex - 7].OpCode == LingoOpCode.kOpPeek && _bytecode[startIndex - 7].Obj == 0))
		    return false;
	    if (!(_bytecode[startIndex - 6].OpCode == LingoOpCode.kOpPushArgList && _bytecode[startIndex - 6].Obj == 1))
		    return false;
	    if (!(_bytecode[startIndex - 5].OpCode == LingoOpCode.kOpExtCall && GetName(_bytecode[startIndex - 5].Obj) == "count"))
		    return false;
	    if (!(_bytecode[startIndex - 4].OpCode == LingoOpCode.kOpPushInt8 && _bytecode[startIndex - 4].Obj == 1))
		    return false;
	    if (!(_bytecode[startIndex - 3].OpCode == LingoOpCode.kOpPeek && _bytecode[startIndex - 3].Obj == 0))
		    return false;
	    if (!(_bytecode[startIndex - 2].OpCode == LingoOpCode.kOpPeek && _bytecode[startIndex - 2].Obj == 2))
		    return false;
	    if (!(_bytecode[startIndex - 1].OpCode == LingoOpCode.kOpLtEq))
		    return false;
	    // if (!(_bytecodes[startIndex].OpCode == kOpJmpIfZ))
	    //     return false;
	    if (!(_bytecode[startIndex + 1].OpCode == LingoOpCode.kOpPeek && _bytecode[startIndex + 1].Obj == 2))
		    return false;
	    if (!(_bytecode[startIndex + 2].OpCode == LingoOpCode.kOpPeek && _bytecode[startIndex + 2].Obj == 1))
		    return false;
	    if (!(_bytecode[startIndex + 3].OpCode == LingoOpCode.kOpPushArgList && _bytecode[startIndex + 3].Obj == 2))
		    return false;
	    if (!(_bytecode[startIndex + 4].OpCode == LingoOpCode.kOpExtCall && GetName(_bytecode[startIndex + 4].Obj) == "getAt"))
		    return false;
	    if (!(_bytecode[startIndex + 5].OpCode == LingoOpCode.kOpSetGlobal ||
              _bytecode[startIndex + 5].OpCode == LingoOpCode.kOpSetProp || 
              _bytecode[startIndex + 5].OpCode == LingoOpCode.kOpSetParam || 
              _bytecode[startIndex + 5].OpCode == LingoOpCode.kOpSetLocal))
		    return false;

	    if (endIndex < 3)
		    return false;
	    if (!(_bytecode[endIndex - 3].OpCode == LingoOpCode.kOpPushInt8 && _bytecode[endIndex - 3].Obj == 1))
		    return false;
	    if (!(_bytecode[endIndex - 2].OpCode == LingoOpCode.kOpAdd))
		    return false;
	    // if (!(_bytecodes[startIndex - 1].OpCode == kOpEndRepeat))
	    //     return false;
	    if (!(_bytecode[endIndex].OpCode == LingoOpCode.kOpPop && _bytecode[endIndex].Obj == 3))
		    return false;

	    return true;
    }

    private void TranslateBytecodes()
    {
        TagLoops();
        
        var ast = new AST(this);
        var i = 0;
        
        while (i < _bytecode.Count)
        {
            var bytecode = _bytecode[i];
            var pos = bytecode.Pos;

            while (pos == ast.CurrentBlock.EndPos)
            {
                var exitedBlock = ast.CurrentBlock;
                var ancenstorStmt = ast.CurrentBlock.AncestorStatement();
                ast.ExitBlock();
                if (ancenstorStmt != null)
                {
                    if (ancenstorStmt.Type == NodeType.kIfStmtNode)
                    {
                        var ifStatement = (IfStmtNode)ancenstorStmt;
                        if (ifStatement.HasElse && exitedBlock == ifStatement.Block1)
                        {
                            ast.EnterBlock(ifStatement.Block2);
                        }
                    } 
                    else if (ancenstorStmt.Type == NodeType.kCaseStmtNode)
                    {
                        var caseStmt = (CaseStmtNode)ancenstorStmt;
                        var caseLabel = ast.CurrentBlock.CurrentCaseLabel;
                        if (caseLabel != null)
                        {
                            if (caseLabel.Expect == CaseExpect.kCaseExpectOtherwise)
                            {
                                ast.CurrentBlock.CurrentCaseLabel = null;
                                caseStmt.AddOtherwise();
                                var otherwiseIndex = _bytecodePosMap[caseStmt.PotentialOtherwisePos];
                                _bytecode[otherwiseIndex].Translation = caseStmt.Otherwise;
                                ast.EnterBlock(caseStmt.Otherwise!.Block);
                            } 
                            else if (caseLabel.Expect == CaseExpect.kCaseExpectEnd)
                            {
                                ast.CurrentBlock.CurrentCaseLabel = null;
                            }
                        }
                    }
                }
            }
            
            var translateSize = TranslateBytecode(ast, _bytecode[i], i);
            i += translateSize;
        }
    }

    private int TranslateBytecode(AST ast, Bytecode bytecode, int index)
    {
        if (bytecode.Tag == BytecodeTag.kTagSkip || 
            bytecode.Tag == BytecodeTag.kTagNextRepeatTarget)
        {
            // This is internal loop logic. Skip it.
            return 1;
        }

        Node? translation = null;
        BlockNode? nextBlock = null;
        
        switch (bytecode.OpCode)
        {
            case LingoOpCode.kOpRet:
            case LingoOpCode.kOpRetFactory:
                if (index == _bytecode.Count - 1)
                {
                    // End of function
                    return 1;
                }

                translation = new ExitStmtNode();
                break;
            case LingoOpCode.kOpPushZero:
                translation = new LiteralNode(new Datum(0));
                break;
            case LingoOpCode.kOpMul:
            case LingoOpCode.kOpAdd:
            case LingoOpCode.kOpSub:
            case LingoOpCode.kOpDiv:
            case LingoOpCode.kOpMod:
            case LingoOpCode.kOpJoinStr:
            case LingoOpCode.kOpJoinPadStr:
            case LingoOpCode.kOpLt:
            case LingoOpCode.kOpLtEq:
            case LingoOpCode.kOpNtEq:
            case LingoOpCode.kOpEq:
            case LingoOpCode.kOpGt:
            case LingoOpCode.kOpGtEq:
            case LingoOpCode.kOpAnd:
            case LingoOpCode.kOpOr:
            case LingoOpCode.kOpContainsStr:
            case LingoOpCode.kOpContains0Str:
            {
                var a = Pop();
                var b = Pop();
                translation = new BinaryOpNode(bytecode.OpCode, a, b);
                break;
            }
            case LingoOpCode.kOpInv:
            {
                var x = Pop();
                translation = new InverseOpNode(x);
                break;
            }
            case LingoOpCode.kOpNot:
            {
                var x = Pop();
                translation = new NotOpNode(x);
                break;
            }
            case LingoOpCode.kOpGetChunk:
            {
                var str = Pop();
                translation = ReadChunkRef(str);
                break;
            }
            case LingoOpCode.kOpHiliteChunk:
            {
                throw new NotImplementedException();
            }
            case LingoOpCode.kOpOntoSpr:
            {
                throw new NotImplementedException();
            }
            case LingoOpCode.kOpIntoSpr:
            {
                throw new NotImplementedException();
            }
            case LingoOpCode.kOpGetField:
            {
                Node? castId = null;
                if (_decompiler.Version >= 500)
                {
                    castId = Pop();
                }
                var fieldId = Pop();
                translation = new MemberExprNode("field", fieldId, castId);
                break;
            }
            case LingoOpCode.kOpStartTell:
            {
                throw new NotImplementedException();
            }
            case LingoOpCode.kOpEndTell:
            {
                ast.ExitBlock();
                return 1;
            }
            case LingoOpCode.kOpPushList:
            {
                var list = Pop();
                list.GetValue().Type = DatumType.kDatumList;
                translation = list;
                break;
            }
            case LingoOpCode.kOpPushPropList:
            {
                var list = Pop();
                list.GetValue().Type = DatumType.kDatumPropList;
                translation = list;
                break;
            }
            case LingoOpCode.kOpSwap:
            {
                if (_stack.Count >= 2)
                {
                    var a = Pop();
                    var b = Pop();
                    _stack.Push(a);
                    _stack.Push(b);
                }

                return 1;
            }
            case LingoOpCode.kOpPushInt8:
            case LingoOpCode.kOpPushInt16:
            case LingoOpCode.kOpPushInt32:
            {
                translation = new LiteralNode(new Datum(bytecode.Obj));
                break;
            }
            case LingoOpCode.kOpPushFloat32:
            {
                translation = new LiteralNode(new Datum((float)bytecode.Obj));
                break;
            }
            case LingoOpCode.kOpPushArgListNoRet:
            case LingoOpCode.kOpPushArgList:
            {
                var argCount = bytecode.Obj;
                var args = new List<Node>(argCount);
                
                for (var i = 0; i < argCount; i++)
                {
                    args.Add(null);
                }
                
                while (argCount != 0)
                {
                    argCount--;
                    args[argCount] = Pop();
                }
                
                var argList = new Datum(bytecode.OpCode == LingoOpCode.kOpPushArgList 
                    ? DatumType.kDatumArgList 
                    : DatumType.kDatumArgListNoRet, args);
                
                translation = new LiteralNode(argList);
                break;
            }
            case LingoOpCode.kOpPushCons:
            {
                var literalId = bytecode.Obj / VariableMultiplier();
                if (-1 < literalId && literalId < _decompiler.Literals.Count)
                {
                    translation = new LiteralNode(_decompiler.Literals[literalId]);
                }
                else
                {
                    translation = new ErrorNode();
                }
                break;
            }
            case LingoOpCode.kOpPushSymb:
            {
                var sym = new Datum(DatumType.kDatumSymbol, _decompiler.GetName((short)bytecode.Obj));
                translation = new LiteralNode(sym);
                break;
            }
            case LingoOpCode.kOpPushVarRef:
            {
                var sym = new Datum(DatumType.kDatumVarRef, _decompiler.GetName((short)bytecode.Obj));
                translation = new LiteralNode(sym);
                break;
            }
            case LingoOpCode.kOpGetGlobal:
            case LingoOpCode.kOpGetGlobal2:
            {
                translation = new VarNode(GetName((short)bytecode.Obj));
                break;
            }
            case LingoOpCode.kOpGetProp:
            {
                translation = new VarNode(GetName((short)bytecode.Obj));
                break;
            }
            case LingoOpCode.kOpGetParam:
            {
                translation = new VarNode(GetArgumentName((short)(bytecode.Obj / VariableMultiplier())));
                break;
            }
            case LingoOpCode.kOpGetLocal:
            {
                translation = new VarNode(GetLocalName((short)(bytecode.Obj / VariableMultiplier())));
                break;
            }
            case LingoOpCode.kOpSetGlobal:
            case LingoOpCode.kOpSetGlobal2:
            case LingoOpCode.kOpSetProp:
            {
                var var = new VarNode(GetName((short)bytecode.Obj));
                var value = Pop();
                translation = new AssignmentStmtNode(var, value);
                break;
            }
            case LingoOpCode.kOpSetParam:
            {
                var var = new VarNode(GetArgumentName(bytecode.Obj / VariableMultiplier()));
                var value = Pop();
                translation = new AssignmentStmtNode(var, value);
                break;
            }
            case LingoOpCode.kOpSetLocal:
            {
                var var = new VarNode(GetLocalName(bytecode.Obj / VariableMultiplier()));
                var value = Pop();
                translation = new AssignmentStmtNode(var, value);
                break;
            }
            case LingoOpCode.kOpJmp:
            {
                var targetPos = bytecode.Pos + bytecode.Obj;
                var targetIndex = _bytecodePosMap[(uint)targetPos];
                var targetBytecode = _bytecode[targetIndex];
                var ancestorLoop = ast.CurrentBlock!.AncestorLoop();
                if (ancestorLoop != null)
                {
                    if (_bytecode[targetIndex - 1].OpCode == LingoOpCode.kOpEndRepeat &&
                        _bytecode[targetIndex - 1].OwnerLoop == ancestorLoop.StartIndex)
                    {
                        translation = new ExitRepeatStmtNode();
                        break;
                    }

                    if (_bytecode[targetIndex].Tag == BytecodeTag.kTagNextRepeatTarget && 
                        _bytecode[targetIndex].OwnerLoop == ancestorLoop.StartIndex)
                    {
                        translation = new NextRepeatStmtNode();
                        break;
                    }
                }

                var nextBytecode = _bytecode[index + 1];
                var ancestorStatement = ast.CurrentBlock.AncestorStatement();
                if (ancestorStatement != null && nextBytecode.Pos == ast.CurrentBlock.EndPos)
                {
                    if (ancestorStatement.Type == NodeType.kIfStmtNode)
                    {
                        var ifStmt = (IfStmtNode)ancestorStatement;
                        if (ast.CurrentBlock == ifStmt.Block1)
                        {
                            ifStmt.HasElse = true;
                            ifStmt.Block2.EndPos = (uint)targetPos;
                            return 1; // if statement amended, nothing to push
                        }
                    } 
                    else if (ancestorStatement.Type == NodeType.kCaseStmtNode)
                    {
                        var caseStmt = (CaseStmtNode)ancestorStatement;
                        caseStmt.PotentialOtherwisePos = bytecode.Pos;
                        caseStmt.EndPos = (uint)targetPos;
                        targetBytecode.Tag = BytecodeTag.kTagEndCase;
                        return 1;
                    }
                }

                if (targetBytecode.OpCode == LingoOpCode.kOpPop && targetBytecode.Obj == 1)
                {
                    var value = Pop();
                    var caseStmt = new CaseStmtNode(value);
                    caseStmt.EndPos = (uint)targetPos;
                    targetBytecode.Tag = BytecodeTag.kTagEndCase;
                    caseStmt.AddOtherwise();
                    translation = caseStmt;
                    nextBlock = caseStmt.Otherwise!.Block;
                    break;
                }

                throw new LingoException("ERROR: Could not identify jmp");
            }
            case LingoOpCode.kOpEndRepeat:
            {
                // This should normally be tagged kTagSkip or kTagNextRepeatTarget and skipped.
                throw new LingoException("ERROR: Stray endrepeat");
            }
            case LingoOpCode.kOpJmpIfZ:
            {
                var endPos = (uint)(bytecode.Pos + bytecode.Obj);
                var endIndex = _bytecodePosMap[endPos];
                
                switch (bytecode.Tag)
                {
                    case BytecodeTag.kTagRepeatWhile:
                    {
                        var condition = Pop();
                        var loop = new RepeatWhileStmtNode(index, condition);
                        loop.Block.EndPos = endPos;
                        translation = loop;
                        nextBlock = loop.Block;
                        break;
                    }
                    case BytecodeTag.kTagRepeatWithIn:
                    {
                        var list = Pop();
                        var varName = GetVarNameFromSet(_bytecode[index + 1]);
                        var loop = new RepeatWithInStmtNode(index, varName, list);
                        loop.Block.EndPos = endPos;
                        translation = loop;
                        nextBlock = loop.Block;
                        break;
                    }
                    case BytecodeTag.kTagRepeatWithTo:
                    case BytecodeTag.kTagRepeatWithDownTo:
                    {
                        bool up = (bytecode.Tag == BytecodeTag.kTagRepeatWithTo);
                        var end = Pop();
                        var start = Pop();
                        var endRepeat = _bytecode[endIndex - 1];
                        var conditionStartIndex = _bytecodePosMap[(uint)(endRepeat.Pos - endRepeat.Obj)];
                        var varName = GetVarNameFromSet(_bytecode[conditionStartIndex - 1]);
                        var loop = new RepeatWithToStmtNode(index, varName, start, up, end);
                        loop.Block.EndPos = endPos;
                        translation = loop;
                        nextBlock = loop.Block;
                        break;
                    }
                    default:
                    {
                        var condition = Pop();
                        var ifStmt = new IfStmtNode(condition);
                        ifStmt.Block1.EndPos = endPos;
                        translation = ifStmt;
                        nextBlock = ifStmt.Block1;
                        break;
                    }
                }

                break;
            }
            case LingoOpCode.kOpLocalCall:
            {
                var argList = Pop();
                var name = _decompiler.Handlers[bytecode.Obj].Name;
                
                if (string.IsNullOrEmpty(name))
                {
                    throw new LingoException("ERROR: Could not find handler name");
                }
                
                translation = new CallNode(name, argList);
                break;
            }
            case LingoOpCode.kOpExtCall:
            case LingoOpCode.kOpTellCall:
            {
                var name = GetName((short)bytecode.Obj);
                var argList = Pop();
                var isStatement = (argList.GetValue().Type == DatumType.kDatumArgListNoRet);
                var rawArgList = argList.GetValue().L;
                var nArgs = rawArgList?.Count ?? 0;

                if (isStatement && 
                    name == "sound" && 
                    nArgs > 0 && 
                    rawArgList![0].Type == NodeType.kLiteralNode &&
                    rawArgList[0].GetValue().Type == DatumType.kDatumSymbol)
                {
                    var cmd = rawArgList[0].GetValue().S;
                    rawArgList.Clear();
                    translation = new SoundCmdStmtNode(cmd!, argList);
                } 
                else if (isStatement && name == "play" && nArgs <= 2)
                {
                    translation = new PlayCmdStmtNode(argList);
                }
                else
                {
                    translation = new CallNode(name, argList);
                }
                
                break;
            }
            case LingoOpCode.kOpObjCallV4:
            {
                var obj = ReadVar(bytecode.Obj);
                var argList = Pop();
                var rawArgList = argList.GetValue().L!;
                if (rawArgList.Count > 0)
                {
                    // first arg is a symbol
                    // replace it with a variable
                    rawArgList[0] = new VarNode(rawArgList[0].GetValue().S!);
                }

                translation = new ObjCallV4Node(obj, argList);
                break;
            }
            case LingoOpCode.kOpPut:
            {
                var putType = (PutType)((bytecode.Obj >> 4) & 0xF);
                var varType = bytecode.Obj & 0xF;
                var var = ReadVar(varType);
                var val = Pop();
                translation = new PutStmtNode(putType, var, val);
                break;
            }
            case LingoOpCode.kOpPutChunk:
            {
                var putType = (PutType)((bytecode.Obj >> 4) & 0xF);
                var varType = bytecode.Obj & 0xF;
                var var = ReadVar(varType);
                var chunk = ReadChunkRef(var);
                var val = Pop();
                if (chunk.Type == NodeType.kCommentNode)
                {
                    translation = chunk;
                }
                else
                {
                    translation = new PutStmtNode(putType, chunk, val);
                }
                break;
            }
            case LingoOpCode.kOpDeleteChunk:
            {
                var var = ReadVar(bytecode.Obj);
                var chunk = ReadChunkRef(var);
                if (chunk.Type == NodeType.kCommentNode)
                {
                    translation = chunk;
                }
                else
                {
                    translation = new ChunkDeleteStmtNode(chunk);
                }

                break;
            }
            case LingoOpCode.kOpGet:
            {
                var propertyId = Pop().GetValue().ToInt();
                translation = ReadV4Property(bytecode.Obj, propertyId);
                break;
            }
            case LingoOpCode.kOpSet:
            {
                var propertyId = Pop().GetValue().ToInt();
                var value = Pop();
                if (bytecode.Obj == 0x00 && 0x01 <= propertyId && propertyId <= 0x05 && value.GetValue().Type == DatumType.kDatumString)
                {
                    // This is either a `set eventScript to "script"` or `when event then script` statement.
                    // If the script starts with a space, it's probably a when statement.
                    // If the script contains a line break, it's definitely a when statement.
                    var script = value.GetValue().S!;
                    if (script.Length > 0 && (script[0] == ' ' || script.IndexOf('\r') != -1))
                    {
                        translation = new WhenStmtNode(propertyId, script);
                    }
                }

                if (translation == null)
                {
                    var prop = ReadV4Property(bytecode.Obj, propertyId);
                    if (prop.Type == NodeType.kCommentNode) // error comment
                    {
                        translation = prop;
                    }
                    else
                    {
                        translation = new AssignmentStmtNode(prop, value, true);
                    }
                }

                break;
            }
            case LingoOpCode.kOpGetMovieProp:
            {
                translation = new TheExprNode(GetName(bytecode.Obj));
                break;
            }
            case LingoOpCode.kOpSetMovieProp:
            {
                var value = Pop();
                var prop = new TheExprNode(GetName(bytecode.Obj));
                translation = new AssignmentStmtNode(prop, value);
                break;
            }
            case LingoOpCode.kOpGetObjProp:
            case LingoOpCode.kOpGetChainedProp:
            {
                var obj = Pop();
                translation = new ObjPropExprNode(obj, GetName(bytecode.Obj));
                break;
            }
            case LingoOpCode.kOpSetObjProp:
            {
                var value = Pop();
                var obj = Pop();
                var prop = new ObjPropExprNode(obj, GetName(bytecode.Obj));
                translation = new AssignmentStmtNode(prop, value);
                break;
            }
            case LingoOpCode.kOpPeek:
            {
                // This op denotes the beginning of a 'repeat with ... in list' statement or a case in a cases statement.

                // In a 'repeat with ... in list' statement, this peeked value is the list.
                // In a cases statement, this is the switch expression.

                var prevLabel = ast.CurrentBlock!.CurrentCaseLabel;
                
                // This must be a case. Find the comparison against the switch expression.
                var originalStackSize = _stack.Count;
                var currIndex = index + 1;
                var currBytecode = _bytecode[currIndex];

                do
                {
                    TranslateBytecode(ast, currBytecode, currIndex);
                    currIndex += 1;
                    currBytecode = _bytecode[currIndex];
                } while (currIndex < _bytecode.Count && !(_stack.Count == originalStackSize + 1 &&
                                                           (currBytecode.OpCode == LingoOpCode.kOpEq ||
                                                            currBytecode.OpCode == LingoOpCode.kOpNtEq)));

                if (currIndex >= _bytecode.Count)
                {
                    bytecode.Translation = new CommentNode("ERROR: Expected eq or nteq!");
                    ast.AddStatement(bytecode.Translation);
                    return currIndex - index + 1;
                }

                // If the comparison is <>, this is followed by another, equivalent case.
                // (e.g. this could be case1 in `case1, case2: statement`)
                var notEq = currBytecode.OpCode == LingoOpCode.kOpNtEq;
                var caseValue = Pop(); // This is the value the switch expression is compared against.

                currIndex += 1;
                currBytecode = _bytecode[currIndex];

                if (currIndex >= _bytecode.Count || currBytecode.OpCode != LingoOpCode.kOpJmpIfZ)
                {
                    bytecode.Translation = new CommentNode("ERROR: Expected jmpifz!");
                    ast.AddStatement(bytecode.Translation);
                    return currIndex - index + 1;
                }

                var jmpifz = currBytecode;
                var jmpPos = (uint)(jmpifz.Pos + jmpifz.Obj);
                var targetIndex = _bytecodePosMap[jmpPos];
                var targetBytecode = _bytecode[targetIndex];
                var prevFromTarget = _bytecode[targetIndex - 1];
                CaseExpect expect;
                if (notEq)
                {
                    expect = CaseExpect.kCaseExpectOr; // Expect an equivalent case after this one.
                } 
                else if (targetBytecode.OpCode == LingoOpCode.kOpPeek)
                {
                    expect = CaseExpect.kCaseExpectNext; // Expect a different case after this one.
                } 
                else if (targetBytecode.OpCode == LingoOpCode.kOpPop && targetBytecode.Obj == 1 && (
                               prevFromTarget.OpCode != LingoOpCode.kOpJmp ||
                               prevFromTarget.Pos + prevFromTarget.Obj == targetBytecode.Pos))
                {
                    expect = CaseExpect.kCaseExpectEnd; // Expect the end of the switch statement.
                }
                else
                {
                    expect = CaseExpect.kCaseExpectOtherwise; // Expect an 'otherwise' block.
                }

                var currLabel = new CaseLabelNode(caseValue, expect);
                jmpifz.Translation = currLabel;
                ast.CurrentBlock.CurrentCaseLabel = currLabel;
                
                if (prevLabel == null)
                {
                    var peekedValue = Pop();
                    var caseStmt = new CaseStmtNode(peekedValue);
                    caseStmt.FirstLabel = currLabel;
                    currLabel.Parent = caseStmt;
                    bytecode.Translation = caseStmt;
                    ast.AddStatement(caseStmt);
                } 
                else if (prevLabel.Expect == CaseExpect.kCaseExpectOr)
                {
                    prevLabel.NextOr = currLabel;
                    currLabel.Parent = prevLabel;
                } 
                else if (prevLabel.Expect == CaseExpect.kCaseExpectNext)
                {
                    prevLabel.NextLabel = currLabel;
                    currLabel.Parent = prevLabel;
                }
                
                // The block doesn't start until the after last equivalent case,
                // so don't create a block yet if we're expecting an equivalent case.
                if (currLabel.Expect != CaseExpect.kCaseExpectOr) {
                    currLabel.Block = new BlockNode();
                    currLabel.Block.Parent = currLabel;
                    currLabel.Block.EndPos = jmpPos;
                    ast.EnterBlock(currLabel.Block);
                }

                return currIndex - index + 1;
            }
            case LingoOpCode.kOpPop:
            {
                // Pop instructions in 'repeat with in' loops are tagged kTagSkip and skipped.
                if (bytecode.Tag == BytecodeTag.kTagEndCase)
                {
                    // We've already recognized this as the end of a case statement.
                    // Attach an 'end case' node for the summary only.
                    bytecode.Translation = new EndCaseNode();
                    return 1;
                }

                if (bytecode.Obj == 1 && _stack.Count == 1)
                {
                    // We have an unused value on the stack, so this must be the end
                    // of a case statement with no labels.
                    var value = Pop();
                    translation = new CaseStmtNode(value);
                    break;
                }
                
                // Otherwise, this pop instruction occurs before a 'return' within
                // a case statement. No translation needed.
                return 1;
            }
            case LingoOpCode.kOpTheBuiltin:
            {
                _ = Pop(); // empty arglist
                translation = new TheExprNode(GetName(bytecode.Obj));
                break;
            }
            case LingoOpCode.kOpObjCall:
            {
                var method = GetName(bytecode.Obj);
                var argList = Pop();
                var rawArgList = argList.GetValue().L;
                var nArgs = rawArgList?.Count ?? 0;
                if (method == "getAt" && nArgs == 2)
                {
                    var obj = rawArgList![0];
                    var prop = rawArgList[1];
                    translation = new ObjBracketExprNode(obj, prop);
                } 
                else if (method == "setAt" && nArgs == 3)
                {
                    var obj = rawArgList![0];
                    var prop = rawArgList[1];
                    var val = rawArgList[2];
                    var propExpr = new ObjBracketExprNode(obj, prop);
                    translation = new AssignmentStmtNode(propExpr, val);
                }
                else if ((method == "getProp" || method == "getPropRef") && (nArgs == 3 || nArgs == 4) && rawArgList![1].GetValue().Type == DatumType.kDatumSymbol)
                {
                    // obj.getProp(#prop, i) => obj.prop[i]
                    // obj.getProp(#prop, i, i2) => obj.prop[i..i2]
                    var obj = rawArgList[0];
                    var propName = rawArgList[1].GetValue().S ?? throw new LingoException("Expected valid string");
                    var i = rawArgList[2];
                    var i2 = (nArgs == 4) ? rawArgList[3] : null;
                    translation = new ObjPropIndexExprNode(obj, propName, i, i2);
                } 
                else if (method == "setProp" && (nArgs == 4 || nArgs == 5) && rawArgList![1].GetValue().Type == DatumType.kDatumSymbol)
                {
                    // obj.setProp(#prop, i, val) => obj.prop[i] = val
                    // obj.setProp(#prop, i, i2, val) => obj.prop[i..i2] = val
                    var obj = rawArgList[0];
                    var propName = rawArgList[1].GetValue().S ?? throw new LingoException("Expected valid string");
                    var i = rawArgList[2];
                    var i2 = (nArgs == 5) ? rawArgList[3] : null;
                    var propExpr = new ObjPropIndexExprNode(obj, propName, i, i2);
                    var val = rawArgList[nArgs - 1];
                    translation = new AssignmentStmtNode(propExpr, val);
                } 
                else if (method == "count" && nArgs == 2 && rawArgList![1].GetValue().Type == DatumType.kDatumSymbol)
                {
                    var obj = rawArgList![0];
                    var propName = rawArgList[1].GetValue().S ?? throw new LingoException("Expected valid string");
                    var propExpr = new ObjPropExprNode(obj, propName);
                    translation = new ObjPropExprNode(propExpr, "count");
                } 
                else if ((method == "setContents" || method == "setContentsAfter" || method == "setContentsBefore") && nArgs == 2)
                {
                    var putType = method switch
                    {
                        "setContents" => PutType.kPutInto,
                        "setContentsAfter" => PutType.kPutAfter,
                        "setContentsBefore" => PutType.kPutBefore,
                        _ => throw new LingoException("Unexpected method")
                    };
                    var var = rawArgList![0];
                    var val = rawArgList[1];
                    translation = new PutStmtNode(putType, var, val);
                }
                else if (method == "hilite" && nArgs == 1)
                {
                    // chunk.hilite() => hilite chunk
                    translation = new ChunkHiliteStmtNode(rawArgList![0]);
                }
                else if (method == "delete" && nArgs == 1)
                {
                    // chunk.delete() => delete chunk
                    translation = new ChunkDeleteStmtNode(rawArgList![0]);
                }
                else
                {
                    translation = new ObjCallNode(method, argList);
                }

                break;
            }
            case LingoOpCode.kOpPushChunkVarRef:
            {
                translation = ReadVar(bytecode.Obj);
                break;
            }
            case LingoOpCode.kOpGetTopLevelProp:
            {
                var name = GetName(bytecode.Obj);
                translation = new VarNode(name);
                break;
            }
            case LingoOpCode.kOpNewObj:
            {
                var objType = GetName(bytecode.Obj);
                var objArgs = Pop();
                translation = new NewObjNode(objType, objArgs);
                break;
            }
            default:
            {
                translation = new CommentNode($"ERROR: Unhandled opcode {bytecode.OpCode}");
                _stack.Clear();
                // throw new NotImplementedException($"Missing opcode translation for {bytecode.OpCode}");
                break;
            }
        }

        if (translation == null)
        {
            translation = new ErrorNode();
        }

        bytecode.Translation = translation;
        
        // Console.WriteLine($"{bytecode.OpCode,20}: {bytecode.Translation}");

        if (translation.IsExpression)
        {
            _stack.Push(translation);
        }
        else
        {
            ast.AddStatement(translation);
        }

        if (nextBlock != null)
        {
            ast.EnterBlock(nextBlock);
        }

        return 1;
    }

    private Node ReadVar(int varType)
    {
        Node? castId = null;
        if (varType == 0x6 && _decompiler.Version >= 500) // field cast ID
        {
            castId = Pop();
        }

        var id = Pop();

        switch (varType)
        {
            case 0x1: // global
            case 0x2: // global
            case 0x3: // property/instance
                return id;
            case 0x4: // arg
            {
                var name = GetArgumentName(id.GetValue().I!.Value / VariableMultiplier());
                var ref_ = new Datum(DatumType.kDatumVarRef, name);
                return new LiteralNode(ref_);
            }
            case 0x5: // local
            {
                var name = GetLocalName(id.GetValue().I!.Value / VariableMultiplier());
                var ref_ = new Datum(DatumType.kDatumVarRef, name);
                return new LiteralNode(ref_);
            }
            case 0x6: // field
            {
                return new MemberExprNode("field", id, castId);
            }
            default:
            {
                throw new LingoException($"Unhandled var type {varType}");
            }
        }
    }

    private Node ReadChunkRef(Node str)
    {
        var lastLine = Pop();
        var firstLine = Pop();
        var lastItem = Pop();
        var firstItem = Pop();
        var lastWord = Pop();
        var firstWord = Pop();
        var lastChar = Pop();
        var firstChar = Pop();
        
        if (!(firstLine.Type == NodeType.kLiteralNode && firstLine.GetValue().Type == DatumType.kDatumInt && firstLine.GetValue().ToInt() == 0))
            str = new ChunkExprNode(ChunkExprType.kChunkLine, firstLine, lastLine, str);
        if (!(firstItem.Type == NodeType.kLiteralNode && firstItem.GetValue().Type == DatumType.kDatumInt && firstItem.GetValue().ToInt() == 0))
            str = new ChunkExprNode(ChunkExprType.kChunkItem, firstItem, lastItem, str);
        if (!(firstWord.Type == NodeType.kLiteralNode && firstWord.GetValue().Type == DatumType.kDatumInt && firstWord.GetValue().ToInt() == 0))
            str = new ChunkExprNode(ChunkExprType.kChunkWord, firstWord, lastWord, str);
        if (!(firstChar.Type == NodeType.kLiteralNode && firstChar.GetValue().Type == DatumType.kDatumInt && firstChar.GetValue().ToInt() == 0))
            str = new ChunkExprNode(ChunkExprType.kChunkChar, firstChar, lastChar, str);

        return str;
    }
    
    private Node ReadV4Property(int propertyType, int propertyId)
    {
        switch (propertyType)
        {
            case 0x00:
            {
                if (propertyId <= 0x0b)
                {
                    
                }
                else
                {
                    _ = Pop();
                }
                break;
            }
            case 0x01: // number of chunks
            {
                _ = Pop();
                break;
            }
            case 0x02: // menu property
            {
                _ = Pop();
                break;
            }
            case 0x03: // menu item property
            {
                _ = Pop();
                _ = Pop();
                break;
            }
            case 0x04: // sound property
            {
                _ = Pop();
                break;
            }
            case 0x05: // resource property - unused?
            {
                return new CommentNode("ERROR: Resource property");
            }
            case 0x06: // sprite property
            {
                _ = Pop();
                break;
            }
            case 0x07: // animation property
            {
                break;
            }
            case 0x08: // animation 2 property
            {
                if (propertyId > 0x02 && _decompiler.Version >= 500)
                {
                    _ = Pop();
                }
                break;
            }
            case 0x09: // generic cast member
            case 0x0a: // chunk of cast member
            case 0x0b: // field
            case 0x0c: // chunk of field
            case 0x0d: // digital video
            case 0x0e: // bitmap
            case 0x0f: // sound
            case 0x10: // button
            case 0x11: // shape
            case 0x12: // movie
            case 0x13: // script
            case 0x14: // scriptText
            case 0x15: // chunk of scriptText
            {
                if (_decompiler.Version >= 500)
                {
                    _ = Pop();
                }

                _ = Pop();
                break;
            }
            default:
                throw new LingoException($"ERROR: Unknown property type {propertyType} id {propertyId}");
        }

        return new CommentNode($"ERROR: Property type {propertyType} with id {propertyId} not handled");
    }
    
    private string GetVarNameFromSet(Bytecode bytecode)
    {
        string varName;
        switch (bytecode.OpCode)
        {
            case LingoOpCode.kOpSetGlobal:
            case LingoOpCode.kOpSetGlobal2:
                varName = GetName(bytecode.Obj);
                break;
            case LingoOpCode.kOpSetProp:
                varName = GetName(bytecode.Obj);
                break;
            case LingoOpCode.kOpSetParam:
                varName = GetArgumentName(bytecode.Obj / VariableMultiplier());
                break;
            case LingoOpCode.kOpSetLocal:
                varName = GetArgumentName(bytecode.Obj / VariableMultiplier());
                break;
            default:
                varName = "ERROR";
                break;
        }
        return varName;
    }

    private string GetName(int id)
    {
        return _decompiler.GetName(id);
    }

    private string GetArgumentName(int id)
    {
        if (-1 < id && id < _function.Arguments.Count)
        {
            return GetName(_function.Arguments[id]);
        }
        
        return $"UNKNOWN_ARG_{id}";
    }
    
    private string GetLocalName(int id)
    {
        if (-1 < id && id < _function.Locals.Count)
        {
            return GetName(_function.Locals[id]);
        }
        
        return $"UNKNOWN_LOCAL_{id}";
    }
    
    internal void ReadNames()
    {
        if (!IsGenericEvent)
        {
            Name = _decompiler.GetName(_function.EnvironmentIndex);
        }

        for (var i = 0; i < _function.Arguments.Count; i++)
        {
            if (i == 0 && _decompiler.IsFactory) continue;
            _argumentNames.Add(_decompiler.GetName(_function.Arguments[i]));
        }

        foreach (var nameId in _function.Locals)
        {
            if (_decompiler.IsValidName(nameId))
            {
                _localNames.Add(_decompiler.GetName(nameId));
            }
        }

        foreach (var nameId in _function.Globals)
        {
            if (_decompiler.IsValidName(nameId))
            {
                _globalNames.Add(_decompiler.GetName(nameId));
            }
        }
    }

    internal void Disassemble()
    {
        ParseBytecode(_function.Bytecode);
        TranslateBytecodes();
    }
}