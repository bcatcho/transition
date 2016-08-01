%%{
  machine TokenizerDef;

  action emitNewLine { _lineNumber++; log("newline"); EmitNewLine(); }
  action startKeyword { log("startKeyword"); StartToken(TokenType.Keyword); }
  action endKeyword { logEnd("endKeyword"); EmitToken(); }
  action startId { log("startId"); StartToken(TokenType.Identifier); }
  action endId { logEnd("endId"); EmitToken(); }
  action emitTransitionOp { log("emit tx op"); EmitOperator(TokenOperator.Transition); }
  action startTransVal { log("startTransVal"); StartToken(TokenType.TransitionValue); }
  action endTransVal { logEnd("endTransVal"); EmitToken(); }
  action commitLastToken { CommitLastToken(); }

  squote = "'";
  dquote = '"';
  notSquoteOrEscape = [^'\\];
  notDquoteOrEscape = [^"\\];
  escapedAny = /\\./;
  ws = [\t ];
  wss = ws**;
  nl = ('\n' | '\r\n') >emitNewLine;
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
  transitionValue = (((dquote %startTransVal) name (dquote @endTransVal))
                    | ((squote %startTransVal) name (squote @endTransVal)));

  emptyLine = nl;
  commentLine = comment nl;
  machineLine = space* keyword* space+ identifier space* transitionOperator space* transitionValue;

  keywordWithIdLine = space* keyword* space+ identifier;

  main := ((machineLine nl+)
          | (keywordWithIdLine nl+));
}%%
