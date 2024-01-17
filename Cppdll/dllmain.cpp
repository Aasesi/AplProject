// dllmain.cpp : Definiuje punkt wejścia dla aplikacji DLL.
#include "pch.h"
/******************************************************************************
* Project Title: Image Binarization
* Description  : This algorithm is designed for the process of converting pixels
*                data from an image into binary image with main goal to separate
*                foreground from background. The process involves determining whether
*                each pixel should be classified as white or black based on individual
*                RGB channel values.
*
*
* Implementation Date: 15.01.2024
* Semester/Academic Year: Semester 5, Winter 2023/2024
* Author: Mikołaj Wilczyński, Konrad Kiełtyka, Weronika Źerańska
*
* Version: 1.0
*
*
******************************************************************************/


/**
 * @brief Applies binary thresholding to RGB color channels and produces a binary output.
 *
 * This function takes three separate arrays representing the red, green, and blue channels
 * of an image, and applies a binary threshold to each channel individually.
 * 
 *
 * @param redarr    Array containing the red channel values of the image.
 * @param greenarr  Array containing the green channel values of the image.
 * @param bluearr   Array containing the blue channel values of the image.
 * @param result    Output array where the binary result will be stored.
 * @param size      Size of the input arrays.
 * @param threshold Threshold value for channel comparison.
 *
 * @return          1 if the operation is successful.
 */
extern "C" __declspec(dllexport) int cppBinarize(int redarr[], int greenarr[], int bluearr[], int* result, int size, int threshold)
{
    for (int i = 0; i < size; i++)
    {
        int new_red = 0;
        int new_green = 0;
        int new_blue = 0;

        if (redarr[i] > threshold)
        {
            new_red = 1;
        }
        if (greenarr[i] > threshold)
        {
            new_green = 1;
        }
        if (bluearr[i] > threshold)
        {
            new_blue = 1;
        }
        int a = ((new_red && new_blue) || (new_blue && new_green) || (new_red && new_green));
        if (a == 1)
        {
            *(result + i) = 255;
        }
        else
        {
            *(result + i) = 0;
        }
    }
    return 1;
}
