using LitMotion;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PricePointController : MonoBehaviour
{
    [SerializeField] private PriceTag priceTagPrefabs;
    [SerializeField] private TMP_Text currentPriceTag;
    [SerializeField] private Transform currentPriceTagHolder;
    [SerializeField] private Transform priceTagContainer;

    private ObjectPool<PriceTag> priceTagPool;

    private List<PriceTag> activePriceTag = new List<PriceTag>();
    private List<decimal> yAxisValues = new List<decimal>();
    private float scaleY;
    private decimal minY;


    private void Start()
    {
        priceTagPool = new ObjectPool<PriceTag>(priceTagPrefabs, 0);
    }

    public Tuple<decimal, decimal> SetPricePoints(decimal maxPrice, decimal minPrice)
    {
        Debug.Log("Max: " + maxPrice + " Min: " + minPrice);
        CalculateIncrement(minPrice, maxPrice, 6);
        //CorrectFloatingPointErrors();
        int totalIncrement = yAxisValues.Count;
        int currentCount = activePriceTag.Count;

        for (int i = currentCount - 1; i >= 0; i--)
        {
            PriceTag temp = activePriceTag[i];
            temp.ReturnToPool();
            activePriceTag.RemoveAt(i);
        }


        for (int i = 0; i < totalIncrement; i++)
        {
            PriceTag temp = priceTagPool.Pull();
            temp.SetText(yAxisValues[i].ToString("0.######"));
            activePriceTag.Add(temp);
        }

        return new Tuple<decimal, decimal>(yAxisValues[0], yAxisValues[^1]);
    }

    public void SetPricePointsPos(float scaleY, decimal minY)
    {
        this.minY = minY;
        this.scaleY = scaleY;
        for (int i = 0; i < activePriceTag.Count; i++)
        {
            activePriceTag[i].transform.SetParent(priceTagContainer);
            activePriceTag[i].transform.localScale = Vector3.one;
            activePriceTag[i].transform.localPosition = new Vector3(0, (float)(yAxisValues[i] - minY) * scaleY, 0);
        }
    }

    public void SetCurrentPricePos(decimal currentPrice)
    {
        currentPriceTag.text = currentPrice.ToString("0.######");
        currentPriceTagHolder.localPosition = new Vector3(0, (float)(currentPrice - minY) * scaleY, 0);
    }

    public void SetCurrentPricePos(decimal currentPrice, float movTime)
    {
        currentPriceTag.text = currentPrice.ToString("0.######");

        //currentPriceTagHolder.DOLocalMoveY((float)(currentPrice - minY) * scaleY, movTime);

        float xPos = currentPriceTagHolder.transform.localPosition.x;
        LMotion.Create(currentPriceTagHolder.transform.localPosition.y, (float)(currentPrice - minY) * scaleY, movTime)
            .WithEase(LitMotion.Ease.InOutCubic)
            .BindWithState(currentPriceTagHolder, (y, target) => currentPriceTagHolder.transform.localPosition = new Vector3(xPos, y, 0));
    }

    private void CalculateIncrement(decimal min, decimal max, int numIntervals)
    {
        if (min == max)
        {
            // Find the last significant digit by counting the decimal places
            decimal adjustment = FindLastSignificantDigit(min);

            // Adjust the min and max values by ±1 on the last significant digit
            min -= adjustment;
            max += adjustment;
        }

        decimal range = max - min;
        decimal rawIncrement = range / (numIntervals - 1);
        decimal niceIncrement = GetNiceIncrement(rawIncrement);
        yAxisValues = new List<decimal>();

        // Start from a nice rounded minimum value
        decimal firstValue = Floor(min / niceIncrement) * niceIncrement;

        for (int i = 0; ; i++)
        {
            decimal yValue = firstValue + i * niceIncrement;
            yAxisValues.Add(yValue);
            if (yValue > max) return;
        }
    }

    private decimal GetNiceIncrement(decimal rawIncrement)
    {
        decimal exponent = Floor(Log10(rawIncrement));
        decimal fraction = rawIncrement / Pow(10, exponent);

        decimal niceFraction;

        if (fraction < 1.5m)
            niceFraction = 1m;
        else if (fraction < 3m)
            niceFraction = 2m;
        else if (fraction < 7m)
            niceFraction = 5m;
        else
            niceFraction = 10m;

        return niceFraction * Pow(10, exponent);
    }

    // Helper methods for decimal

    private decimal Floor(decimal value)
    {
        return Math.Floor(value);
    }

    private decimal Log10(decimal value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Logarithm is only defined for positive numbers.");
        }

        double doubleValue = (double)value;

        // Check if the double conversion caused overflow/underflow
        if (double.IsInfinity(doubleValue) || double.IsNaN(doubleValue))
        {
            throw new OverflowException("Value was either too large or too small for a Decimal.");
        }

        return (decimal)Math.Log10((double)doubleValue);
    }

    private decimal Pow(decimal value, decimal exponent)
    {
        return (decimal)Math.Pow((double)value, (double)exponent);
    }

    // Function to find the last significant digit adjustment
    private decimal FindLastSignificantDigit(decimal value)
    {
        // Get the number of decimal places
        int decimalPlaces = BitConverter.GetBytes(decimal.GetBits(value)[3])[2];

        // Adjust by the last significant digit
        return (decimal)Math.Pow(10, -decimalPlaces);
    }
}
