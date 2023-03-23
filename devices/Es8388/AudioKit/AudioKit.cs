using Es8388;
using Es8388.Configuration;
using System.Diagnostics;

namespace AudioKit
{
    public class AudioKit
    {
        private readonly Es8388.Es8388 _es83881;

        public AudioKit(Es8388.Es8388 es83881)
        {
            _es83881 = es83881;
        }

        public void Init(CodecConfiguration codecConfiguration)
        {
            Debug.WriteLine(_es83881.Init(codecConfiguration).ToString());
            Debug.WriteLine(_es83881.Configi2s(I2sFmt.ES_I2S_NORMAL, IfaceBits.AUDIO_HAL_BIT_LENGTH_16BITS).ToString());
            Debug.WriteLine(_es83881.CtrlState(CodecMode.AUDIO_HAL_CODEC_MODE_BOTH, HalCtrl.AUDIO_HAL_CTRL_START).ToString());
            Debug.WriteLine(_es83881.SetVoiceVolume(100).ToString());
        }
    }
}
