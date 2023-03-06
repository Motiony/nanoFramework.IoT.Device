using System;
using System.Device.I2c;

namespace Es8388
{
    /// <summary>
    /// ES8388 - audio codec
    /// </summary>
    public class Es8388
    {
        private readonly I2cDevice _i2cDevice;
        private OutSelect _outSelect;
        private InSelect _inSelect;
        public const byte DefaultI2cAddress = 0x10;

        public Es8388(I2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice ?? throw new ArgumentNullException(nameof(i2cDevice));
        }

        public bool Init()
        {
            bool res = true;
            /* INITIALIZATION (BASED ON ES8388 USER GUIDE EXAMPLE) */
            // Set Chip to Slave
            res &= I2cWrite(Register.ES8388_MASTERMODE, 0x00);
            // Power down DEM and STM
            res &= I2cWrite(Register.ES8388_CHIPPOWER, 0xFF);
            // Set same LRCK	Set same LRCK
            res &= I2cWrite(Register.ES8388_DACCONTROL21, 0x80);
            // Set Chip to Play&Record Mode
            res &= I2cWrite(Register.ES8388_CONTROL1, 0x05);
            // Power Up Analog and Ibias
            res &= I2cWrite(Register.ES8388_CONTROL2, 0x40);

            /* ADC setting */
            // Micbias for Record
            res &= I2cWrite(Register.ES8388_ADCPOWER, 0x00);
            // Enable Lin1/Rin1 (0x00 0x00) for Lin2/Rin2 (0x50 0x80)
            res &= I2cWrite(Register.ES8388_ADCCONTROL2, 0x50);
            res &= I2cWrite(Register.ES8388_ADCCONTROL3, 0x80);
            // PGA gain (0x88 - 24db) (0x77 - 21db)
            res &= I2cWrite(Register.ES8388_ADCCONTROL1, 0x77);
            // SFI setting (i2s mode/16 bit)
            res &= I2cWrite(Register.ES8388_ADCCONTROL4, 0x0C);
            // ADC MCLK/LCRK ratio (256)
            res &= I2cWrite(Register.ES8388_ADCCONTROL5, 0x02);
            // set ADC digital volume
            res &= I2cWrite(Register.ES8388_ADCCONTROL8, 0x00);
            res &= I2cWrite(Register.ES8388_ADCCONTROL9, 0x00);
            // recommended ALC setting for VOICE refer to ES8388 MANUAL
            res &= I2cWrite(Register.ES8388_ADCCONTROL10, 0xEA);
            res &= I2cWrite(Register.ES8388_ADCCONTROL11, 0xC0);
            res &= I2cWrite(Register.ES8388_ADCCONTROL12, 0x12);
            res &= I2cWrite(Register.ES8388_ADCCONTROL13, 0x06);
            res &= I2cWrite(Register.ES8388_ADCCONTROL14, 0xC3);

            /* DAC setting */
            // Power Up DAC& enable Lout/Rout
            res &= I2cWrite(Register.ES8388_DACPOWER, 0x3C);
            // SFI setting (i2s mode/16 bit)
            res &= I2cWrite(Register.ES8388_DACCONTROL1, 0x18);
            // DAC MCLK/LCRK ratio (256)
            res &= I2cWrite(Register.ES8388_DACCONTROL2, 0x02);
            // unmute codec
            res &= I2cWrite(Register.ES8388_DACCONTROL3, 0x00);
            // set DAC digital volume
            res &= I2cWrite(Register.ES8388_DACCONTROL4, 0x00);
            res &= I2cWrite(Register.ES8388_DACCONTROL5, 0x00);
            // Setup Mixer
            // (reg[16] 1B mic Amp, 0x09 direct;[reg 17-20] 0x90 DAC, 0x50 Mic Amp)
            res &= I2cWrite(Register.ES8388_DACCONTROL16, 0x09);
            res &= I2cWrite(Register.ES8388_DACCONTROL17, 0x50);
            res &= I2cWrite(Register.ES8388_DACCONTROL18, 0x38);  //??
            res &= I2cWrite(Register.ES8388_DACCONTROL19, 0x38);  //??
            res &= I2cWrite(Register.ES8388_DACCONTROL20, 0x50);
            // set Lout/Rout Volume -45db
            res &= I2cWrite(Register.ES8388_DACCONTROL24, 0x00);
            res &= I2cWrite(Register.ES8388_DACCONTROL25, 0x00);
            res &= I2cWrite(Register.ES8388_DACCONTROL26, 0x00);
            res &= I2cWrite(Register.ES8388_DACCONTROL27, 0x00);

            /* Power up DEM and STM */
            res &= I2cWrite(Register.ES8388_CHIPPOWER, 0x00);
            /* set up MCLK) */
            return res;
        }

