using UnityEngine;
using System.Collections;

public class SineFit
{
    //constants
    private const int NUM_STEPS = 10;
    private const float SF_PI = 3.141592653589793f;
    private const float RUNNING_K_WEIGHT = 0.95f;
    private const float RUNNING_K_THRESHOLD = 0.90f;
    private const int NUM_SAMPLES = 100;
    private const int SUPERSAMPLE_STEP = 3;
    private const int NUM_SUPERSAMPLES = NUM_SAMPLES * SUPERSAMPLE_STEP;
    private const int CONV_SIZE = 9;
    //variables
    private float[] superdata = new float[NUM_SUPERSAMPLES];
    private float[] data = new float[NUM_SAMPLES];
    private float[] rawdata = new float[NUM_SAMPLES];
    private float[] temp_data1 = new float[NUM_SAMPLES];
    private float[] temp_data2 = new float[NUM_SAMPLES];
    private float[] conv = new float[CONV_SIZE];

    //Bret: y = A sin (B + Cx) + D 

    private float A; //amplitude
    private float B; // horizontal offset
    private float C; // 2pi/period?
    private float D; // vertical offset
    private float rms;
    private float running_k;

    public SineFit()
    {
        conv[0] = 1f / 248.0f;
        conv[1] = 8f / 248.0f;
        conv[2] = 28f / 248.0f;
        conv[3] = 56f / 248.0f;
        conv[4] = 70f / 248.0f;
        conv[5] = 56f / 248.0f;
        conv[6] = 28f / 248.0f;
        conv[7] = 8f / 248.0f;
        conv[8] = 1f / 248.0f;

        for (int i = 0; i < NUM_SAMPLES; i++)
        {
            data[i] = 0;
            rawdata[i] = 0;
            temp_data1[i] = 0;
            temp_data2[i] = 0;
        }
        for (int i = 0; i < NUM_SUPERSAMPLES; i++)
        {
            superdata[i] = 0;
        }
        A = 0;
        B = 0;
        C = 0;
        D = 0;

        running_k = 0;

    }
    public void addSample(float sample)
    {
        for (int i = 0; i < NUM_SUPERSAMPLES - 1; i++)
        {
            superdata[i] = superdata[i + 1];
        }
        superdata[NUM_SUPERSAMPLES - 1] = sample;
        for (int i = 0; i < NUM_SAMPLES; i++)
        {
            float accum = 0;
            for (int j = 0; j < SUPERSAMPLE_STEP; j++)
            {
                accum += superdata[i * SUPERSAMPLE_STEP + j];
            }
            data[i] = accum / SUPERSAMPLE_STEP;
        }
    }
    private float fitQuality(float[] first, float[] second)
    {
        float qual = 0f;
        for (int i = 0; i < NUM_SAMPLES; i++)
        {
            qual += Mathf.Pow(first[i] - second[i], 2) * (NUM_SAMPLES + i);
        }
        return qual;
    }

    // Bret: Given a set of equation parameters, returns the an error metric
    private float fitQualNogen(float a, float b, float c, float d)
    {
        float qual = 0f;
        for (int i = 0; i < NUM_SAMPLES; i++)
        {
            qual += Mathf.Pow(data[i] - (a * Mathf.Sin(b + c * i) + d), 2) * (NUM_SAMPLES + i);
        }
        return qual;
    }
    private void generate_sine(float[] dat, float a, float b, float c, float d)
    {
        for (int i = 0; i < NUM_SAMPLES; i++)
        {
            dat[i] = a * Mathf.Sin(b + c * i) + d;
        }
    }

