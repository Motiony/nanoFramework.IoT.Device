using Es8388.Configuration;
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
        public const byte DefaultI2cAddress = 0x10;

        public Es8388(I2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice ?? throw new ArgumentNullException(nameof(i2cDevice));
        }

        public int Init(CodecConfiguration cfg)
        {
            int res = I2cWrite(Register.ES8388_DACCONTROL3, 0x04);  // 0x04 mute/0x00 unmute&ramp;DAC unmute and  disabled digital volume control soft ramp
            /* Chip Control and Power Management */
            res |= I2cWrite(Register.ES8388_CONTROL2, 0x50);
            res |= I2cWrite(Register.ES8388_CHIPPOWER, 0x00); //normal all and power up all

            // Disable the internal DLL to improve 8K sample rate
            res |= I2cWrite(0x35, 0xA0);
            res |= I2cWrite(0x37, 0xD0);
            res |= I2cWrite(0x39, 0xD0);

            res |= I2cWrite(Register.ES8388_MASTERMODE, (byte)cfg.Mode); //CODEC IN I2S SLAVE MODE

            /* dac */
            res |= I2cWrite(Register.ES8388_DACPOWER, 0xC0);  //disable DAC and disable Lout/Rout/1/2
            res |= I2cWrite(Register.ES8388_CONTROL1, 0x12);  //Enfr=0,Play&Record Mode,(0x17-both of mic&paly)
            res |= I2cWrite(Register.ES8388_DACCONTROL1, 0x18);//1a 0x18:16bit iis , 0x00:24
            res |= I2cWrite(Register.ES8388_DACCONTROL2, 0x02);  //DACFsMode,SINGLE SPEED; DACFsRatio,256
            res |= I2cWrite(Register.ES8388_DACCONTROL16, 0x00); // 0x00 audio on LIN1&RIN1,  0x09 LIN2&RIN2
            res |= I2cWrite(Register.ES8388_DACCONTROL17, 0x90); // only left DAC to left mixer enable 0db
            res |= I2cWrite(Register.ES8388_DACCONTROL20, 0x90); // only right DAC to right mixer enable 0db
            res |= I2cWrite(Register.ES8388_DACCONTROL21, 0x80); // set internal ADC and DAC use the same LRCK clock, ADC LRCK as internal LRCK
            res |= I2cWrite(Register.ES8388_DACCONTROL23, 0x00); // vroi=0
            res |= SetAdcDacVolume(Module.ES_MODULE_DAC, 0, 0);       // 0db
            DacOutput dacOutput;
            if (Configuration.DacOutput.AUDIO_HAL_DAC_OUTPUT_LINE2 == cfg.Output)
            {
                dacOutput = DacOutput.DAC_OUTPUT_LOUT1 | DacOutput.DAC_OUTPUT_ROUT1;
            }
            else if (Configuration.DacOutput.AUDIO_HAL_DAC_OUTPUT_LINE1 == cfg.Output)
            {
                dacOutput = DacOutput.DAC_OUTPUT_LOUT2 | DacOutput.DAC_OUTPUT_ROUT2;
            }
            else
            {
                dacOutput = DacOutput.DAC_OUTPUT_LOUT1 | DacOutput.DAC_OUTPUT_LOUT2 | DacOutput.DAC_OUTPUT_ROUT1 | DacOutput.DAC_OUTPUT_ROUT2;
            }
            res |= I2cWrite(Register.ES8388_DACPOWER, (int)dacOutput);  //0x3c Enable DAC and Enable Lout/Rout/1/2
            /* adc */
            res |= I2cWrite(Register.ES8388_ADCPOWER, 0xFF);
            res |= I2cWrite(Register.ES8388_ADCCONTROL1, 0xbb); // MIC Left and Right channel PGA gain

            AdcInput adcInput;
            if (Configuration.AdcInput.AUDIO_HAL_ADC_INPUT_LINE1 == cfg.Input)
            {
                adcInput = AdcInput.ADC_INPUT_LINPUT1_RINPUT1;
            }
            else if (Configuration.AdcInput.AUDIO_HAL_ADC_INPUT_LINE2 == cfg.Input)
            {
                adcInput = AdcInput.ADC_INPUT_LINPUT2_RINPUT2;
            }
            else
            {
                adcInput = AdcInput.ADC_INPUT_DIFFERENCE;
            }
            res |= I2cWrite(Register.ES8388_ADCCONTROL2, (int)adcInput);  //0x00 LINSEL & RINSEL, LIN1/RIN1 as ADC Input; DSSEL,use one DS Reg11; DSR, LINPUT1-RINPUT1
            res |= I2cWrite(Register.ES8388_ADCCONTROL3, 0x02);
            res |= I2cWrite(Register.ES8388_ADCCONTROL4, 0x0d); // Left/Right data, Left/Right justified mode, Bits length, I2S format
            res |= I2cWrite(Register.ES8388_ADCCONTROL5, 0x02);  //ADCFsMode,singel SPEED,RATIO=256
            //ALC for Microphone
            res |= SetAdcDacVolume(Module.ES_MODULE_ADC, 0, 0);      // 0db
            res |= I2cWrite(Register.ES8388_ADCPOWER, 0x09);    // Power on ADC, enable LIN&RIN, power off MICBIAS, and set int1lp to low power mode
            return res;
        }

        private int SetAdcDacVolume(Module mode, int volume, int dot)
        {
            int res = 0;
            if (volume < -96 || volume > 0)
            {
                if (volume < -96)
                    volume = -96;
                else
                    volume = 0;
            }
            dot = (dot >= 5 ? 1 : 0);
            volume = (-volume << 1) + dot;
            if (mode == Module.ES_MODULE_ADC || mode == Module.ES_MODULE_ADC_DAC)
            {
                res |= I2cWrite(Register.ES8388_ADCCONTROL8, volume);
                res |= I2cWrite(Register.ES8388_ADCCONTROL9, volume);  //ADC Right Volume=0db
            }
            if (mode == Module.ES_MODULE_DAC || mode == Module.ES_MODULE_ADC_DAC)
            {
                res |= I2cWrite(Register.ES8388_DACCONTROL5, volume);
                res |= I2cWrite(Register.ES8388_DACCONTROL4, volume);
            }
            return res;
        }

        private int Start(Module mode)
        {
            int res = 1;
            byte prev_data = I2cRead(Register.ES8388_DACCONTROL21);
            if (mode == Module.ES_MODULE_LINE)
            {
                res |= I2cWrite(Register.ES8388_DACCONTROL16, 0x09); // 0x00 audio on LIN1&RIN1,  0x09 LIN2&RIN2 by pass enable
                res |= I2cWrite(Register.ES8388_DACCONTROL17, 0x50); // left DAC to left mixer enable  and  LIN signal to left mixer enable 0db  : bupass enable
                res |= I2cWrite(Register.ES8388_DACCONTROL20, 0x50); // right DAC to right mixer enable  and  LIN signal to right mixer enable 0db : bupass enable
                res |= I2cWrite(Register.ES8388_DACCONTROL21, 0xC0); //enable adc
            }
            else
            {
                res |= I2cWrite(Register.ES8388_DACCONTROL21, 0x80);   //enable dac
            }
            byte data = I2cRead(Register.ES8388_DACCONTROL21);
            if (prev_data != data)
            {
                res |= I2cWrite(Register.ES8388_CHIPPOWER, 0xF0);   //start state machine
                res |= I2cWrite(Register.ES8388_CHIPPOWER, 0x00);   //start state machine
            }
            if (mode == Module.ES_MODULE_ADC || mode == Module.ES_MODULE_ADC_DAC || mode == Module.ES_MODULE_LINE)
            {
                res |= I2cWrite(Register.ES8388_ADCPOWER, 0x00);   //power up adc and line in
            }
            if (mode == Module.ES_MODULE_DAC || mode == Module.ES_MODULE_ADC_DAC || mode == Module.ES_MODULE_LINE)
            {
                res |= I2cWrite(Register.ES8388_DACPOWER, 0x3c);   //power up dac and line out
                res |= SetVoiceMute(false);
            }

            return res;
        }

        private int Stop(Module mode)
        {
            int res = 1;
            if (mode == Module.ES_MODULE_LINE)
            {
                res |= I2cWrite(Register.ES8388_DACCONTROL21, 0x80); //enable dac
                res |= I2cWrite(Register.ES8388_DACCONTROL16, 0x00); // 0x00 audio on LIN1&RIN1,  0x09 LIN2&RIN2
                res |= I2cWrite(Register.ES8388_DACCONTROL17, 0x90); // only left DAC to left mixer enable 0db
                res |= I2cWrite(Register.ES8388_DACCONTROL20, 0x90); // only right DAC to right mixer enable 0db
                return res;
            }
            if (mode == Module.ES_MODULE_DAC || mode == Module.ES_MODULE_ADC_DAC)
            {
                res |= I2cWrite(Register.ES8388_DACPOWER, 0x00);
                res |= SetVoiceMute(true);
            }
            if (mode == Module.ES_MODULE_ADC || mode == Module.ES_MODULE_ADC_DAC)
            {
                res |= I2cWrite(Register.ES8388_ADCPOWER, 0xFF);  //power down adc and line in
            }
            if (mode == Module.ES_MODULE_ADC_DAC)
            {
                res |= I2cWrite(Register.ES8388_DACCONTROL21, 0x9C);  //disable mclk
            }

            return res;
        }

        public int Deinit()
        {
            return I2cWrite(Register.ES8388_CHIPPOWER, 0xFF);  //reset and stop es8388
        }

        private int ConfigFmt(Module mode, I2sFmt fmt)
        {
            int res = 1;
            int reg;
            if (mode == Module.ES_MODULE_ADC || mode == Module.ES_MODULE_ADC_DAC)
            {
                reg = I2cRead(Register.ES8388_ADCCONTROL4);
                reg &= 0xfc;
                res |= I2cWrite(Register.ES8388_ADCCONTROL4, reg | (int)fmt);
            }
            if (mode == Module.ES_MODULE_DAC || mode == Module.ES_MODULE_ADC_DAC)
            {
                reg = I2cRead(Register.ES8388_DACCONTROL1);
                reg = reg & 0xf9;
                res |= I2cWrite(Register.ES8388_DACCONTROL1, reg | ((int)fmt << 1));
            }
            return res;
        }

        public int SetVoiceVolume(int volume)
        {
            if (volume < 0)
                volume = 0;
            else if (volume > 100)
                volume = 100;
            volume /= 3;
            int res = I2cWrite(Register.ES8388_DACCONTROL4, 0);
            res |= I2cWrite(Register.ES8388_DACCONTROL5, 0);
            // ROUT1VOL LOUT1VOL 0 -> -45dB; 33 -> – 4.5dB
            res |= I2cWrite(Register.ES8388_DACCONTROL24, volume);
            res |= I2cWrite(Register.ES8388_DACCONTROL25, volume);
            // DAC LDACVOL RDACVOL default 0 = 0DB; Default value 192 = – -96 dB
            res |= I2cWrite(Register.ES8388_DACCONTROL26, volume);
            res |= I2cWrite(Register.ES8388_DACCONTROL27, volume);
            return res;
        }

        public int GetVoiceVolume()
        {
            int volume = I2cRead(Register.ES8388_DACCONTROL24);
            volume *= 3;
            if (volume == 99)
            {
                volume = 100;
            }
            return volume;
        }

        private int SetBitsPerSample(Module mode, BitLength bits_length)
        {
            int res = 1;
            int reg;
            int bits = (int)bits_length;

            if (mode == Module.ES_MODULE_ADC || mode == Module.ES_MODULE_ADC_DAC)
            {
                reg = I2cRead(Register.ES8388_ADCCONTROL4);
                reg &= 0xe3;
                res |= I2cWrite(Register.ES8388_ADCCONTROL4, reg | (bits << 2));
            }
            if (mode == Module.ES_MODULE_DAC || mode == Module.ES_MODULE_ADC_DAC)
            {
                reg = I2cRead(Register.ES8388_DACCONTROL1);
                reg &= 0xc7;
                res |= I2cWrite(Register.ES8388_DACCONTROL1, reg | (bits << 3));
            }
            return res;
        }

        private int SetVoiceMute(bool enable)
        {
            int res = 1;
            int reg = I2cRead(Register.ES8388_DACCONTROL3);
            reg &= 0xFB;
            res |= I2cWrite(Register.ES8388_DACCONTROL3, reg | ((enable ? 1 : 0) << 2));
            return res;
        }

        public int SetMicGain(MicGain gain)
        {
            int gain_n = (int)gain / 3;
            gain_n = (gain_n << 4) + gain_n;
            return I2cWrite(Register.ES8388_ADCCONTROL1, gain_n); //MIC PGA
        }

        public int CtrlState(CodecMode mode, HalCtrl ctrl_state)
        {
            Module es_mode_t;
            switch (mode)
            {
                case CodecMode.AUDIO_HAL_CODEC_MODE_ENCODE:
                    es_mode_t = Module.ES_MODULE_ADC;
                    break;
                case CodecMode.AUDIO_HAL_CODEC_MODE_LINE_IN:
                    es_mode_t = Module.ES_MODULE_LINE;
                    break;
                case CodecMode.AUDIO_HAL_CODEC_MODE_DECODE:
                    es_mode_t = Module.ES_MODULE_DAC;
                    break;
                case CodecMode.AUDIO_HAL_CODEC_MODE_BOTH:
                    es_mode_t = Module.ES_MODULE_ADC_DAC;
                    break;
                default:
                    es_mode_t = Module.ES_MODULE_DAC;
                    break;
            }
            int res;
            if (HalCtrl.AUDIO_HAL_CTRL_STOP == ctrl_state)
            {
                res = Stop(es_mode_t);
            }
            else
            {
                res = Start(es_mode_t);
            }
            return res;
        }

        public int Configi2s(I2sFmt fmt, IfaceBits bits)
        {
            int res = 1;
            BitLength tmp;
            res |= ConfigFmt(Module.ES_MODULE_ADC_DAC, fmt);
            if (bits == IfaceBits.AUDIO_HAL_BIT_LENGTH_16BITS)
            {
                tmp = BitLength.BIT_LENGTH_16BITS;
            }
            else if (bits == IfaceBits.AUDIO_HAL_BIT_LENGTH_24BITS)
            {
                tmp = BitLength.BIT_LENGTH_24BITS;
            }
            else
            {
                tmp = BitLength.BIT_LENGTH_32BITS;
            }
            res |= SetBitsPerSample(Module.ES_MODULE_ADC_DAC, tmp);
            return res;
        }

        private int I2cWrite(Register reg, int data)
        {
            return I2cWrite((byte)reg, data);
        }

        private int I2cWrite(byte reg, int data)
        {
            SpanByte writeBuffer = new byte[2] { reg, (byte)data };
            var writeTransferResult = _i2cDevice.Write(writeBuffer);
            return writeTransferResult.Status == I2cTransferStatus.FullTransfer ? 1 : 0;
        }

        private byte I2cRead(Register reg)
        {
            _i2cDevice.WriteByte((byte)reg);
            return _i2cDevice.ReadByte();
        }
    }
}
