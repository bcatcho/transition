#!/bin/bash

if [ ! -d "tmp" ]; then
  mkdir tmp
fi

./ragel -o tmp/Tokenizer.cs -A Tokenizer.rl.cs
cp tmp/Tokenizer.cs ../Statescript/Compiler/
