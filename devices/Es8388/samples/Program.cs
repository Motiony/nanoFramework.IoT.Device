using Es8388.Configuration;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.I2c;
using System.Device.I2s;

namespace Es8388.Samples
{
    public class Program
    {
        public static void Main()
        {
            nanoFramework.Hardware.Esp32.Configuration.SetPinFunction(18, DeviceFunction.I2C1_DATA);
            nanoFramework.Hardware.Esp32.Configuration.SetPinFunction(23, DeviceFunction.I2C1_CLOCK);
            I2cDevice i2cDevice = new(new I2cConnectionSettings(1, Es8388.DefaultI2cAddress, I2cBusSpeed.StandardMode));

            var codecConfiguration = new CodecConfiguration
            {
                Input = AdcInput.AUDIO_HAL_ADC_INPUT_LINE2,
                Mode = Mode.AUDIO_HAL_MODE_SLAVE,
                Output = DacOutput.AUDIO_HAL_DAC_OUTPUT_ALL
            };
            var codec = new Es8388(i2cDevice);
            codec.Init(codecConfiguration);
            codec.Configi2s(I2sFmt.ES_I2S_NORMAL, IfaceBits.AUDIO_HAL_BIT_LENGTH_16BITS);
            codec.CtrlState(CodecMode.AUDIO_HAL_CODEC_MODE_BOTH, HalCtrl.AUDIO_HAL_CTRL_START);
            codec.SetVoiceVolume(100);

            nanoFramework.Hardware.Esp32.Configuration.SetPinFunction(0, DeviceFunction.I2S1_MCK);
            nanoFramework.Hardware.Esp32.Configuration.SetPinFunction(5, DeviceFunction.I2S1_BCK);
            nanoFramework.Hardware.Esp32.Configuration.SetPinFunction(25, DeviceFunction.I2S1_WS);
            nanoFramework.Hardware.Esp32.Configuration.SetPinFunction(26, DeviceFunction.I2S1_DATA_OUT);
            nanoFramework.Hardware.Esp32.Configuration.SetPinFunction(35, DeviceFunction.I2S1_MDATA_IN);
            I2sDevice i2s = new(new I2sConnectionSettings(1)
            {
                BitsPerSample = I2sBitsPerSample.Bit16,
                ChannelFormat = I2sChannelFormat.OnlyLeft,
                Mode = I2sMode.Master | I2sMode.Rx | I2sMode.Tx,
                CommunicationFormat = I2sCommunicationFormat.I2S,
                SampleRate = 44100,
            });

            SpanByte buff = new byte[1024];
            while (true)
            {
                i2s.Read(buff);
                i2s.Write(buff);
            }
        }
    }
}