    //Bret: Determine some good starting parameters for the equation based on the data
    private void estimateInital()
    {
        float period;
        int startIndex;
        int firstCrossing;
        int lastCrossing;
        float initialValue;
        /* Calculate mean. D is the vertical offset of the wave which is just the mean */
        D = 0;
        for (int i = 0; i < NUM_SAMPLES; i++)
        {
            D += data[i];
        }
        D /= NUM_SAMPLES;


        /* Calculate RMS error. i.e. on average how far off is each sample */
        rms = 0;
        for (int i = 0; i < NUM_SAMPLES; i++)
        {
            rms += Mathf.Pow(data[i] - D, 2);
        }
        rms = Mathf.Sqrt(rms / NUM_SAMPLES);

        // Calculate the amplitude based on rms. This equation is from: https://en.wikipedia.org/wiki/Root_mean_square
        A = rms * Mathf.Sqrt(2.0f);


        //Bret the following code finds the first time that the data crosses either the mean + or - the rms
        // We then find the last time it crosses and the number of crossings. This is used to calculate the period of the wave

        /* Find initial crossing */
        startIndex = -1;
        initialValue = 0;
        for (int i = 0; i < NUM_SAMPLES; i++)
        {
            if (data[i] > D + rms)
            {
                /* First crossing is past +rms */
                startIndex = i;
                initialValue = rms;
                break;
            }
            else if (data[i] < D - rms)
            {
                /* First crossing is past -rms */
                startIndex = i;
                initialValue = -rms;
                break;
            }
        }
        if (startIndex == -1)
        {
            /* No match was found, so we see what gradient descent can do */
            return;
        }
        /* Find average period length */
        lastCrossing = startIndex;
        int j = 0;
        firstCrossing = -1;
        initialValue *= -1;
        for (int i = startIndex + 1; i < NUM_SAMPLES; i++)
        {
            if ((initialValue < 0 && data[i] < initialValue - D)
                || (initialValue > 0 && data[i] > initialValue + D))
            {
                if (firstCrossing < 0)
                {
                    firstCrossing = i;
                }
                initialValue *= -1;
                lastCrossing = i;
                j++;
            }
        }
        if (j < 2)
        {
            //A = 0;
            return;
        }
        period = (float)(lastCrossing - firstCrossing) / (j - 1);
        C = SF_PI / period;
        B = SF_PI * -lastCrossing / period + SF_PI / 4;
        if (initialValue > 0)
        {
            B += SF_PI;
        }
    }

    public void doFit()
    {
        int i;
        float delta;
        float total;
        float dA, dB, dC, dD;
        float aG, bG, cG, dG;
        float nA, nB, nC, nD;
        float origQual;
        float newQual;
        B += C;
        origQual = fitQualNogen(A, B, C, D);
        nA = A;
        nB = B;
        nC = C;
        nD = D;
        estimateInital();
        newQual = fitQualNogen(A, B, C, D);
        if (newQual < origQual)
        {
            origQual = newQual;
        }
        else
        {
            A = nA;
            B = nB;
            C = nC;
            D = nD;
        }

        //Perform gradient descent to find the local minimum of the parameters that give the lowest rms error.
        for (i = 0; i < NUM_STEPS; i++)
        {
            delta = Mathf.Pow(((float)(NUM_STEPS + 1 - i)) / (2 * NUM_STEPS), 2); // arbitrary adaptive step size that gets smaller as the iterations increase.

            //Bret: Not sure where the constants at the end came from. Probably experimenting to weight how much each parameter is going to change
            dA = rms * delta * 0.2f;
            dB = SF_PI * delta * 0.4f;
            dC = C * delta * 0.1f;
            dD = rms * delta * 0.1f;

            // Calculate the gradients of the rms for each parameter. I.e. sort of like taking the partial derivatives
            aG = fitQualNogen(A + dA, B, C, D) - fitQualNogen(A - dA, B, C, D);
            bG = fitQualNogen(A, B + dB, C, D) - fitQualNogen(A, B - dB, C, D);
            cG = fitQualNogen(A, B, C + dC, D) - fitQualNogen(A, B, C - dC, D);
            dG = fitQualNogen(A, B, C, D + dD) - fitQualNogen(A, B, C, D - dD);
            total = -Mathf.Sqrt(aG * aG + bG * bG + cG * cG + dG * dG);
            nA = A + dA * aG / total; // normalize the gradient? not sure why you divide by the total
            nB = B + dB * bG / total;
            nC = C + dC * cG / total;
            nD = D + dD * dG / total;
            newQual = fitQualNogen(nA, nB, nC, nD);
            if (newQual < origQual)
            {
                origQual = newQual;
                A = nA;
                B = nB;
                C = nC;
                D = nD;
            }
        }
    }
    public float getA()
    {
        return A;
    }
    public float getB()
    {
        return B;
    }
    public float getC()
    {
        return C;
    }
    public float getD()
    {
        return D;
    }
    public float getVelocity()
    {
        float k;
        //Don't move forward if we aren't getting a clear periodic wave.
        if (C > 0.001f)
        {
            k = A;
            running_k = running_k * RUNNING_K_WEIGHT + k * (1 - RUNNING_K_WEIGHT); // alpha weighting so more recent entries count for more.
            if (k > running_k * RUNNING_K_THRESHOLD)
            {
                return 0.1f * Mathf.Sqrt(Mathf.Abs(A * Mathf.Cos(B + C * NUM_SAMPLES)));//Bret This is almost taking the derivative of the sine wave to find the velocity at the most recent sample (It should be ACcos(Cx+B) )
                                                                                        // I think the sqrt was added to smooth the velocity a bit and make it less jarring when you start moving.
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return 0;
        }
    }
}
