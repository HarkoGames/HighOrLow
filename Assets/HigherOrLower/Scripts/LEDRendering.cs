﻿using UnityEngine;

public class LEDRendering{

    /// <summary>
    /// Increment the value,and wrap if needed. 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="max"></param>
    /// <param name="incrementVal"></param>
    /// <param name="invertAxis"></param>
    /// <returns>true if the value needed to wrap</returns>
    private static bool IncrementValue(ref int value, int max, int incrementVal)
    {
        bool wrapped = false;
        value += incrementVal;
        bool invertAxis = false;
        if (invertAxis)
        {
            if (value < 0)
            {
                value = max - 1;
                wrapped = true;
            }
        }
        else
        {
            if (value >= max)
            {
                value = 0;
                wrapped = true;
            }
        }
        return wrapped;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="startValue"></param>
    /// <param name="max"></param>
    /// <param name="incrementVal"></param>
    /// <param name="invertAxis"></param>
    /// <returns></returns>
    private static bool IncrementOffsetValue(ref int value, int startValue, int panelSize, int incrementVal, bool reverse = false)
    {
        int max = (panelSize - 1) - startValue;
        int min = 0 - startValue;
        bool invertAxis = false;

        bool wrapped = false;
        if (reverse)
        {
            incrementVal *= -1;
            invertAxis = !invertAxis;
        }
        value += incrementVal;
        if (invertAxis)
        {
            if (value < min)
            {
                value = max;
                wrapped = true;
            }
        }
        else
        {
            if (value > max)
            {
                value = min;
                wrapped = true;
            }
        }
        return wrapped;
    }

    public enum GraphType { Horizontal, Vertical, DiagonalX, DiagonalXFrom0 }

    public static void Graph(GraphType gType, LEDPanel panel, int startX, int startY, int count, bool invertX = false, bool invertY = false)
    {
        switch (gType)
        {
            case GraphType.Horizontal:
                HorizontalLines(panel, startX, startY, count, invertX, invertY);
                break;
            case GraphType.Vertical:
                VerticalLines(panel, startX, startY, count, invertX, invertY);
                break;
            case GraphType.DiagonalXFrom0:
                DiagonalLinesByXAxisFrom0(panel, startX, startY, count, invertX, invertY);
                break;
            case GraphType.DiagonalX:
                DiagonalLinesByXAxis(panel, startX, startY, count, invertX, invertY);
                break;
        }
    }

    private static void HorizontalLines(LEDPanel panel, int startX, int startY, int count, bool invertX = false, bool invertY = false)
    {
        panel.ClearLED();
        int lcvX = 0;
        int lcvY = startY;
        // boiler plate
        int xIncrement = 1;
        int yIncrement = 1;
        //
        int[,] values = new int[panel.LEDArraySize, panel.LEDArraySize];
        for (int lcv = 0; lcv < count; lcv++)
        {
            values[lcvX, lcvY] = 1;
            if (IncrementValue(ref lcvX, panel.LEDArraySize, xIncrement))
            {
                IncrementValue(ref lcvY, panel.LEDArraySize, yIncrement);
            }
        }
        RenderFromArray(panel, values, invertX, invertY);
    }

    private static void VerticalLines(LEDPanel panel, int startX, int startY, int count, bool invertX = false, bool invertY = false)
    {
        panel.ClearLED();
        int lcvX = startX;
        int lcvY = 0;
        // boiler plate
        int xIncrement = 1;
        int yIncrement = 1;
        //
        int[,] values = new int[panel.LEDArraySize, panel.LEDArraySize];
        for (int lcv = 0; lcv < count; lcv++)
        {
            values[lcvX, lcvY] = 1;
            if (IncrementValue(ref lcvY, panel.LEDArraySize, yIncrement))
            {
                IncrementValue(ref lcvX, panel.LEDArraySize, xIncrement);
            }
        }
        RenderFromArray(panel, values, invertX, invertY);
    }

    private static void DiagonalLinesByXAxisFrom0(LEDPanel panel, int startX, int startY, int count, bool invertX = false, bool invertY = false)
    {
        startX = 0;
        startY = 0;
        // boiler plate
        int xIncrement = 1;
        int yIncrement = 1;
        //
        panel.ClearLED();
        int lcvX = startX;
        int lcvY = startY;
        int yOffset = 0, xOffset = 0;
        int[,] values = new int[panel.LEDArraySize, panel.LEDArraySize];
        for (int lcv = 0; lcv < count; lcv++)
        {
            bool xWrap = false, yWrap = false;
            values[lcvX, lcvY] = 1;
            var msg = string.Format("setting x:{0},y:{1}, ", lcvX, lcvY);
            if (lcvX == panel.LEDArraySize - 1 && lcvY == 0) // top right
            {
                IncrementOffsetValue(ref yOffset, startY, panel.LEDArraySize, yIncrement);
                lcvY = startY + yOffset;
                lcvX = 0;

            }
            else
            {
                xWrap = IncrementValue(ref lcvX, panel.LEDArraySize, xIncrement);
                yWrap = IncrementValue(ref lcvY, panel.LEDArraySize, yIncrement);
            }
            if (xWrap)
            {
                IncrementOffsetValue(ref xOffset, startX, panel.LEDArraySize, xIncrement);
                lcvY = 0;
                lcvX = startX + xOffset;
            }
            else if (yWrap)
            {
                IncrementOffsetValue(ref yOffset, startY, panel.LEDArraySize, yIncrement);
                lcvY = startY + yOffset;
                lcvX = 0;
            }
            msg += string.Format(" next up x:{0}, y:{1}", lcvX, lcvY);
            //Debug.Log(msg);
        }
        RenderFromArray(panel, values, invertX, invertY);
    }

    private static void DiagonalLinesByXAxis(LEDPanel panel, int startX, int startY, int count, bool invertX = false, bool invertY = false)
    {
        panel.ClearLED();
        // boiler plate
        int xIncrement = 1;
        int yIncrement = 1;

        int lowX = 0, highX = panel.LEDArraySize - 1;
        int lowY = 0, highY = panel.LEDArraySize - 1;

        int deadEndX = panel.LEDArraySize - 1;
        int deadEndY =  0;
        panel.ClearLED();
        int lcvX = startX;
        int lcvY = startY;
        int yOffset = 0, xOffset = 0;
        int[,] values = new int[panel.LEDArraySize, panel.LEDArraySize];
        for (int lcv = 0; lcv < count; lcv++)
        {
            bool xWrap = false, yWrap = false;
            values[lcvX, lcvY] = 1;
            var msg = string.Format("setting x:{0},y:{1}, ", lcvX, lcvY);
            if (lcvX == deadEndX && lcvY == deadEndY) // dead end corner
            {
                IncrementOffsetValue(ref yOffset, startY, panel.LEDArraySize, yIncrement,  true);
                lcvY = startY + yOffset;
                lcvX = lowX;
                xOffset = lowX - startX;
            }
            else
            {
                xWrap = IncrementValue(ref lcvX, panel.LEDArraySize, xIncrement);
                yWrap = IncrementValue(ref lcvY, panel.LEDArraySize, yIncrement);
            }
            if (xWrap)
            {
                IncrementOffsetValue(ref xOffset, startX, panel.LEDArraySize, xIncrement);
                lcvY = lowY;
                lcvX = startX + xOffset;
            }
            else if (yWrap)
            {
                IncrementOffsetValue(ref yOffset, startY, panel.LEDArraySize, yIncrement, true);
                lcvY = startY + yOffset;
                lcvX = lowX;
            }
            msg += string.Format(" next up x:{0}, y:{1}", lcvX, lcvY);
            //Debug.Log(msg);
        }
        RenderFromArray(panel, values, invertX, invertY);
    }

    private static void RenderFromArray(LEDPanel panel, int[,] values, bool invertX, bool invertY)
    {
        int xIncrement = invertX ? -1 : 1;
        int yIncrement = invertY ? -1 : 1;

        int lowX = 0, highX = values.GetLength(0) - 1;
        int lowY = 0, highY = values.GetLength(1) - 1;

        if (invertX)
        {
            int temp = lowX;
            lowX = highX;
            highX = temp;
        }
        if (invertY)
        {
            int temp = lowY;
            lowY = highY;
            highY = temp;
        }

        int[,] newValues = new int[values.GetLength(0), values.GetLength(1)];
/*
        int oldX = 0, oldY = 0;
        if (invertX || invertY)
        {
            for (int x = lowX; x <= highX; x += xIncrement)
            {
                for (int y = lowY; y <= highY; y += yIncrement)
                {
                    newValues[x, y] = values[oldX, oldY];
                    oldY++;
                }
                oldX++;
            }
        }
        else
        {
            newValues = values;
        }
        */
        int oldX = lowX;
        for (int x = 0; x < values.GetLength(0); x++)
        {
            int oldY = lowY;
            for (int y = 0; y < values.GetLength(1); y++)
            {
                if (values[oldX,oldY] == 1)
                {
                    panel.SetLED(x, y, true);
                }
                oldY += yIncrement;
            }
            oldX += xIncrement;
        }
    }
}
