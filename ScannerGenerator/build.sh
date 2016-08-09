#!/bin/bash

if [ ! -d "tmp" ]; then
  mkdir tmp
fi

./ragel -T0 -o tmp/Scanner.cs -A Scanner.rl.cs
cp tmp/Scanner.cs ../Transition/Compiler/
