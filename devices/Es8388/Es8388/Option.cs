namespace Es8388
{
    public enum MixSelect
    {
        MIXIN1,  // direct line 1
        MIXIN2,  // direct line 2
        MIXRES,  // reserverd es8388
        MIXADC   // Select from ADC/ALC
    }

    public enum OutSelect
    {
        OUT1,    // Select Line OUT L/R 1
        OUT2,    // Select Line OUT L/R 2
        OUTALL,  // Enable ALL
    }

    public enum InSelect
    {
        IN1,      // Select Line IN L/R 1
        IN2,      // Select Line IN L/R 2
        IN1DIFF,  // differential IN L/R 1
        IN2DIFF   // differential IN L/R 2
    }

    public enum MixerControl
    {
        DACOUT,     // Select Sink From DAC
        SRCSELOUT,  // Select Sink From SourceSelect()
        MIXALL,     // Sink ALL DAC & SourceSelect()
    }

    public enum AlcModeSelect
    {
        DISABLE,  // Disable ALC
        GENERIC,  // Generic Mode
        VOICE,    // Voice Mode
        MUSIC     // Music Mode
    }
}
