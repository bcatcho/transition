#!/bin/bash

if [ ! -d "tmp" ]; then
  mkdir tmp
fi

./ragel -T0 -o tmp/Tokenizer.cs -A Tokenizer.rl.cs
cp tmp/Tokenizer.cs ../Transition/Compiler/
