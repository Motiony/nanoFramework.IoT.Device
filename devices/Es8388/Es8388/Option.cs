﻿using System;

namespace Es8388
{
    public enum Module
    {
        ES_MODULE_MIN = -1,
        ES_MODULE_ADC = 0x01,
        ES_MODULE_DAC = 0x02,
        ES_MODULE_ADC_DAC = 0x03,
        ES_MODULE_LINE = 0x04,
        ES_MODULE_MAX
    }

    public enum SCLK
    {
        MCLK_DIV_MIN = -1,
        MCLK_DIV_1 = 1,
        MCLK_DIV_2 = 2,
        MCLK_DIV_3 = 3,
        MCLK_DIV_4 = 4,
        MCLK_DIV_6 = 5,
        MCLK_DIV_8 = 6,
        MCLK_DIV_9 = 7,
        MCLK_DIV_11 = 8,
        MCLK_DIV_12 = 9,
        MCLK_DIV_16 = 10,
        MCLK_DIV_18 = 11,
        MCLK_DIV_22 = 12,
        MCLK_DIV_24 = 13,
        MCLK_DIV_33 = 14,
        MCLK_DIV_36 = 15,
        MCLK_DIV_44 = 16,
        MCLK_DIV_48 = 17,
        MCLK_DIV_66 = 18,
        MCLK_DIV_72 = 19,
        MCLK_DIV_5 = 20,
        MCLK_DIV_10 = 21,
        MCLK_DIV_15 = 22,
        MCLK_DIV_17 = 23,
        MCLK_DIV_20 = 24,
        MCLK_DIV_25 = 25,
        MCLK_DIV_30 = 26,
        MCLK_DIV_32 = 27,
        MCLK_DIV_34 = 28,
        MCLK_DIV_7 = 29,
        MCLK_DIV_13 = 30,
        MCLK_DIV_14 = 31,
        MCLK_DIV_MAX
    }

    public enum LCLK
    {
        LCLK_DIV_MIN = -1,
        LCLK_DIV_128 = 0,
        LCLK_DIV_192 = 1,
        LCLK_DIV_256 = 2,
        LCLK_DIV_384 = 3,
        LCLK_DIV_512 = 4,
        LCLK_DIV_576 = 5,
        LCLK_DIV_768 = 6,
        LCLK_DIV_1024 = 7,
        LCLK_DIV_1152 = 8,
        LCLK_DIV_1408 = 9,
        LCLK_DIV_1536 = 10,
        LCLK_DIV_2112 = 11,
        LCLK_DIV_2304 = 12,

        LCLK_DIV_125 = 16,
        LCLK_DIV_136 = 17,
        LCLK_DIV_250 = 18,
        LCLK_DIV_272 = 19,
        LCLK_DIV_375 = 20,
        LCLK_DIV_500 = 21,
        LCLK_DIV_544 = 22,
        LCLK_DIV_750 = 23,
        LCLK_DIV_1000 = 24,
        LCLK_DIV_1088 = 25,
        LCLK_DIV_1496 = 26,
        LCLK_DIV_1500 = 27,
        LCLK_DIV_MAX,
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

    public enum BitLength
    {
        BIT_LENGTH_MIN = -1,
        BIT_LENGTH_16BITS = 0x03,
        BIT_LENGTH_18BITS = 0x02,
        BIT_LENGTH_20BITS = 0x01,
        BIT_LENGTH_24BITS = 0x00,
        BIT_LENGTH_32BITS = 0x04,
        BIT_LENGTH_MAX,
    }

    enum MicGain
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
