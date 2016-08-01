%%{
  machine TokenizerDef;

  action emitNewLine { _lineNumber++; EmitNewLine(); }
  action emitToken { EmitToken(); }
  action startTransOp { StartOperatorToken(TokenOperator.Transition); }
  action startAssignOp { StartOperatorToken(TokenOperator.Assign); }
  action startKeyword { StartToken(TokenType.Keyword); }
  action startId { StartToken(TokenType.Identifier); }
  action startVal { StartToken(TokenType.Value); }
  action startTransVal { StartToken(TokenType.TransitionValue); }
  action startMessageVal { StartToken(TokenType.MessageValue); }

  squote = "'";
  dquote = '"';
  notSquoteOrEscape = [^'\\];
  notDquoteOrEscape = [^"\\];
  escapedAny = /\\./;
  nl = ('\n' | '\r\n') >emitNewLine;
  transOp = '->' >startTransOp %emitToken;
  assignOp = ':' >startAssignOp %emitToken;

  paramSquoteValue = (notSquoteOrEscape | escapedAny)*;
  paramDquoteValue = (notDquoteOrEscape | escapedAny)*;

  keyword = (('\@' >startKeyword) ('machine'|'state'|'on'|'enter'|'exit'|'run')) %emitToken;
  identifier = (([a-zA-Z_] >startId) ([a-zA-Z_0-9]**)) %emitToken;
  name = ([a-zA-Z_] [a-zA-Z_0-9]**);
  quotedValue = (((dquote %startVal) paramDquoteValue (dquote >emitToken))
                | ((squote %startVal) paramDquoteValue (squote >emitToken)));

  keywordLine = keyword (space+ identifier (space* transOp space* quotedValue)?)?;

  comment = ('#' (^(empty|'\n'|'\r\n'))*);
  param = identifier space* (transOp | assignOp) space* quotedValue;
  taskLine = ((transOp space* quotedValue) | (identifier (space+ param)*));

  main := (space* (keywordLine | taskLine) space* comment? nl+)*;
}%%
