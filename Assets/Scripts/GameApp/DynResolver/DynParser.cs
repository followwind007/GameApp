using System;
using System.Collections.Generic;

namespace GameApp.DynResolver
{
    public class DynParser
    {
        private const string 
            TypeStrict = "strict",
            TypeDefault = "default",
            TypeStruct = "struct", 
            TypeInt = "int",
            TypeFloat = "float",
            TypeString = "string";
        
        public static readonly HashSet<string> ReserveTypes = new HashSet<string>()
        {
            TypeStrict, TypeDefault ,TypeStruct, TypeInt, TypeFloat, TypeString
        };
        
        private static readonly HashSet<char> SepChars = new HashSet<char>() { ' ', '\f', '\t', '\v', '{', '}', ',', ';' };
        
        private enum Token
        {
            None,
            Struct,
            Type,
            ArrayType
        }
        
        private struct LexState
        {
            public Token tk;
            public string str;
        }
        
        private enum Stage
        {
            StructType,
            StructName,
            StructId,
            StructContent,
            FieldType,
            FieldName,
            FieldId
        }
        
        private enum State
        {
            StructDec,
            TypeDec
        }
        
        private struct GState
        {
            public State state;
            public Stage stage;

            public void Next()
            {
                stage++;
            }
        }
        
        private struct Struct
        {
            public string type;
            public string name;
            public int id;

            public void Reset()
            {
                type = null;
                name = null;
                id = 0;
            }
        }
        
        private struct Property
        {
            public string type;
            public bool isArray;
            public string name;
            public int id;

            public void Reset()
            {
                type = null;
                isArray = false;
                name = null;
                id = 0;
            }
        }

        private string _content;

        private int _idx;
        private int _line;
        
        private Stack<GState> _states = new Stack<GState>();
        private LexState _lex;

        private Struct _struct;
        private Property _property;

        private List<DynDescriptor> _descriptors = new List<DynDescriptor>();
        private DynDescriptor _descriptor;
        private DynField _field;

        private int Length => _content.Length;
        private char Cur => _content[_idx];

        public List<DynDescriptor> Parse(string content)
        {
            _content = content;
            _descriptors = new List<DynDescriptor>();
            _idx = 0;
            _line = 0;
            while (_idx < Length)
            {
                NextLex();
                switch (_lex.tk)
                {
                    case Token.None:
                        break;
                    case Token.Struct:
                        _states.Push(new GState {state = State.StructDec, stage = Stage.StructType});
                        break;
                    case Token.Type:
                    case Token.ArrayType:
                        if (_states.Count < 1 || _states.Peek().state != State.StructDec)
                            throw GetException($"Can not declare property [{_lex.str}] outside the struct.");
                        
                        _states.Push(new GState {state = State.TypeDec, stage = Stage.FieldType});
                        break;
                }

                if (!string.IsNullOrEmpty(_lex.str))
                {
                    HandleState();
                }
            }
            
            return _descriptors;
        }

        public void Dispose()
        {
            _states.Clear();
            _descriptor = null;
            _field = null;
            _descriptors = null;
            _content = null;
        }

        private void HandleState()
        {
            if (_states.Count == 0) return;
            
            var curState = _states.Peek();
            switch (curState.state)
            {
                case State.StructDec:
                    switch (curState.stage)
                    {
                        case Stage.StructType:
                            var str = ReadName();
                            if (str != TypeStruct)
                                throw GetException($"Unrecognize type {str}.");
                            _struct.Reset();
                            _struct.type = _lex.str;
                            NextAndHandle();
                            break;
                        case Stage.StructName:
                            _struct.name = ReadName();
                            NextAndHandle();
                            break;
                        case Stage.StructId:
                            _struct.id = ReadInteger();
                            NextStage();
                            break;
                        case Stage.StructContent:
                            if (_lex.str == null || _lex.str.Length != 1)
                                throw GetException($"Can not resolve '{_lex.str}' in Struct");
                            var sep = _lex.str[0];
                            if (sep == '}')
                            {
                                if (_descriptor == null)
                                    throw GetException("Can not find any struct defined");
                                if (_field != null)
                                    _descriptor.AddField(_field);
                                
                                _descriptors.Add(_descriptor);
                                _descriptor = null;
                                _states.Pop();
                            }
                            else if (sep == '{')
                                _descriptor = new DynDescriptor(_struct.name, _struct.id, _struct.type);
                            else if (!SepChars.Contains(sep))
                                throw GetException($"Can not resolve separator '{sep}' in Struct");
                            
                            break;
                        default:
                            throw GetException($"Illegal stage [{curState.stage}] in Struct.");
                    }
                    break;
                case State.TypeDec:
                    switch (curState.stage)
                    {
                        case Stage.FieldType:
                            if (_lex.tk != Token.Type && _lex.tk != Token.ArrayType)
                            {
                                throw GetException($"Can not resolve token '{_lex.str}' [{_lex.tk}] at FieldType stage");
                            }
                            _property.Reset();
                            _property.type = _lex.str;
                            _property.isArray = _lex.tk == Token.ArrayType;
                            
                            NextAndHandle();
                            break;
                        case Stage.FieldName:
                            _property.name = ReadName();
                            NextAndHandle();
                            break;
                        case Stage.FieldId:
                            _property.id = ReadInteger();
                            _field = new DynField(_property.type, _property.name, _property.id, _property.isArray);
                            _descriptor.AddField(_field);
                            _states.Pop();
                            break;
                        default:
                            throw GetException($"Illegal stage [{curState.stage}] in Type.");
                    }
                    break;
            }
        }

