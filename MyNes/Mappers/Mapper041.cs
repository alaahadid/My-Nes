﻿// This file is part of My Nes
//
// A Nintendo Entertainment System / Family Computer (Nes/Famicom)
// Emulator written in C#.
// 
// Copyright © Alaa Ibrahim Hadid 2009 - 2021
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.If not, see<http://www.gnu.org/licenses/>.
// 
// Author email: mailto:alaahadidfreeware@gmail.com
//
namespace MyNes.Core
{
    [BoardInfo("Caltron 6-in-1", 41)]
    class Mapper041 : Board
    {
        private bool enableReg = false;
        private int vromReg = 0;

        internal override void HardReset()
        {
            base.HardReset();
            vromReg = 0;
            enableReg = true;
        }
        internal override void WriteSRM(ref ushort address, ref byte data)
        {
            if (address <= 0x67FF)
            {
                Switch32KPRG(address & 0x7, PRGArea.Area8000);
                enableReg = (address & 0x4) == 0x4;

                vromReg = (vromReg & 0x03) | ((address >> 1) & 0x0C);
                Switch08KCHR(vromReg);
                Switch01KNMTFromMirroring((address & 0x20) == 0x20 ? Mirroring.Horz : Mirroring.Vert);
            }
            else
                base.WriteSRM(ref address, ref data);
        }
        internal override void WritePRG(ref ushort address, ref byte data)
        {
            if (enableReg)
            {
                vromReg = (vromReg & 0x0C) | (data & 0x3);
                Switch08KCHR(vromReg);
            }
        }
        internal override void WriteStateData(ref System.IO.BinaryWriter stream)
        {
            base.WriteStateData(ref stream);
            stream.Write(enableReg);
            stream.Write(vromReg);
        }
        internal override void ReadStateData(ref System.IO.BinaryReader stream)
        {
            base.ReadStateData(ref stream);
            enableReg = stream.ReadBoolean();
            vromReg = stream.ReadInt32();
        }
    }
}
