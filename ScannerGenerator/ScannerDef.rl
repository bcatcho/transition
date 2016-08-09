%%{
  machine ScannerDef;

  action emitNewLine { _lineNumber++; EmitNewLine(); }
  action emitToken { EmitToken(); }
  action startTransOp { StartOperatorToken(TokenOperator.Transition); }
  action startAssignOp { StartOperatorToken(TokenOperator.Assign); }
  action startKeyword { StartToken(TokenType.Keyword); }
  action startId { StartToken(TokenType.Identifier); }
  action startVal { StartToken(TokenType.Value); }
  action startTransVal { StartToken(TokenType.TransitionValue); }
  action startMessageVal { StartToken(TokenType.MessageValue); }

  nl = ('\n' | '\r\n') >emitNewLine;
  transOp = '->' >startTransOp %emitToken;
  assignOp = ':' >startAssignOp %emitToken;

  squote = "'";
  dquote = '"';
  notSquoteOrEscape = [^'\\];
  notDquoteOrEscape = [^"\\];
  escapedAny = /\\./;
  squoteValue = (notSquoteOrEscape | escapedAny)*;
  dquoteValue = (notDquoteOrEscape | escapedAny)*;
  quotedValue = (((dquote %startVal) dquoteValue (dquote >emitToken))
                | ((squote %startVal) squoteValue (squote >emitToken)));

  keyword = (('\@' >startKeyword)
            (('machine' @{ SetKeyword(TokenKeyword.Machine); })
             | ('state' @{ SetKeyword(TokenKeyword.State); })
             | ('on' @{ SetKeyword(TokenKeyword.On); })
             | ('enter' @{ SetKeyword(TokenKeyword.Enter); })
             | ('exit' @{ SetKeyword(TokenKeyword.Exit); })
             | ('run' @{ SetKeyword(TokenKeyword.Run); })
            )) %emitToken;
  identifier = (([a-zA-Z_] >startId) ([a-zA-Z_0-9]**)) %emitToken;
  keywordLine = keyword (space+ identifier (space* transOp space* quotedValue)?)?;

  param = identifier space* (transOp | assignOp) space* quotedValue;
  taskLine = ((transOp space* quotedValue) | (identifier (space+ param)*));
  comment = ('#' (any - (empty|'\n'|'\n\r'))*);

  main := (space* (comment | keywordLine | taskLine) comment? space* nl+)*;
}%%