        private void NextAndHandle()
        {
            NextStage();
            HandleState();
        }

        private void NextStage()
        {
            var curState = _states.Pop();
            curState.Next();
            _states.Push(curState);
        }
        

        private Exception GetException(string msg)
        {
            return new Exception($"{msg} line{_line}");
        }
        
        private void NextLex()
        {
            _lex.tk = Token.None;
            _lex.str = null;
            
            var cur = Cur;

            switch (cur)
            {
                case '\r': case '\n':
                    _line++;
                    Next();
                    break;
                case ' ': case '\f': case '\t': case '\v':
                    Next();
                    break;
                case '/':
                    if (_idx + 1 < Length && _content[_idx + 1] == '/')
                    {
                        ToLineEnd();
                        Next();
                    }
                    else
                    {
                        _lex.str += cur;
                        Next();
                    }
                    break;
                default:
                    if (char.IsLetter(cur))
                    {
                        var str = ReadUntilSep();
                        if (str == TypeStrict || str == TypeDefault)
                        {
                            _lex.tk = Token.Struct;
                            _lex.str = str;
                        }
                        else
                        {
                            if (str.EndsWith("[]"))
                            {
                                var trimed = str.Substring(0, str.Length - 2);
                                _lex.tk = Token.ArrayType;
                                _lex.str = trimed;
                            }
                            else
                            {
                                _lex.tk = Token.Type;
                                _lex.str = str;
                            }
                        }
                    }
                    else
                    {
                        Next();
                        _lex.str += cur;
                    }
                    
                    break;
            }
        }

        private void ToLineEnd()
        {
            for (var i = _idx; i < Length; i++)
            {
                if (_content[i] == '\n') break;
                _idx++;
            }
        }
        
        private string ReadUntilSep()
        {
            var start = _idx;
            
            while (_idx < Length)
            {
                bool sep;
                var cur = Cur;
                switch (cur)
                {
                    case '\n': case '\r':
                        sep = true;
                        break;
                    default:
                        sep = SepChars.Contains(cur);
                        break;
                }

                if (sep) break;
                
                Next();
            }

            return _content.Substring(start, _idx - start);
        }
        
        private string ReadName()
        {
            while (_idx < Length)
            {
                var cur = Cur;
                if (cur == ' ')
                    Next();
                else if(char.IsLetter(cur))
                    break;
                else
                    throw GetException($"Read name error at {cur}");
            }
            
            var start = _idx;
            while (_idx < Length)
            {
                var cur = Cur;
                if (char.IsLetter(cur) || char.IsNumber(cur) || cur == '_')
                    Next();
                else
                    break;
            }
            return _content.Substring(start, _idx - start);
        }

        private int ReadInteger()
        {
            while (_idx < Length)
            {
                var cur = Cur;
                if (cur == ' ')
                    Next();
                else if (char.IsNumber(cur))
                    break;
                else
                    throw GetException($"Read number error at {cur}.");
                
            }

            var start = _idx;
            while (_idx < Length)
            {
                if (char.IsNumber(Cur))
                    Next();
                else
                    break;
            }

            var str = _content.Substring(start, _idx - start);
            
            if (!int.TryParse(str, out var res))
                throw GetException($"Try parse '{str}' to integer error.");
            
            return res;
        }

        private void Next()
        {
            _idx++;
        }
    }
}