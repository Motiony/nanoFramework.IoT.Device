using nanoFramework.Hardware.Esp32;
using System;
using System.Device.I2c;
using System.Device.I2s;
using System.Diagnostics;
using System.Threading;

while (!Debugger.IsAttached)
{
    Thread.Sleep(500);
}

Configuration.SetPinFunction(18, DeviceFunction.I2C1_DATA);
Configuration.SetPinFunction(23, DeviceFunction.I2C1_CLOCK);
I2cDevice i2cDevice = new(new I2cConnectionSettings(1, Es8388.Es8388.DefaultI2cAddress, I2cBusSpeed.StandardMode));
var audioKit = new AudioKit.AudioKit(new Es8388.Es8388(i2cDevice));
audioKit.Init(new Es8388.Configuration.CodecConfiguration
{
    Input = Es8388.Configuration.AdcInput.AUDIO_HAL_ADC_INPUT_LINE2,
    Mode = Es8388.Configuration.Mode.AUDIO_HAL_MODE_SLAVE,
    Output = Es8388.Configuration.DacOutput.AUDIO_HAL_DAC_OUTPUT_ALL
});

Configuration.SetPinFunction(0, DeviceFunction.I2S1_MCK);
Configuration.SetPinFunction(5, DeviceFunction.I2S1_BCK);
Configuration.SetPinFunction(25, DeviceFunction.I2S1_WS);
Configuration.SetPinFunction(26, DeviceFunction.I2S1_DATA_OUT);
Configuration.SetPinFunction(35, DeviceFunction.I2S1_MDATA_IN);

I2sDevice i2s = new(new I2sConnectionSettings(1)
{
    BitsPerSample = I2sBitsPerSample.Bit16,
    ChannelFormat = I2sChannelFormat.OnlyLeft,
    Mode = I2sMode.Master | I2sMode.Rx | I2sMode.Tx,
    CommunicationFormat = I2sCommunicationFormat.I2S,
    SampleRate = 44100,

});

// should be one second of sound data:
SpanByte buff = new byte[1024];
while (true)
{
    i2s.Read(buff);
    i2s.Write(buff);
    Debug.WriteLine(buff[4].ToString());
}

