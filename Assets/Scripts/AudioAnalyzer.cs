using UnityEngine;
using System.Collections;

public class AudioAnalyzer : MonoBehaviour {

    [Tooltip("Must be to the power of two")]
    public int samples;
    private float[] frequencyData;



    public int Samples
    {
        get
        {
            return samples;
        }
    }

   
    void Start () {
        frequencyData = new float[samples];
    }
	
	// Get the spectrum, size of the array is the amount of samples specified
	public float[] GetFrequencyData () {
        AudioListener.GetSpectrumData(frequencyData,0, FFTWindow.BlackmanHarris);
        return frequencyData;
	}

    
    //average spectrum values, size of 8
    //not flexible at the moment
    public float[] GetAverageValues()
    {
        int count = 0;
        //float diff = 0;
        AudioListener.GetSpectrumData(frequencyData, 0, FFTWindow.BlackmanHarris);
        float[] curValues = new float[8];
        for (int i = 0; i < 8; ++i)
        {
            float average = 0;

            int sampleCount = (int)Mathf.Pow(2, i) * 2;
            for (int j = 0; j < sampleCount; ++j)
            {
                average += frequencyData[count] * (count + 1);
                ++count;
            }
            average /= samples;
            //diff = Mathf.Clamp(average * 10 - curValues[i], 0, 4);
            curValues[i] = average * 10;
        }
        return curValues;
    }



    public float[] GetNormalizedValues()
    {
        AudioListener.GetOutputData(frequencyData, 0);
        return frequencyData;
    }
 

}
