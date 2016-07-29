%%{
  machine ParserMachine;

  action saveIndent {
    if (!_isInCommentLine) {
      _prevIndent = _indent;
    }

    _indent = 0;
  }

  action incrementLineNumber {
    _lineNumber++;
  }

  action indent {
    _indent++;
  }

  action startToken {
    StartToken(p);
  }

  action endToken {
    _currentNode.Id = EndToken(p);
  }

  action startDefaultParam {
    _currentParam = new AstParam {
      Name = "@default"
    };
  }

  action endParamName {
    _currentParam = new AstParam {
      Name = EndToken(p)
    };
  }

  action endParam {
    _currentParam.Val = EndToken(p);
    _currentNode.Params.Add(_currentParam);
  }

  action startCommentLine {
    _isInCommentLine = true;
  }

  action endCommentLine {
    _isInCommentLine = false;
  }

  action startTask {
    if (_indent == 0) {
      _currentParent = null; // start a new root node
    } else if (_indent > _prevIndent) {
      _indentationStack.Push(_prevIndent);
      _currentParent = _currentNode; // save the parent
    } else if (_indent < _prevIndent) {
      int stackIndent;
      while (_indentationStack.Count > 0) {
        _currentParent = _currentParent.Parent;
        stackIndent = _indentationStack.Pop();;
        if (_indent == stackIndent) {
          break;
        } else if (_indent > stackIndent) {
          throw new System.Exception("Parser Error: Bad Indentation on line " + _lineNumber + " cannot determine parent");
        }
      }
    }

    _currentNode = new AstNode {
      LineNumber = _lineNumber,
    };

    // not in guard, assign parent
    if (_currentParent != null) {
      _currentNode.Parent = _currentParent;
      _currentParent.Children.Add(_currentNode);
    }
  }

  action endTask {
    if (_currentRoot == null || _indent == 0) {
      _currentRoot = _currentNode;
      _trees.Add(_currentRoot);
    }
  }

  squote = "'";
  dquote = '"';
  notSquoteOrEscape = [^'\\];
  notDquoteOrEscape = [^"\\];
  escapedAny = /\\./;
  whitespace = [\t ];

  indent = whitespace @indent;
  nl = ('\n' | '\r\n') %saveIndent %incrementLineNumber;

  comment = ('#' [^\r\n]*);

  paramName = ([a-zA-Z_]+[a-zA-Z_0-9]*) >startToken %endParamName;
  paramSquoteValue = (notSquoteOrEscape | escapedAny)* >startToken %endParam;
  paramDquoteValue = (notDquoteOrEscape | escapedAny)* >startToken %endParam;
  paramValue = ((dquote paramDquoteValue dquote)|(squote paramSquoteValue squote));
  param = whitespace+ (paramName ':' paramValue);
  defaultParam = whitespace+ (paramValue) >startDefaultParam;

  emptyLine = indent* nl;
  commentLine = indent* comment nl > startCommentLine %endCommentLine;

  id = [a-zA-Z_0-9~\-\?\>\@\$\*]+ >startToken %endToken;
  task = (id (defaultParam | param*) whitespace*) >startTask %endTask;
  line = indent* whitespace* task comment? nl;
  lastLine = indent* whitespace* task comment?;
  singleLineFile = indent* whitespace* task;

  main := (singleLineFile|(emptyLine|line|lastLine|commentLine)**);
}%%
