# pdcl - a **P**rocedure **D**otnet-**C**ompatible **L**anguage .
### cdcl is a procedure programming language that is translated into MSIL, does not support OOP.

#### A brief info

The language itself is a non-OOP lang which is translated into a single .il file. 
Then you can use any IL-disassembler to compile it into a library or executable.
 

#### APPLICATION CONTEXT:
A 'main' function defined throughout the project


#### Common Type System

As language is translated into MSIL, a suitable CTS will be used.

The language is provided with primitive types, almost all names are the same as in IL.
```c
boolean - bool

uint8 - Byte
uint16 - unsigned short
int16 - short
uint32 - unsigned int
int32 - int
uint64 - unsigned long
int64 - long

single - float
double - double

char - char
string - string
```