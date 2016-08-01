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

  keyword = (('\@' >startKeyword)
            (('machine' @{ SetKeyword(TokenKeyword.Machine); })
             | ('state' @{ SetKeyword(TokenKeyword.State); })
             | ('on' @{ SetKeyword(TokenKeyword.On); })
             | ('enter' @{ SetKeyword(TokenKeyword.Enter); })
             | ('exit' @{ SetKeyword(TokenKeyword.Exit); })
             | ('run' @{ SetKeyword(TokenKeyword.Run); })
            )) %emitToken;
  identifier = (([a-zA-Z_] >startId) ([a-zA-Z_0-9]**)) %emitToken;
  quotedValue = (((dquote %startVal) paramDquoteValue (dquote >emitToken))
                | ((squote %startVal) paramSquoteValue (squote >emitToken)));

  comment = ('#' (any - (empty|'\n'|'\n\r'))*);
  keywordLine = keyword (space+ identifier (space* transOp space* quotedValue)?)?;
  param = identifier space* (transOp | assignOp) space* quotedValue;
  taskLine = ((transOp space* quotedValue) | (identifier (space+ param)*));

  main := (space* (comment | keywordLine | taskLine) comment? space* nl+)*;
}%%
