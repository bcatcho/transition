%%{
  machine TokenizerDef;

  action emitNewLine { _lineNumber++; log("newline"); EmitNewLine(); }
  action startKeyword { log("startKeyword"); StartToken(); }
  action endKeyword { logEnd("endKeyword"); EmitToken(TokenType.Keyword); }
  action startId { log("startId"); StartToken(); }
  action endId { logEnd("endId"); EmitToken(TokenType.Identifier); }
  action emitTransitionOp { log("emit tx op"); EmitOperator(TokenOperator.Transition); }
  action startTransitionValue { log("startTransitionValue"); StartToken(); }
  action endTransitionValue { logEnd("endTransitionValue"); EmitToken(TokenType.TransitionValue); }

  squote = "'";
  dquote = '"';
  notSquoteOrEscape = [^'\\];
  notDquoteOrEscape = [^"\\];
  escapedAny = /\\./;
  ws = [\t ];
  wss = ws**;
  nl = ('\n' | '\r\n') @emitNewLine;
  transitionOperator = '->' %emitTransitionOp;
  assignmentOperator = ':';
  comment = ('#' !nl*);

  #paramOperator = (transitionOperator | assignmentOperator);
  #paramSquoteValue = (notSquoteOrEscape | escapedAny)*; #>startToken %endParam;
  #paramDquoteValue = (notDquoteOrEscape | escapedAny)*;# >startToken %endParam;
  #paramValue = ((dquote paramDquoteValue dquote)|(squote paramSquoteValue squote));
  #param = (paramName? paramOperator paramValue);
  #defaultParam = ws+ (paramValue) >startDefaultParam;

  keyword = (('@' >startKeyword) [a-zA-Z_]**) %endKeyword;
  identifier = (([a-zA-Z_] >startId) ([a-zA-Z_0-9]**)) %endId;
  name = ([a-zA-Z_] [a-zA-Z_0-9]**) ;
  transitionValue = ((dquote name dquote)|(squote name squote)) >startTransitionValue %endTransitionValue;

  emptyLine = nl;
  commentLine = comment nl;

  machineLine = space* keyword* space+ identifier space* transitionOperator space* transitionValue;

  main := machineLine nl?;
}%%
