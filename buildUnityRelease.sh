#!/bin/bash
set +e

if [ -z "$1" ]; then
  echo "Error. Please include a version number."
  echo "Usage:"
  echo "  ./buildUnityRelease.sh v0.1.0"
  exit 1
else
  echo "==> Building unity release for version $1"
fi

if [ ! -d "tmp" ]; then
  mkdir tmp
fi
cd tmp

dirname="transition_unity_$1"
if [ -d "$dirname" ]; then
  rm -rf $dirname
fi
mkdir $dirname

cp ../Transition/*.cs $dirname
cp -R ../Transition/Compiler $dirname
cp -R ../Transition/Actions $dirname
zip -r "transition_unity_$1.zip" $dirname

echo "==> Output: ${pwd}/transition_unity_$1.zip"
rm -rf $dirname
