namespace Shockky.Lingo.Instructions
{
    public enum OPCode : byte
    {
        Return = 0x01,
        ReturnFactory,
        PushInt0,
        Multiple,
        Add,
        Substract,
        Divide,
        Modulo,
        Inverse,
        JoinString,
        JoinPadString,
        LessThan,
        LessThanEquals,
        NotEqual,
        Equals,
        GreaterThan,
        GreaterThanEquals,
        And,
        Or,
        Not,
        ContainsString,
        StartsWith,
        ChunkExpression,
        Hilite,
        OntoSprite,
        IntoSprite,
        CastString,
        StartTell,
        EndTell,
        WrapList,
        NewPropList,
        Op_20,

        Swap,
        Op_22, //OD: chunk_expr_related
        Op_23, //OD: chunk_expr_related - pas
        Op_25 = 0x25,
        ExecuteJavascript,

        //Multi
        PushInt = 0x41,
        NewArgList, //load uint - csnover
        NewList,
        PushConstant,
        PushSymbol,
        PushObject,
        Op_47, //TODO: LoadVar - csnover //TODO: VariableKind - maps from 47.. and 4d..
        Op_48, //TODO: GetGlobal - EventScripts - ScummVM (factories)
        GetGlobal,
        GetProperty,
        GetParameter,
        GetLocal,
        Op_4d, //TODO: StoreVar - csnover eq-rust
        Op_4e, //TODO: SetGlobal - EventScrips - ScummVM (factories)
        SetGlobal,
        SetProperty,
        SetParameter,
        SetLocal,
        Jump,
        EndRepeat,
        IfFalse,
        CallLocal,
        CallExternal,
        CallObjOld,
        InsertString, //TODO: ChunkExpressions
        Insert,
        DeleteString,
        Get,
        Set,
        Op_5E,
        GetMovieProp,
        SetMovieProp,
        GetObjProp,
        SetObjProp,
        TellCall,
        Dup,
        Pop,
        GetMovieInfo, //TODO: builtin
        CallObj,
        Op_68, //OD: ChunkExpr
        Op_69, //OD
        Op_6A, //OD
        Op_6B, //OD
        Op_6C, //OD
        Op_6D,
        PushInt16,
        PushInt32,
        GetChainedProp,
        PushFloat,
        GetTopLevelProp,
        Op_73, //OD: immediate is builtin function symbol?
        Op_7d = 0x7d //TODO: does this exist
    }
}