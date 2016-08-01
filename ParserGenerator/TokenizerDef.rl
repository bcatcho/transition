%%{
  machine TokenizerDef;

  action emitNewLine { _lineNumber++; log("newline"); EmitNewLine(); }
  action emitToken { EmitToken(); }
  action startTransOp { log("emit tx op"); StartOperatorToken(TokenOperator.Transition); }
  action startKeyword { log("startKeyword"); StartToken(TokenType.Keyword); }
  action startId { log("startId"); StartToken(TokenType.Identifier); }
  action startTransVal { log("startTransVal"); StartToken(TokenType.TransitionValue); }

  squote = "'";
  dquote = '"';
  notSquoteOrEscape = [^'\\];
  notDquoteOrEscape = [^"\\];
  escapedAny = /\\./;
  ws = [\t ];
  wss = ws**;
  nl = ('\n' | '\r\n') >emitNewLine;
  transitionOperator = '->' >startTransOp @emitToken;
  assignmentOperator = ':';
  comment = ('#' !nl*);

  #paramOperator = (transitionOperator | assignmentOperator);
  #paramSquoteValue = (notSquoteOrEscape | escapedAny)*; #>startToken %endParam;
  #paramDquoteValue = (notDquoteOrEscape | escapedAny)*;# >startToken %endParam;
  #paramValue = ((dquote paramDquoteValue dquote)|(squote paramSquoteValue squote));
  #param = (paramName? paramOperator paramValue);
  #defaultParam = ws+ (paramValue) >startDefaultParam;

  keyword = (('@' >startKeyword) [a-zA-Z_]**) %emitToken;
  identifier = (([a-zA-Z_] >startId) ([a-zA-Z_0-9]**)) %emitToken;
  name = ([a-zA-Z_] [a-zA-Z_0-9]**) ;
  transitionValue = (((dquote %startTransVal) name (dquote @emitToken))
                    | ((squote %startTransVal) name (squote @emitToken)));

  emptyLine = nl;
  commentLine = comment nl;
  machineLine = space* keyword* space+ identifier space* transitionOperator space* transitionValue;
  keywordLine = space* keyword*;
  keywordWithIdLine = space* keyword* space+ identifier;

  main := ((machineLine | keywordLine | keywordWithIdLine) nl)*;
}%%