        // Select output sink
        // OUT1 -> Select Line OUTL/R1
        // OUT2 -> Select Line OUTL/R2
        // OUTALL -> Enable ALL
        public bool OutputSelect(OutSelect outSelect)
        {
            bool res = true;
            if (outSelect == OutSelect.OUTALL)
                res &= I2cWrite(Register.ES8388_DACPOWER, 0x3C);
            else if (outSelect == OutSelect.OUT1)
                res &= I2cWrite(Register.ES8388_DACPOWER, 0x30);
            else if (outSelect == OutSelect.OUT2)
                res &= I2cWrite(Register.ES8388_DACPOWER, 0x0C);
            _outSelect = outSelect;
            return res;
        }

        // Select input source
        // IN1     -> Select Line IN L/R 1
        // IN2     -> Select Line IN L/R 2
        // IN1DIFF -> differential IN L/R 1
        // IN2DIFF -> differential IN L/R 2
        public bool InputSelect(InSelect inSelect)
        {
            bool res = true;
            if (inSelect == InSelect.IN1)
                res &= I2cWrite(Register.ES8388_ADCCONTROL2, 0x00);
            else if (inSelect == InSelect.IN2)
                res &= I2cWrite(Register.ES8388_ADCCONTROL2, 0x50);
            else if (inSelect == InSelect.IN1DIFF)
            {
                res &= I2cWrite(Register.ES8388_ADCCONTROL2, 0xF0);
                res &= I2cWrite(Register.ES8388_ADCCONTROL3, 0x00);
            }
            else if (inSelect == InSelect.IN2DIFF)
            {
                res &= I2cWrite(Register.ES8388_ADCCONTROL2, 0xF0);
                res &= I2cWrite(Register.ES8388_ADCCONTROL3, 0x80);
            }
            _inSelect = inSelect;
            return res;
        }

        // mute Output
        public bool DACmute(bool mute)
        {
            byte _reg = I2cRead(Register.ES8388_ADCCONTROL1);
            bool res = true;
            if (mute)
                res &= I2cWrite(Register.ES8388_DACCONTROL3, (byte)(_reg | 0x04));
            else
                res &= I2cWrite(Register.ES8388_DACCONTROL3, (byte)(_reg & ~(0x04)));
            return res;
        }

        // set output volume max is 33
        public bool SetOutputVolume(byte vol)
        {
            if (vol > 33) vol = 33;
            bool res = true;
            if (_outSelect == OutSelect.OUTALL || _outSelect == OutSelect.OUT1)
            {
                res &= I2cWrite(Register.ES8388_DACCONTROL24, vol);  // LOUT1VOL
                res &= I2cWrite(Register.ES8388_DACCONTROL25, vol);  // ROUT1VOL
            }
            if (_outSelect == OutSelect.OUTALL || _outSelect == OutSelect.OUT2)
            {
                res &= I2cWrite(Register.ES8388_DACCONTROL26, vol);  // LOUT2VOL
                res &= I2cWrite(Register.ES8388_DACCONTROL27, vol);  // ROUT2VOL
            }
            return res;
        }

        public byte GetOutputVolume() => I2cRead(Register.ES8388_DACCONTROL24);

        // set input gain max is 8 +24db
        public bool SetInputGain(int gain)
        {
            if (gain > 8) gain = 8;
            bool res = true;
            gain = (gain << 4) | gain;
            res &= I2cWrite(Register.ES8388_ADCCONTROL1, (byte)gain);
            return res;
        }

        public byte GetInputGain()
        {
            var reg = I2cRead(Register.ES8388_ADCCONTROL1);
            return (byte)(reg & 0x0F);
        }

