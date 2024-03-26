#!/usr/bin/env bash

cut -f2 phase0.txt | tr ',' '\n'  | sort | uniq > phase1.txt

