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

        public char GetNextChar() {
            if (AtEnd()) return '\0';
            return fileContents[current];
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
                    Program.GetErrorHandler().Error(line, "", "Unknown Character: '" + chr + "'");
                    break;
            }
        }

        private void StringLogic(int startLine) {
            while (GetNextChar() != '"' && !AtEnd()) {
                // while the next character is not quotation marks and not at the end, advance
                // check if at the end of a line though, we still want that to update
                if (GetNextChar() == '\n') {line++;}
                Advance();

            }

            // if at the end, it means the string wasnt terminated. So error
            if (AtEnd()) {
                Program.GetErrorHandler().Error(line, "", $"String not terminated. String started at line: {startLine}");
                // error, so return
                return;
            }

            // at this point the string is closing, so advance so the tokeniser doesnt check the end quote and think it is the start of a new string
            Advance();
            string stringContents = fileContents[(start+1)..(current-1)]; // get the contents of the string NOT including quotations!
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
        public Scanner scanner = new Scanner();
    }
}