        // Recommended ALC setting from User Guide
        // DISABLE -> Disable ALC
        // GENERIC -> Generic Mode
        // VOICE   -> Voice Mode
        // MUSIC   -> Music Mode
        public bool SetALCmode(AlcModeSelect alc)
        {
            bool res = true;

            // generic ALC setting
            byte ALCSEL = 0b11;       // stereo
            byte ALCLVL = 0b0011;     //-12db
            byte MAXGAIN = 0b111;     //+35.5db
            byte MINGAIN = 0b000;     //-12db
            byte ALCHLD = 0b0000;     // 0ms
            byte ALCDCY = 0b0101;     // 13.1ms/step
            byte ALCATK = 0b0111;     // 13.3ms/step
            byte ALCMODE = 0b0;       // ALC
            byte ALCZC = 0b0;         // ZC off
            byte TIME_OUT = 0b0;      // disable
            byte NGAT = 0b1;          // enable
            byte NGTH = 0b10001;      //-51db
            byte NGG = 0b00;          // hold gain
            byte WIN_SIZE = 0b00110;  // default

            if (alc == AlcModeSelect.DISABLE)
                ALCSEL = 0b00;
            else if (alc == AlcModeSelect.MUSIC)
            {
                ALCDCY = 0b1010;  // 420ms/step
                ALCATK = 0b0110;  // 6.66ms/step
                NGTH = 0b01011;   // -60db
            }
            else if (alc == AlcModeSelect.VOICE)
            {
                ALCLVL = 0b1100;  // -4.5db
                MAXGAIN = 0b101;  // +23.5db
                MINGAIN = 0b010;  // 0db
                ALCDCY = 0b0001;  // 820us/step
                ALCATK = 0b0010;  // 416us/step
                NGTH = 0b11000;   // -40.5db
                NGG = 0b01;       // mute ADC
                res &= I2cWrite(Register.ES8388_ADCCONTROL1, 0x77);
            }
            res &= I2cWrite(Register.ES8388_ADCCONTROL10, (byte)(ALCSEL << 6 | MAXGAIN << 3 | MINGAIN));
            res &= I2cWrite(Register.ES8388_ADCCONTROL11, (byte)(ALCLVL << 4 | ALCHLD));
            res &= I2cWrite(Register.ES8388_ADCCONTROL12, (byte)(ALCDCY << 4 | ALCATK));
            res &= I2cWrite(Register.ES8388_ADCCONTROL13, (byte)(ALCMODE << 7 | ALCZC << 6 | TIME_OUT << 5 | WIN_SIZE));
            res &= I2cWrite(Register.ES8388_ADCCONTROL14, (byte)(NGTH << 3 | NGG << 2 | NGAT));

            return res;
        }

        // MIXIN1 – direct IN1 (default)
        // MIXIN2 – direct IN2
        // MIXRES – reserved es8388
        // MIXADC – ADC/ALC input (after mic amplifier)
        public bool MixerSourceSelect(MixSelect LMIXSEL, MixSelect RMIXSEL)
        {
            bool res = true;
            byte reg = (byte)((byte)LMIXSEL << 3 | (byte)RMIXSEL);
            res &= I2cWrite(Register.ES8388_DACCONTROL16, reg);
            return res;
        }

        // Mixer source control
        // DACOUT -> Select Sink From DAC
        // SRCSEL -> Select Sink From SourceSelect()
        // MIXALL -> Sink DACOUT + SRCSEL
        public bool MixerSourceControl(MixerControl mix)
        {
            if (mix == MixerControl.DACOUT)
                return MixerSourceControl(true, false, 2, true, false, 2);
            if (mix == MixerControl.SRCSELOUT)
                return MixerSourceControl(false, true, 2, false, true, 2);
            if (mix == MixerControl.MIXALL)
                return MixerSourceControl(true, true, 2, true, true, 2);
            return false;
        }

        // LD/RD = DAC(i2s), false disable, true enable
        // LI2LO/RI2RO from mixerSourceSelect(), false disable, true enable
        // LOVOL = gain, 0 -> 6db, 1 -> 3db, 2 -> 0db, higher will attenuate
        public bool MixerSourceControl(bool LD2LO, bool LI2LO, byte LI2LOVOL, bool RD2RO, bool RI2RO, byte RI2LOVOL)
        {
            bool res = true;
            int _regL, _regR;
            if (LI2LOVOL > 7) LI2LOVOL = 7;
            if (RI2LOVOL > 7) RI2LOVOL = 7;
            _regL = (Convert.ToByte(LD2LO) << 7) | (Convert.ToByte(LI2LO) << 6) | (LI2LOVOL << 3);
            _regR = (Convert.ToByte(RD2RO) << 7) | (Convert.ToByte(RI2RO) << 6) | (RI2LOVOL << 3);
            res &= I2cWrite(Register.ES8388_DACCONTROL17, (byte)_regL);
            res &= I2cWrite(Register.ES8388_DACCONTROL20, (byte)_regR);
            return res;
        }

        // true -> analog out = analog in
        // false -> analog out = DAC(i2s)
        public bool AnalogBypass(bool bypass)
        {
            bool res = true;
            if (bypass)
            {
                if (_inSelect == InSelect.IN1)
                    MixerSourceSelect(MixSelect.MIXIN1, MixSelect.MIXIN1);
                else if (_inSelect == InSelect.IN2)
                    MixerSourceSelect(MixSelect.MIXIN2, MixSelect.MIXIN2);
                MixerSourceControl(false, true, 2, false, true, 2);
            }
            else
            {
                MixerSourceControl(true, false, 2, true, false, 2);
            }
            return res;
        }

        private bool I2cWrite(Register reg, byte data)
        {
            SpanByte writeBuffer = new byte[2] { (byte)reg, data };
            var writeTransferResult = _i2cDevice.Write(writeBuffer);
            return writeTransferResult.Status == I2cTransferStatus.FullTransfer;
        }

        private byte I2cRead(Register reg)
        {
            _i2cDevice.WriteByte((byte)reg);
            return _i2cDevice.ReadByte();
        }
    }
}
