
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.module extern ProgA.netmodule
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern JP_NET.Lab01a
{
  .ver 0:0:0:0
}
.assembly JP_NET.AL_OUTPUT
{
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.file ClassA.netmodule
    .hash = (BE EE 1F 0E 70 59 71 B1 D5 19 41 3C 3A 39 6C E1   // ....pYq...A<:9l.
             0C 55 DB 26 )                                     // .U.&
.file ClassB.netmodule
    .hash = (83 1B 71 00 01 0D D6 03 6D 66 67 F8 68 18 93 70   // ..q.....mfg.h..p
             42 11 BC 54 )                                     // B..T
.file ProgA.netmodule
    .hash = (82 4E DC 1C C7 6D ED 08 6E 41 16 17 7C BC 0E 44   // .N...m..nA..|..D
             AF 0F CF 98 ) 
.class extern public JP_NET.Lab1.ClassA
{
  .file ClassA.netmodule
  .class 0x02000002
}
.class extern public JP_NET.Lab1.ClassB
{
  .file ClassB.netmodule
  .class 0x02000002
}
.class extern public JP_NET.Lab1.Program
{
  .file ProgA.netmodule
  .class 0x02000002
}
.module JP_NET.AL_OUTPUT.exe
// MVID: {550C6C6C-3309-403A-9B87-939B2F9883CF}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x066D0000


// ================== GLOBAL METHODS =========================

.method privatescope static void  __EntryPoint$PST06000001() cil managed
{
  .entrypoint
  // Code size       8 (0x8)
  .maxstack  8
  IL_0000:  tail.
  IL_0002:  call       void [.module ProgA.netmodule]JP_NET.Lab1.Program::Main()
  IL_0007:  ret
} // end of global method __EntryPoint


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file JP_NET.AL_OUTPUT.res
