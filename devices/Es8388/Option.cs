using System;

namespace Es8388
{
    enum Module
    {
        ES_MODULE_MIN = -1,
        ES_MODULE_ADC = 0x01,
        ES_MODULE_DAC = 0x02,
        ES_MODULE_ADC_DAC = 0x03,
        ES_MODULE_LINE = 0x04,
        ES_MODULE_MAX
    }

    [Flags]
    internal enum DacOutput
    {
        DAC_OUTPUT_MIN = -1,
        DAC_OUTPUT_LOUT1 = 0x04,
        DAC_OUTPUT_LOUT2 = 0x08,
        DAC_OUTPUT_SPK = 0x09,
        DAC_OUTPUT_ROUT1 = 0x10,
        DAC_OUTPUT_ROUT2 = 0x20,
        DAC_OUTPUT_ALL = 0x3c,
        DAC_OUTPUT_MAX
    }

    [Flags]
    internal enum AdcInput
    {
        ADC_INPUT_MIN = -1,
        ADC_INPUT_LINPUT1_RINPUT1 = 0x00,
        ADC_INPUT_MIC1 = 0x05,
        ADC_INPUT_MIC2 = 0x06,
        ADC_INPUT_LINPUT2_RINPUT2 = 0x50,
        ADC_INPUT_DIFFERENCE = 0xf0,
        ADC_INPUT_MAX,
    }

    public enum I2sFmt
    {
        ES_I2S_MIN = -1,
        ES_I2S_NORMAL = 0,
        ES_I2S_LEFT = 1,
        ES_I2S_RIGHT = 2,
        ES_I2S_DSP = 3,
        ES_I2S_MAX
    }

    enum BitLength
    {
        BIT_LENGTH_MIN = -1,
        BIT_LENGTH_16BITS = 0x03,
        BIT_LENGTH_18BITS = 0x02,
        BIT_LENGTH_20BITS = 0x01,
        BIT_LENGTH_24BITS = 0x00,
        BIT_LENGTH_32BITS = 0x04,
        BIT_LENGTH_MAX,
    }

    public enum MicGain
    {
        MIC_GAIN_MIN = -1,
        MIC_GAIN_0DB = 0,
        MIC_GAIN_3DB = 3,
        MIC_GAIN_6DB = 6,
        MIC_GAIN_9DB = 9,
        MIC_GAIN_12DB = 12,
        MIC_GAIN_15DB = 15,
        MIC_GAIN_18DB = 18,
        MIC_GAIN_21DB = 21,
        MIC_GAIN_24DB = 24,
        MIC_GAIN_MAX,
    }

    public enum IfaceBits
    {
        AUDIO_HAL_BIT_LENGTH_16BITS = 1,   /*!< set 16 bits per sample */
        AUDIO_HAL_BIT_LENGTH_24BITS,       /*!< set 24 bits per sample */
        AUDIO_HAL_BIT_LENGTH_32BITS,       /*!< set 32 bits per sample */
    }
}
