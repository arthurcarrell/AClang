using System.Text.RegularExpressions;

namespace Language
{

    enum TokenType {
        // single char
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
        COMMA, DOT, PLUS, MINUS, SEMICOLON, SLASH, STAR, QUESTION,

        // mulitple char tokens but are still symbols
        EXCLAMATION, EXCLAMATION_EQUAL,
        EQUAL, EQUAL_EQUAL,
        GREATER, GREATER_EQUAL,
        LESS, LESS_EQUAL, 
        ARROW,

        // literals
        IDENTIFIER, STRING, NUMBER,

        // keywords
        AND, FUN, FOR, IF, NULL, OR, RETURN, ELSE, THIS, TRUE, FALSE, VAR, WHILE, LOG,

        // end of file
        EOF
    }
    class Token 
    {
        private TokenType? literal;
        private string value;
        private string? tokenType;
        private int line;

        public Token(TokenType? setLiteral, string setValue, string? setType, int setLine) {
            value = setValue;
            literal = setLiteral;
            tokenType = setType;
            line = setLine;
        }  

        public TokenType? GetLiteral() { return literal; }
        public string GetValue() { return value; }
        public string? GetTokenType() { return tokenType; }
        public int GetLine() {return line; }


    }


    class Scanner 
    {
        private string fileContents = "";
        private List<Token> fileTokens = new List<Token>();

        private int start = 0;
        private int current = 0;
        private int line = 1;

        public void ReadFile(string path) {
            fileContents = File.ReadAllText(path);
        }

        public string GetRawFileContents() {
            return fileContents;
        }


        public char Advance() {
            return fileContents[current++];
        }

        public char GetNextChar(int amount=0) {
            if (current+amount >= fileContents.Length) return '\0';
            return fileContents[current+amount];
        }
        public void ScanToken() {
            char chr = Advance();

            // switch statement for each individual char
            switch (chr) {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;

                // do operators
                /* 
                These are a little more advanced, as > may be its own token or it may be >=, so you have to check the next character to see if its an equal sign.
                If you do check, you dont want to put three equals, so you increase current
                */
                case '!': if (GetNextChar() == '=') {AddToken(TokenType.EXCLAMATION_EQUAL); current++;} else {AddToken(TokenType.EXCLAMATION);} break;
                case '=': if (GetNextChar() == '=') {AddToken(TokenType.EQUAL_EQUAL); current++;} else {AddToken(TokenType.EQUAL);} break;
                case '<': if (GetNextChar() == '=') {AddToken(TokenType.LESS_EQUAL); current++;} else {AddToken(TokenType.LESS);} break;
                case '>': if (GetNextChar() == '=') {AddToken(TokenType.GREATER_EQUAL); current++;} else {AddToken(TokenType.GREATER);} break;
                
                // divide and comment control
                case '/':
                    if (GetNextChar() == '/') {
                        // this is a comment, so disregard the rest of this line.
                        while (chr != '\n' && !AtEnd()) {
                            // this increases the pointer until the line ends or the program ends.
                            chr = Advance();
                        }
                    } else {
                        AddToken(TokenType.SLASH);
                    }
                    break;

                // whitespace stuff
                case '\n': line++; break;
                case ' ': break;
                case '\r': break;
                case '\t': break;

                // strings
                case '"': StringLogic(line); break;

                
                default:

                    // check for numbers, if so, then do number logic
                    if (IsNumber(chr)) {
                        // it is a number, so run NumberLogic();
                        NumberLogic();
                    } else if (IsAlphabet(chr.ToString())) {
                        // do Identifier logic
                        Identifier();
                    } else {
                        Program.GetErrorHandler().UnknownCharacterError(line, chr);
                    }
                    
                    break;
            }
        }
        
        private void Identifier() {
            while (IsAlphabet(GetNextChar().ToString()) || IsNumber(GetNextChar())) {
                // while alphanumeric, advance
                Advance();
            }

            Dictionary<string, TokenType> map = IdentifierMap();
            string stringContents = fileContents[start..current];
            TokenType outTokenType;
            if (map.ContainsKey(stringContents))
            {
                map.TryGetValue(stringContents, out outTokenType);
                Console.WriteLine(outTokenType);
            } else {
                outTokenType = TokenType.IDENTIFIER;
            }
            AddToken(outTokenType);

        }

        private static Dictionary<string, TokenType> IdentifierMap() {

            Dictionary<string, TokenType> map = new Dictionary<string, TokenType>
            {
                { "and", TokenType.AND },
                { "fun", TokenType.FUN },
                { "for", TokenType.FOR },
                { "if", TokenType.IF },
                //{ "class", TokenType.CLASS }
                { "null", TokenType.NULL },
                { "or", TokenType.OR },
                { "return", TokenType.RETURN },
                { "else", TokenType.ELSE },
                //{ "this", TokenType.THIS }
                { "true", TokenType.TRUE },
                { "false", TokenType.FALSE },
                { "var", TokenType.VAR },
                { "while", TokenType.WHILE },
                { "Log", TokenType.LOG },
            };
            return map;
        }
        private static bool IsAlphabet(string value) {
            // use regex string to check if value is alphabetical
            // credit: https://stackoverflow.com/questions/1181419/verifying-that-a-string-contains-only-letters-in-c-sharp
            return Regex.IsMatch(value, @"^[a-zA-Z]+$");
        }
        private static bool IsNumber(char chr) {
            return double.TryParse(chr.ToString(), out _);
        }
        
        private void NumberLogic() {

            // while there are numbers, just advance
            while (IsNumber(GetNextChar())) {
                Advance();
            }

            // check if the next chr is a decimal and the one after is a number if so, advance.
            if (GetNextChar() == '.' && IsNumber(GetNextChar(1))) {
                Advance();

                while (IsNumber(GetNextChar())) {
                    Advance();
                }
            }

            // add the number
            AddToken(TokenType.NUMBER, fileContents[start..current]);
        }

        private void StringLogic(int startLine) {
            while (GetNextChar() != '"' && !AtEnd()) {
                // while the next character is not quotation marks and not at the end, advance            
                Advance();

            }

            // if at the end, it means the string wasnt terminated. So error
            if (AtEnd()) {
                Program.GetErrorHandler().UnterminatedStringError(line, startLine);
                // error, so return
                return;
            }

            // at this point the string is closing, so advance so the tokeniser doesnt check the end quote and think it is the start of a new string
            Advance();
            string stringContents = fileContents[(start+1)..(current-1)]; // get the contents of the string NOT including quotations!
            

            // check for multi-line strings, which are bad.
            
            if (stringContents.Contains('\n')) {Program.GetErrorHandler().MultilineStringError(line); return;}


            AddToken(TokenType.STRING, stringContents);

        }
        public void ScanTokens() {
            // reset the token list
            fileTokens = new List<Token>();

            while (!AtEnd()) {
                start = current;
                ScanToken();
            }

            fileTokens.Add(new Token(TokenType.EOF, "", null, line));
        }

        public void AddToken(TokenType type, string? literal=null) {
            //Console.WriteLine($"TokenType: {type} - start: {start} - current: {current} fileContents.length: {fileContents.Length} nextChar: '{GetNextChar()}'");
            string text = fileContents[start..current];
            fileTokens.Add(new Token(type, text, literal, line));
        }


        private bool AtEnd() {
            return current >= fileContents.Length;
        }
    }
    class Lexer 
    {
        
    }
}