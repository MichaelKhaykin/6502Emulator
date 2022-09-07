using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator
{
    public enum AddressingModes
    {
        Immediate,
        Absolute,
        ZeroPage,
        Implied,
        Accumulator,
        IndirectX,
        IndirectY,
        ZeroPageX,
        ZeroPageY,
        AbsoluteX,
        AbsoluteY,
        Relative,
        Indirect,
    
        DEFAULTCASE,
        JumpLabel,
    }

    public enum InstructionType
    {    
        ADC, AND, ASL, BCC, BCS, BEQ, BIT, BMI, BNE, BPL, BRK, BVC, BVS, CLC,
        CLD, CLI, CLV, CMP, CPX, CPY, DEC, DEX, DEY, EOR, INC, INX, INY, JMP,
        JSR, LDA, LDX, LDY, LSR, NOP, ORA, PHA, PHP, PLA, PLP, ROL, ROR, RTI,
        RTS, SBC, SEC, SED, SEI, STA, STX, STY, TAX, TAY, TSX, TXA, TXS, TYA
    }

    public enum FlagType
    {
        C,
        Z,
        V,
        S,
        D,
        B,
    }
}
