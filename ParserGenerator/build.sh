#!/bin/bash

if [ ! -d "tmp" ]; then
  mkdir tmp
fi

./ragel -o tmp/Parser.cs -A ParserDefinition.rl.cs
cp tmp/Parser.cs ../Statescript/Compiler/
