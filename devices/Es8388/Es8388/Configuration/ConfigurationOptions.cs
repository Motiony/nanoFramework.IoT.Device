namespace Es8388.Configuration
{
    public enum AdcInput
    {
        AUDIO_HAL_ADC_INPUT_LINE1 = 0x00,  /*!< mic input to adc channel 1 */
        AUDIO_HAL_ADC_INPUT_LINE2,         /*!< mic input to adc channel 2 */
        AUDIO_HAL_ADC_INPUT_ALL,           /*!< mic input to both channels of adc */
        AUDIO_HAL_ADC_INPUT_DIFFERENCE,    /*!< mic input to adc difference channel */
    }

    public enum DacOutput
    {
        AUDIO_HAL_DAC_OUTPUT_LINE1 = 0x00,  /*!< dac output signal to channel 1 */
        AUDIO_HAL_DAC_OUTPUT_LINE2,         /*!< dac output signal to channel 2 */
        AUDIO_HAL_DAC_OUTPUT_ALL,           /*!< dac output signal to both channels */
    }

    public enum Mode
    {
        AUDIO_HAL_MODE_SLAVE = 0x00,   /*!< set slave mode */
        AUDIO_HAL_MODE_MASTER = 0x01,  /*!< set master mode */
    }

    public enum CodecMode
    {
        AUDIO_HAL_CODEC_MODE_ENCODE = 1,  /*!< select adc */
        AUDIO_HAL_CODEC_MODE_DECODE,      /*!< select dac */
        AUDIO_HAL_CODEC_MODE_BOTH,        /*!< select both adc and dac */
        AUDIO_HAL_CODEC_MODE_LINE_IN,     /*!< set adc channel */
    }

    public enum HalCtrl
    {
        AUDIO_HAL_CTRL_STOP = 0x00,  /*!< set stop mode */
        AUDIO_HAL_CTRL_START = 0x01,  /*!< set start mode */
    }
}
